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

namespace LightClaw.Engine.IO
{
    public class ContentManager : IContentManager
    {
        private static readonly IContentReader[] defaultReaders = Assembly.GetExecutingAssembly()
                                                                          .GetTypesByBase<IContentReader>(true)
                                                                          .Select(t =>
                                                                          {
                                                                              try
                                                                              {
                                                                                  return (IContentReader)Activator.CreateInstance(t);
                                                                              }
                                                                              catch { return null; }
                                                                          }).FilterNull().ToArray();

        private static readonly IContentResolver[] defaultResolvers = Assembly.GetExecutingAssembly()
                                                                              .GetTypesByBase<IContentResolver>(true)
                                                                              .Select(t =>
                                                                              {
                                                                                  try
                                                                                  {
                                                                                      return (IContentResolver)Activator.CreateInstance(t);
                                                                                  }
                                                                                  catch { return null; }
                                                                              }).FilterNull().ToArray();

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
                return await this.resolvers.Select(resolver => resolver.ExistsAsync(resourceString)).FirstOrDefaultAsync(t => t.Result);
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
                return await this.GetStreamInternal(resourceString);
            }
        }

        public async Task<object> LoadAsync(string resourceString, Type assetType, object parameter = null)
        {
            using (var releaser = await this.assetLocks.GetOrAdd(resourceString, new AsyncLock()).LockAsync())
            {
                WeakReference cachedAsset = null;
                object asset = null;

                if (!this.cachedAssets.TryGetValue(resourceString, out cachedAsset) || 
                    (asset = cachedAsset.Target) == null ||
                    !(assetType.IsAssignableFrom(asset.GetType()))) // Load new if asset is not cached, WeakRef was collected or cached asset is not of requested type
                {
                    try
                    {
                        using (Stream assetStream = await this.GetStreamInternal(resourceString))
                        {
                            if (assetStream == null)
                            {
                                throw new FileNotFoundException("Asset '{0}' could not be found.".FormatWith(resourceString));
                            }

                            asset = await this.readers
                                              .Select(reader => reader.ReadAsync(this, resourceString, assetStream, assetType, parameter))
                                              .FirstOrDefaultAsync(t => t.Result != null);
                            if (asset == null)
                            {
                                throw new InvalidOperationException("Asset '{0}' could not be deserialized from the stream.".FormatWith(resourceString));
                            }

                            cachedAsset = new WeakReference(asset);
                            this.cachedAssets.AddOrUpdate(resourceString, cachedAsset, (key, oldValue) => cachedAsset);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // Although it shouldn't, Stream might throw ODE when disposed two times -> catch that.
                    }
                }

                return asset;
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

        private Task<Stream> GetStreamInternal(string resourceString) // Version w/o locking to avoid deadlocks when LoadAsync calls GetStream
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);

            return this.resolvers.Select(resolver => resolver.GetStreamAsync(resourceString)).FirstOrDefaultAsync(t => t.Result != null);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.readers.All(reader => readers != null));
            Contract.Invariant(this.resolvers.All(resolver => readers != null));
        }
    }
}
