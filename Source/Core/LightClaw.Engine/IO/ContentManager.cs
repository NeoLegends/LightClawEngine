using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    public class ContentManager : IContentManager
    {
        private static readonly IContentReader[] defaultReaders = new IContentReader[]
        {
            new StringContentReader(), new SceneReader()
        };

        private static readonly IContentResolver[] defaultResolvers = new IContentResolver[]
        {
            new DiskContentResolver(AppDomain.CurrentDomain.BaseDirectory)
        };

        private readonly ConcurrentDictionary<string, AsyncLock> assetLocks = new ConcurrentDictionary<string, AsyncLock>();

        private readonly ConcurrentDictionary<string, WeakReference> cachedAssets = new ConcurrentDictionary<string, WeakReference>();

        private readonly ConcurrentBag<IContentReader> readers = new ConcurrentBag<IContentReader>();

        private readonly ConcurrentBag<IContentResolver> resolvers = new ConcurrentBag<IContentResolver>();

        public ContentManager() : this(defaultReaders, defaultResolvers) { }

        public ContentManager(IEnumerable<IContentReader> readers, IEnumerable<IContentResolver> resolvers)
        {
            Contract.Requires<ArgumentNullException>(readers != null);
            Contract.Requires<ArgumentNullException>(resolvers != null);
            Contract.Requires<ArgumentException>(readers.All(reader => reader != null));
            Contract.Requires<ArgumentException>(resolvers.All(resolver => resolver != null));

            this.Register(readers);
            this.Register(resolvers);
        }

        public async Task<bool> ExistsAsync(string resourceString)
        {
            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, new AsyncLock()).LockAsync())
            {
                foreach (Task<bool> task in this.resolvers.Select(resolver => resolver.ExistsAsync(resourceString)).ToArray())
                {
                    if (await task)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void ForceReload(string resourceString)
        {
            WeakReference weakRef;
            this.cachedAssets.TryRemove(resourceString, out weakRef);
        }

        public async Task<Stream> GetStreamAsync(string resourceString)
        {
            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, new AsyncLock()).LockAsync())
            {
                foreach (Task<Stream> task in this.resolvers.Select(resolver => resolver.GetStreamAsync(resourceString)).ToArray())
                {
                    Stream result = await task;
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
        }

        public async Task<T> LoadAsync<T>(string resourceString, object parameter = null)
        {
            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, new AsyncLock()).LockAsync())
            {
                WeakReference cachedAsset = null;
                object asset = null;

                if (!this.cachedAssets.TryGetValue(resourceString, out cachedAsset) || 
                    (asset = cachedAsset.Target) == null ||
                    !(asset is T)) // Load new if asset is not cached, WeakRef was collected or cached asset is not of requested type
                {
                    using (Stream assetStream = await this.GetStreamAsync(resourceString))
                    {
                        if (assetStream == null)
                        {
                            throw new FileNotFoundException("The asset could not be found.");
                        }

                        IEnumerable<IContentReader> contentReaders = this.readers.Where(reader => reader.CanRead(typeof(T), parameter));
                        if (!contentReaders.Any())
                        {
                            throw new InvalidOperationException("There was no suitable IContentReader to read the asset found.");
                        }
                        foreach (IContentReader reader in contentReaders)
                        {
                            asset = await reader.ReadAsync(resourceString, assetStream, typeof(T), parameter);
                            if (asset != null)
                            {
                                break;
                            }
                        }
                        if (asset == null)
                        {
                            throw new InvalidOperationException("The asset could not be deserialized from the stream.");
                        }

                        cachedAsset = new WeakReference(asset);
                        this.cachedAssets.AddOrUpdate(resourceString, cachedAsset, (key, oldValue) => cachedAsset);
                    }
                }

                return (T)asset;
            }
        }

        public void Register(IContentReader reader)
        {
            this.readers.Add(reader);
        }

        public void Register(IContentResolver resolver)
        {
            this.resolvers.Add(resolver);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.readers.All(reader => readers != null));
            Contract.Invariant(this.resolvers.All(resolver => readers != null));
        }
    }
}
