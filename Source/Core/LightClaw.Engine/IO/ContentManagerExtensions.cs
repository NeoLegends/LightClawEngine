using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    public static class ContentManagerExtensions
    {
        public static async Task<T> LoadAsync<T>(
                this IContentManager contentManager, 
                string resourceString, 
                object parameter = null, 
                bool forceReload = false
            )
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<T>>() != null);

            return (T)await contentManager.LoadAsync(resourceString, typeof(T), parameter, forceReload);
        }

        public static void Register(this IContentManager manager, IEnumerable<IContentReader> readers)
        {
            Contract.Requires<ArgumentNullException>(manager != null);
            Contract.Requires<ArgumentNullException>(readers != null);

            foreach (IContentReader reader in readers.FilterNull())
            {
                manager.Register(reader);
            }
        }

        public static void Register(this IContentManager manager, IEnumerable<IContentResolver> resolvers)
        {
            Contract.Requires<ArgumentNullException>(manager != null);
            Contract.Requires<ArgumentNullException>(resolvers != null);

            foreach (IContentResolver resolver in resolvers.FilterNull())
            {
                manager.Register(resolver);
            }
        }
    }
}
