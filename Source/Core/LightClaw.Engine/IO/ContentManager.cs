using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents a caching <see cref="IContentManager"/>.
    /// </summary>
    /// <seealso cref="IContentManager"/>
    public class ContentManager : DisposableEntity, IContentManager
    {
        /// <summary>
        /// Contains <see cref="AsyncLock"/>s used to lock access to a specific asset while it is being loaded.
        /// </summary>
        private readonly ConcurrentDictionary<ResourceString, AsyncLock> assetLocks = new ConcurrentDictionary<ResourceString, AsyncLock>();

        /// <summary>
        /// Represents the asset cache. Assets are cached using weak references to reduce memory pressure.
        /// </summary>
        private readonly ConcurrentDictionary<ResourceKey, WeakReference<object>> cachedAssets = new ConcurrentDictionary<ResourceKey, WeakReference<object>>();

        /// <summary>
        /// A collection of all registered <see cref="IContentReader"/>s.
        /// </summary>
        private readonly ConcurrentBag<IContentReader> readers = new ConcurrentBag<IContentReader>();

        /// <summary>
        /// A collection of all registered <see cref="IContentResolver"/>s.
        /// </summary>
        private readonly ConcurrentBag<IContentResolver> resolvers = new ConcurrentBag<IContentResolver>();

        /// <summary>
        /// Notifies about the start of an asset loading process.
        /// </summary>
        public event EventHandler<ParameterEventArgs> AssetLoading;

        /// <summary>
        /// Notifies about the end of an asset loading process.
        /// </summary>
        public event EventHandler<ParameterEventArgs> AssetLoaded;

        /// <summary>
        /// Notifies about the start of <see cref="GetStreamAsync"/>.
        /// </summary>
        /// <seealso cref="GetStreamAsync"/>
        public event EventHandler<ParameterEventArgs> StreamObtaining;

        /// <summary>
        /// Notifies about the end of <see cref="GetStreamAsync"/>.
        /// </summary>
        /// <seealso cref="GetStreamAsync"/>
        public event EventHandler<ParameterEventArgs> StreamObtained;

        /// <summary>
        /// Initializes a new <see cref="ContentManager"/> using the default <see cref="IContentReader"/>s and
        /// <see cref="IContentResolver"/>s.
        /// </summary>
        public ContentManager()
            : this(GetDefaultReaders(), GetDefaultResolvers())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ContentManager"/> from the specified <see cref="IContentReader"/>s and
        /// <see cref="IContentResolver"/>s.
        /// </summary>
        /// <param name="readers">The <see cref="IContentReader"/>s to use.</param>
        /// <param name="resolvers">The <see cref="IContentResolver"/>s to use.</param>
        /// <remarks>
        /// The <see cref="ContentManager"/> will assume ownage and dispose the <see cref="IContentReader"/>s and
        /// <see cref="IContentResolver"/>s (in case it implements IDisposable) on its disposal.
        /// </remarks>
        public ContentManager(IEnumerable<IContentReader> readers, IEnumerable<IContentResolver> resolvers)
        {
            Contract.Requires<ArgumentNullException>(readers != null);
            Contract.Requires<ArgumentNullException>(resolvers != null);

            this.Register(readers.FilterNull());
            this.Register(resolvers.FilterNull());
        }

        /// <summary>
        /// Checks whether an asset with the specified resource string exists.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the process.</param>
        /// <param name="resourceString">The resource string to check for.</param>
        /// <returns><c>true</c> if the asset exists, otherwise <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(ResourceString resourceString, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, key => new AsyncLock()).LockAsync())
            {
                return await this.resolvers.Select(resolver => resolver.ExistsAsync(resourceString, token)).AnyAsync(b => b);
            }
        }

        /// <summary>
        /// Gets a <u>writable</u> <see cref="Stream"/> around a specific resource string. If there is no asset with the
        /// specified resource string, it will be created.
        /// </summary>
        /// <remarks>This is the engine's main asset output (save-file, etc.) interface.</remarks>
        /// <param name="resourceString">The resource string to obtain a <see cref="Stream"/> around.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the stream obtaining process.</param>
        /// <returns>A <see cref="Stream"/> wrapping the specified asset.</returns>
        /// <seealso cref="Stream"/>
        public async Task<Stream> GetStreamAsync(ResourceString resourceString, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Logger.Debug(rs => "Obtaining stream around '{0}'.".FormatWith(rs), resourceString);

            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, key => new AsyncLock()).LockAsync())
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.StreamObtaining, this.StreamObtained, resourceString, resourceString))
            {
                try
                {
                    return await this.resolvers.Select(resolver => resolver.GetStreamAsync(resourceString, true, token))
                                               .FirstAsync(s => s != null);
                }
                catch (InvalidOperationException ex)
                {
                    Logger.Warn(rs => "No writable stream around '{0}' found.".FormatWith(rs), resourceString);
                    throw new FileNotFoundException(
                        "No writable stream was found. If reading is required only, consider registering an IContentReader in combination with LoadAsync.",
                        ex
                    );
                }
            }
        }

        /// <summary>
        /// Asynchronously loads the asset with the specified resource string.
        /// </summary>
        /// <param name="resourceString">The resource string of the asset to load.</param>
        /// <param name="assetType">The <see cref="Type"/> of asset to load.</param>
        /// <param name="parameter">
        /// A custom parameter that is handed to the <see cref="IContentReader"/>s to provide them with additional
        /// information about the asset being read. 
        /// <example> 
        /// Imagine a content reader reading texture files to a
        /// generic texture class. It needs information about the file type of the image to load to be able to properly
        /// load it. 
        /// </example>
        /// </param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the content loading process.</param>
        /// <param name="forceReload">
        /// Indicates whether to force-load the asset from the disk and bypass any caching structures.
        /// </param>
        /// <returns>The loaded asset.</returns>
        /// <exception cref="FileNotFoundException">The asset could not be found.</exception>
        /// <exception cref="InvalidOperationException">The asset could not be deserialized from the stream.</exception>
        public async Task<object> LoadAsync(ResourceString resourceString, Type assetType, object parameter, CancellationToken token, bool forceReload)
        {
            token.ThrowIfCancellationRequested();
            Logger.Debug((at, rs) => "Loading an asset of type '{0}' from resource '{1}'.".FormatWith(at.AssemblyQualifiedName, rs), assetType, resourceString);

            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, key => new AsyncLock()).LockAsync())
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.AssetLoading, this.AssetLoaded, resourceString, resourceString))
            {
                WeakReference<object> cachedAsset = null;
                object asset = null;

                if (forceReload || // Load if reload forced,
                    !this.cachedAssets.TryGetValue(new ResourceKey(resourceString, assetType), out cachedAsset) || // no cache available or
                    !cachedAsset.TryGetTarget(out asset)) // weak reference to cached asset collected.
                {
                    Logger.Debug(
                        (fr, rs) => ((fr ? "Reload of '{0}' forced" : "No cached version of '{0}' available") + ", obtaining stream...").FormatWith(rs),
                        forceReload, resourceString
                    );
                    token.ThrowIfCancellationRequested();
                    using (Stream assetStream = await this.resolvers.Select(resolver => resolver.GetStreamAsync(resourceString, false, token))
                                                                    .FirstFinishedOrDefaultAsync(s => s != null))
                    {
                        // If the stream could not be found, throw exception
                        if (assetStream == null)
                        {
                            string message = "Asset '{0}' could not be found.".FormatWith(resourceString);
                            Logger.Warn(message);
                            throw new FileNotFoundException(message);
                        }

                        // Try to deserialize using registered content readers
                        Logger.Debug(rs => "Stream around '{0}' obtained, obtaining reader...".FormatWith(rs), resourceString);
                        IContentReader reader = this.GetReader(assetType, parameter);
                        if (reader != null)
                        {
                            Logger.Debug((rs, at) => "Reader for asset '{0}' of type '{1}' obtained, deserializing...".FormatWith(rs, at.FullName), resourceString, assetType);
                            token.ThrowIfCancellationRequested();
                            asset = await reader.ReadAsync(new ContentReadParameters(this, resourceString, assetType, assetStream, token, parameter));
                        }
                        else
                        {
                            Logger.Debug(at => "{0} for asset type '{1}' could not be obtained.".FormatWith(typeof(IContentReader).Name, at.FullName), assetType);
                        }

                        // If no content reader was able to deserialize the asset, forget it and throw exception
                        if (asset == null)
                        {
                            string message = "Asset '{0}' could not be deserialized.".FormatWith(resourceString);
                            Logger.Warn(message);
                            throw new InvalidOperationException(message);
                        }
                    }

                    cachedAsset = new WeakReference<object>(asset);
                    this.cachedAssets.TryAdd(new ResourceKey(resourceString, assetType), cachedAsset);
                }
                else
                {
                    Logger.Debug(rs => "Cached version of '{0}' available, returning that instead.".FormatWith(rs), resourceString);
                    // Remark for the future: No need to use cachedAsset.TryGetTarget, we've already called it inside the if.
                }

                Logger.Debug(rs => "Asset '{0}' loaded successfully.".FormatWith(rs), resourceString);
                return asset;
            }
        }

        /// <summary>
        /// Registers a new <see cref="IContentReader"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IContentManager"/> will assume ownage and dispose the <see cref="IContentReader"/> (in case
        /// it implements IDisposable) on its disposal.
        /// </remarks>
        /// <param name="reader">The <see cref="IContentReader"/> to register.</param>
        /// <seealso cref="IContentReader"/>
        public void Register(IContentReader reader)
        {
            this.readers.Add(reader);
        }

        /// <summary>
        /// Registers a new <see cref="IContentResolver"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IContentManager"/> will assume ownage and dispose the <see cref="IContentResolver"/> (in case
        /// it implements IDisposable) on its disposal.
        /// </remarks>
        /// <param name="resolver">The <see cref="IContentResolver"/> to register.</param>
        /// <seealso cref="IContentResolver"/>
        public void Register(IContentResolver resolver)
        {
            this.resolvers.Add(resolver);
        }

        /// <summary>
        /// Disposes the <see cref="IContentManager"/> disposing of all of the <see cref="IContentReader"/>s and
        /// <see cref="IContentResolver"/>s.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose of managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            // Some funny ODE occurs here w/o the blocks. I wonder whether any of the readers / resolvers will be
            // disposed before the ODE is thrown by the ConcurrentBag...?!
            try
            {
                IContentResolver resolver;
                while (this.resolvers.TryTake(out resolver))
                {
                    IDisposable disposableResolver = resolver as IDisposable;
                    if (disposableResolver != null)
                    {
                        disposableResolver.Dispose();
                    }
                }
            }
            catch (ObjectDisposedException) { }

            try
            {
                IContentReader reader;
                while (this.readers.TryTake(out reader))
                {
                    IDisposable disposableReader = reader as IDisposable;
                    if (disposableReader != null)
                    {
                        disposableReader.Dispose();
                    }
                }
            }
            catch (ObjectDisposedException) { }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the <see cref="IContentReader"/> for the specified <paramref name="assetType"/>.
        /// </summary>
        /// <param name="assetType">The <see cref="Type"/> of asset to get the <see cref="IContentReader"/> for.</param>
        /// <returns>The <see cref="IContentReader"/> for the asset of the specified <see cref="Type"/>.</returns>
        private IContentReader GetReader(Type assetType, object parameter)
        {
            Contract.Requires<ArgumentNullException>(assetType != null);

            IContentReader reader = this.readers.FirstOrDefault(rdr => rdr.CanRead(assetType, parameter));
            if (reader == null)
            {
                Logger.Debug(t => "None of the registered readers could read assets of the specified type. Attempting to get the {0} through the attribute for an asset of type '{1}'.".FormatWith(typeof(IContentReader).Name, t.FullName), assetType);
                ContentReaderAttribute attr = assetType.GetCustomAttribute<ContentReaderAttribute>();
                if (attr != null && (reader = (IContentReader)CreateInstanceOrDefault(attr.ContentReaderType)) != null)
                {
                    this.readers.Add(reader);
                }
            }
            return (reader != null && reader.CanRead(assetType, parameter)) ? reader : null;
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.readers.All(reader => reader != null));
            Contract.Invariant(this.resolvers.All(resolver => resolver != null));
        }

        /// <summary>
        /// Creates an instance of the specified <see cref="Type"/> or returns null if a failure occurs.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to create an instance of.</typeparam>
        /// <returns>The created instance or <c>null</c> if instance-creation failed.</returns>
        public static T CreateInstanceOrDefault<T>()
        {
            object instance;
            return (TryCreateInstance(typeof(T), out instance) && instance != null) ? (T)instance : default(T);
        }

        /// <summary>
        /// Creates an instance of the specified <see cref="Type"/> or returns null if a failure occurs.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> to create an instance of.</param>
        /// <returns>The created instance or <c>null</c> if instance-creation failed.</returns>
        public static object CreateInstanceOrDefault(Type t)
        {
            Contract.Requires<ArgumentNullException>(t != null);

            object instance;
            return TryCreateInstance(t, out instance) ? instance : null;
        }

        /// <summary>
        /// Gets all default <see cref="IContentReader"/>s.
        /// </summary>
        /// <returns>All default <see cref="IContentReader"/>s.</returns>
        private static IEnumerable<IContentReader> GetDefaultReaders()
        {
            Contract.Ensures(Contract.Result<IEnumerable<IContentReader>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<IContentReader>>().All(reader => reader != null));

            return new[] { new PrimitiveReader() };
        }

        /// <summary>
        /// Gets all default <see cref="IContentResolver"/>s.
        /// </summary>
        /// <returns>All default <see cref="IContentResolver"/>s.</returns>
        private static IEnumerable<IContentResolver> GetDefaultResolvers()
        {
            Contract.Ensures(Contract.Result<IEnumerable<IContentResolver>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<IContentResolver>>().All(resolver => resolver != null));

#if DESKTOP
            return new[] { new FileSystemContentResolver() };
#else
            throw new NotImplementedException("There currently are no IContentResolvers for platforms other than desktop.");
#endif
        }

        /// <summary>
        /// Tries to create an instance of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> of object to create an instance from.</param>
        /// <param name="instance">The newly created instance, if the method succeeds.</param>
        /// <returns><c>true</c> if the instance could be created, otherwise <c>false</c>.</returns>
        private static bool TryCreateInstance(Type t, out object instance)
        {
            Contract.Requires<ArgumentNullException>(t != null);

            try
            {
                instance = Activator.CreateInstance(t);
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        /// <summary>
        /// Represents a unique key of an asset including the path and its type.
        /// </summary>
        /// <remarks>
        /// Because an asset can be loaded as different types (e.g. shader source as string or as shader) storing the
        /// <see cref="ResourceString"/> as cache key is not enough.
        /// </remarks>
        private struct ResourceKey : ICloneable, IEquatable<ResourceKey>
        {
            /// <summary>
            /// The assets <see cref="ResourceString"/>.
            /// </summary>
            public ResourceString ResourceString { get; private set; }

            /// <summary>
            /// The assets <see cref="Type"/>.
            /// </summary>
            public Type Type { get; private set; }

            /// <summary>
            /// Initializes a new <see cref="ResourceKey"/>.
            /// </summary>
            /// <param name="resourceString">The assets <see cref="ResourceString"/>.</param>
            /// <param name="assetType">The assets <see cref="Type"/>.</param>
            public ResourceKey(ResourceString resourceString, Type assetType)
                : this()
            {
                Contract.Requires<ArgumentNullException>(assetType != null);

                this.ResourceString = resourceString;
                this.Type = assetType;
            }

            /// <summary>
            /// Clones the <see cref="ResourceKey"/>.
            /// </summary>
            /// <returns>The cloned object.</returns>
            public object Clone()
            {
                return new ResourceKey(this.ResourceString, this.Type);
            }

            /// <summary>
            /// Checks whether the <see cref="ResourceKey"/> equals the specified object.
            /// </summary>
            /// <param name="obj">The object to test.</param>
            /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;

                return (obj is ResourceKey) ? this.Equals((ResourceKey)obj) : false;
            }

            /// <summary>
            /// Checks whether the <see cref="ResourceKey"/> equals the specified <see cref="ResourceKey"/>.
            /// </summary>
            /// <param name="other">The <see cref="ResourceKey"/> to test.</param>
            /// <returns><c>true</c> if the <see cref="ResourceKey"/> are equal, otherwise <c>false</c>.</returns>
            public bool Equals(ResourceKey other)
            {
                return (this.ResourceString == other.ResourceString) && (this.Type == other.Type);
            }

            /// <summary>
            /// Gets the <see cref="ResourceKey"/>s hash code.
            /// </summary>
            /// <returns>The hash code.</returns>
            public override int GetHashCode()
            {
                return HashF.GetHashCode(this.ResourceString, this.Type);
            }

            /// <summary>
            /// Checks whether two <see cref="ResourceKey"/> are equal.
            /// </summary>
            /// <param name="left">The first operand.</param>
            /// <param name="right">The second operand.</param>
            /// <returns><c>true</c> if the <see cref="ResourceKey"/> are equal, otherwise <c>false</c>.</returns>
            public static bool operator ==(ResourceKey left, ResourceKey right)
            {
                return left.Equals(right);
            }

            /// <summary>
            /// Checks whether two <see cref="ResourceKey"/> are inequal.
            /// </summary>
            /// <param name="left">The first operand.</param>
            /// <param name="right">The second operand.</param>
            /// <returns><c>true</c> if the <see cref="ResourceKey"/> are inequal, otherwise <c>false</c>.</returns>
            public static bool operator !=(ResourceKey left, ResourceKey right)
            {
                return !(left == right);
            }
        }
    }
}
