using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using log4net;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents a caching <see cref="IContentManager"/>.
    /// </summary>
    public class ContentManager : DisposableEntity, IContentManager
    {
        /// <summary>
        /// Contains <see cref="AsyncLock"/>s used to lock access to a specific asset while it is being loaded.
        /// </summary>
        private readonly ConcurrentDictionary<string, AsyncLock> assetLocks = new ConcurrentDictionary<string, AsyncLock>();

        /// <summary>
        /// Represents the asset cache. Assets are cached using weak references to reduce memory pressure.
        /// </summary>
        private readonly ConcurrentDictionary<string, WeakReference<object>> cachedAssets = new ConcurrentDictionary<string, WeakReference<object>>();

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
        /// Occurs when a new <see cref="IContentResolver"/> was registered.
        /// </summary>
        public event EventHandler<ParameterEventArgs> ContentReaderRegistered;

        /// <summary>
        /// Occurs when a new <see cref="IContentResolver"/> was registered.
        /// </summary>
        public event EventHandler<ParameterEventArgs> ContentResolverRegistered;

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
        /// Initializes a new <see cref="ContentManager"/> using the default <see cref="IContentReader"/>s and <see cref="IContentResolver"/>s.
        /// </summary>
        public ContentManager() : this(GetDefaultReaders(), GetDefaultResolvers()) { }

        /// <summary>
        /// Initializes a new <see cref="ContentManager"/> from the specified <see cref="IContentReader"/>s and <see cref="IContentResolver"/>s.
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
        /// <param name="resourceString">The resource string to check for.</param>
        /// <returns><c>true</c> if the asset exists, otherwise <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(ResourceString resourceString)
        {
            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, new AsyncLock()).LockAsync())
            {
                return await this.resolvers.Select(resolver => resolver.ExistsAsync(resourceString)).AnyAsync(b => b);
            }
        }

        /// <summary>
        /// Gets a <u>writable</u> <see cref="Stream"/> around a specific resource string. If there is no
        /// asset with the specified resource string, it will be created.
        /// </summary>
        /// <remarks>This is the engine's main asset output (save-file, etc.) interface.</remarks>
        /// <param name="resourceString">The resource string to obtain a <see cref="Stream"/> around.</param>
        /// <returns>A <see cref="Stream"/> wrapping the specified asset.</returns>
        /// <seealso cref="Stream"/>
        public async Task<Stream> GetStreamAsync(ResourceString resourceString)
        {
            Logger.Debug(() => "Obtaining stream around '{0}'.".FormatWith(resourceString));

            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, new AsyncLock()).LockAsync())
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.StreamObtaining, this.StreamObtained, resourceString, resourceString))
            {
                try
                {
                    return await this.resolvers.Select(resolver => resolver.GetStreamAsync(resourceString))
                                               .FirstAsync(s => (s != null) && s.CanRead && s.CanWrite);
                }
                catch (InvalidOperationException ex)
                {
                    Logger.Warn(() => "No writable stream around '{0}' found.".FormatWith(resourceString));
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
        /// Imagine a content reader reading texture files to a generic texture class. It needs information about
        /// the file type of the image to load to be able to properly load it.
        /// </example>
        /// </param>
        /// <param name="forceReload">Indicates whether to force-load the asset from the disk and bypass any caching structures.</param>
        /// <returns>The loaded asset.</returns>
        /// <exception cref="FileNotFoundException">The asset could not be found.</exception>
        /// <exception cref="InvalidOperationException">The asset could not be deserialized from the stream.</exception>
        public async Task<object> LoadAsync(ResourceString resourceString, Type assetType, object parameter = null, bool forceReload = false)
        {
            Logger.Debug(() => "Loading an asset of type '{0}' from resource '{1}'.".FormatWith(assetType.AssemblyQualifiedName, resourceString));

            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, new AsyncLock()).LockAsync())
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.AssetLoading, this.AssetLoaded, resourceString, resourceString))
            {
                WeakReference<object> cachedAsset = null;
                object asset = null;

                if (forceReload ||                                                     // Load if reload forced,
                    !this.cachedAssets.TryGetValue(resourceString, out cachedAsset) || // no cache available,
                    !cachedAsset.TryGetTarget(out asset) ||                            // weak reference to cached asset collected or
                    !(assetType.IsAssignableFrom(asset.GetType())))                    // types mismatch.
                {
                    Logger.Debug(
                        () => ((forceReload ? "No cached version of '{0}' available" : "Reload of '{0}' forced") + ", obtaining stream...").FormatWith(resourceString)
                    );
                    try
                    {
                        using (Stream assetStream = await this.resolvers.Select(resolver => resolver.GetStreamAsync(resourceString))
                                                                        .FirstOrDefaultAsync(s => (s != null) && s.CanRead))
                        {
                            if (assetStream == null)
                            {
                                string message = "Asset '{0}' could not be found.".FormatWith(resourceString);
                                Logger.Warn(() => message);
                                throw new FileNotFoundException(message);
                            }
                            Logger.Debug(() => "Stream around '{0}' obtained, deserializing...".FormatWith(resourceString));

                            asset = await this.readers.Select(reader => reader.ReadAsync(this, resourceString, assetStream, assetType, parameter))
                                                      .FirstOrDefaultAsync(reader => reader != null);
                            if (asset == null)
                            {
                                string message = "Asset '{0}' could not be deserialized.".FormatWith(assetStream);
                                Logger.Warn(() => message);
                                throw new InvalidOperationException(message);
                            }

                            cachedAsset = new WeakReference<object>(asset);
                            this.cachedAssets.AddOrUpdate(resourceString, cachedAsset, (key, oldValue) => cachedAsset);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // Although it shouldn't, Stream might throw ODE when disposed multiple times (i.e. by a StreamReader) -> catch that.
                    }
                }
                else
                {
                    Logger.Debug(() => "Cached version of '{0}' available, loading that instead.".FormatWith(resourceString));
                }

                Logger.Debug(() => "Asset '{0}' loaded successfully.".FormatWith(resourceString));
                return asset;
            }
        }

        /// <summary>
        /// Registers a new <see cref="IContentReader"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="ContentManager"/> will assume ownage and dispose the <see cref="IContentReader"/> (in case it implements IDisposable)
        /// on its disposal.
        /// </remarks>
        /// <param name="reader">The <see cref="IContentReader"/> to register.</param>
        /// <seealso cref="IContentReader"/>
        public void Register(IContentReader reader)
        {
            this.readers.Add(reader);
            this.Raise(this.ContentReaderRegistered, reader);
        }

        /// <summary>
        /// Registers a new <see cref="IContentResolver"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="ContentManager"/> will assume ownage and dispose the <see cref="IContentResolver"/> (in case it implements IDisposable)
        /// on its disposal.
        /// </remarks>
        /// <param name="resolver">The <see cref="IContentResolver"/> to register.</param>
        /// <seealso cref="IContentResolver"/>
        public void Register(IContentResolver resolver)
        {
            this.resolvers.Add(resolver);
            this.Raise(this.ContentResolverRegistered, resolver);
        }

        /// <summary>
        /// Disposes the <see cref="IContentManager"/> disposing of all of the <see cref="IContentReader"/>s
        /// and <see cref="IContentResolver"/>s.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose of managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
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

                IContentReader reader;
                while (this.readers.TryTake(out reader))
                {
                    IDisposable disposableReader = reader as IDisposable;
                    if (disposableReader != null)
                    {
                        disposableReader.Dispose();
                    }
                }

                base.Dispose(disposing);
            }
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
        /// Gets all default <see cref="IContentReader"/>s.
        /// </summary>
        /// <returns>All default <see cref="IContentReader"/>s.</returns>
        private static IEnumerable<IContentReader> GetDefaultReaders()
        {
            Contract.Ensures(Contract.Result<IEnumerable<IContentReader>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<IContentReader>>().All(reader => reader != null));

            return Assembly.GetExecutingAssembly()
                           .GetTypesByBase<IContentReader>(true)
                           .Where(t => !t.IsAbstract)
                           .Select(t =>
                           {
                               try
                               {
                                   return (IContentReader)Activator.CreateInstance(t);
                               }
                               catch { return null; }
                           }).FilterNull();
        }

        /// <summary>
        /// Gets all default <see cref="IContentResolver"/>s.
        /// </summary>
        /// <returns>All default <see cref="IContentResolver"/>s.</returns>
        private static IEnumerable<IContentResolver> GetDefaultResolvers()
        {
            Contract.Ensures(Contract.Result<IEnumerable<IContentResolver>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<IContentResolver>>().All(resolver => resolver != null));

            return Assembly.GetExecutingAssembly()
                           .GetTypesByBase<IContentResolver>(true)
                           .Where(t => !t.IsAbstract)
                           .Select(t =>
                           {
                               try
                               {
                                   return (IContentResolver)Activator.CreateInstance(t);
                               }
                               catch { return null; }
                           }).FilterNull();
        }
    }
}
