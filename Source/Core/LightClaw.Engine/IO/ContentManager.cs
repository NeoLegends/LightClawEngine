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
    public class ContentManager : Entity, IContentManager
    {
        /// <summary>
        /// A list of all <see cref="IContentReader"/>s contained in the main assembly.
        /// </summary>
        private static readonly IEnumerable<IContentReader> defaultReaders = Assembly.GetExecutingAssembly()
                                                                                     .GetTypesByBase<IContentReader>(true)
                                                                                     .Select(t =>
                                                                                     {
                                                                                         try
                                                                                         {
                                                                                             return (IContentReader)Activator.CreateInstance(t);
                                                                                         }
                                                                                         catch { return null; }
                                                                                     }).FilterNull();

        /// <summary>
        /// A list of all <see cref="IContentResolver"/>s contained in the main assembly.
        /// </summary>
        private static readonly IEnumerable<IContentResolver> defaultResolvers = Assembly.GetExecutingAssembly()
                                                                                         .GetTypesByBase<IContentResolver>(true)
                                                                                         .Select(t =>
                                                                                         {
                                                                                             try
                                                                                             {
                                                                                                 return (IContentResolver)Activator.CreateInstance(t);
                                                                                             }
                                                                                             catch { return null; }
                                                                                         }).FilterNull();

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

        public event EventHandler<ParameterEventArgs> AssetLoading;

        public event EventHandler<ParameterEventArgs> AssetLoaded;

        public event EventHandler<ParameterEventArgs> ContentReaderRegistered;

        public event EventHandler<ParameterEventArgs> ContentResolverRegistered;

        public event EventHandler<ParameterEventArgs> StreamObtaining;

        public event EventHandler<ParameterEventArgs> StreamObtained;

        public ContentManager() : this(defaultReaders, defaultResolvers) { }

        public ContentManager(IEnumerable<IContentReader> readers, IEnumerable<IContentResolver> resolvers)
        {
            Contract.Requires<ArgumentNullException>(readers != null);
            Contract.Requires<ArgumentNullException>(resolvers != null);

            this.Register(readers.FilterNull());
            this.Register(resolvers.FilterNull());
        }

        public async Task<bool> ExistsAsync(string resourceString)
        {
            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, new AsyncLock()).LockAsync())
            {
                return await this.resolvers.Select(resolver => resolver.ExistsAsync(resourceString)).FirstOrDefaultAsync(t => t.Result);
            }
        }

        public async Task<Stream> GetStreamAsync(string resourceString)
        {
            Logger.Debug(() => "Obtaining stream around '{0}'.".FormatWith(resourceString));

            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, new AsyncLock()).LockAsync())
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.StreamObtaining, this.StreamObtained, resourceString, resourceString))
            {
                try
                {
                    return await this.resolvers.Select(resolver => resolver.GetStreamAsync(resourceString))
                                               .FirstAsync(t => (t.Result != null) && t.Result.CanRead && t.Result.CanWrite);
                }
                catch (InvalidOperationException ex)
                {
                    Logger.Warn(() => "No writable stream around '{0}' found.".FormatWith(resourceString));
                    throw new FileNotFoundException(
                        "No writable stream was found. If reading is required only, consider registering an IContentReader.",
                        ex
                    );
                }
            }
        }

        public async Task<object> LoadAsync(string resourceString, Type assetType, object parameter = null, bool forceReload = false)
        {
            Logger.Debug(() => "Loading an asset of type '{0}' from resource '{0}'.".FormatWith(assetType.AssemblyQualifiedName, resourceString));

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
                    Logger.Debug(() => "No cached version of '{0}' available or reload forced, obtaining stream...".FormatWith(resourceString));
                    try
                    {
                        using (Stream assetStream = await this.resolvers.Select(resolver => resolver.GetStreamAsync(resourceString))
                                                                        .FirstOrDefaultAsync(t => (t.Result != null) && t.Result.CanRead))
                        {
                            if (assetStream == null)
                            {
                                string message = "Asset '{0}' could not be found.".FormatWith(resourceString);
                                Logger.Warn(() => message);
                                throw new FileNotFoundException(message);
                            }
                            Logger.Debug(() => "Stream around '{0}' obtained, deserializing...".FormatWith(resourceString));

                            asset = await this.readers.Select(reader => reader.ReadAsync(this, resourceString, assetStream, assetType, parameter))
                                                      .FirstOrDefaultAsync(t => t.Result != null);
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
                        // Although it shouldn't, Stream might throw ODE when disposed two times -> catch that.
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

        public void Register(IContentReader reader)
        {
            this.readers.Add(reader);
            this.Raise(this.ContentReaderRegistered, reader);
        }

        public void Register(IContentResolver resolver)
        {
            this.resolvers.Add(resolver);
            this.Raise(this.ContentResolverRegistered, resolver);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.readers.All(reader => reader != null));
            Contract.Invariant(this.resolvers.All(resolver => resolver != null));
        }
    }
}
