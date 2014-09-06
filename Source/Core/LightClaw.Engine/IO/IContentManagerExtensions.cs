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
    /// <summary>
    /// Contains extension methods to <see cref="IContentManager"/> extending the default behaviour on all instances.
    /// </summary>
    public static class IContentManagerExtensions
    {
        // We provide additional, commonly needed content manager functionality through
        // extension methods to free custom interface implementers of implementing those
        // methods.

        /// <summary>
        /// Asynchronously loads the asset with the specified resource string.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of asset to load.</typeparam>
        /// <param name="contentManager">The <see cref="IContentManager"/> to load the asset.</param>
        /// <param name="resourceString">The resource string of the asset to load.</param>
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

        /// <summary>
        /// Registers a bunch of <see cref="IContentReader"/>s in the <see cref="IContentManager"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IContentManager"/> will assume ownage and dispose the <see cref="IContentReader"/> (in case it implements IDisposable)
        /// on its disposal.
        /// </remarks>
        /// <param name="contentManager">The <see cref="IContentManager"/> to register the <paramref name="readers"/> in.</param>
        /// <param name="readers">The <see cref="IContentReader"/>s to register.</param>
        public static void Register(this IContentManager contentManager, IEnumerable<IContentReader> readers)
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(readers != null);
            
            foreach (IContentReader reader in readers.FilterNull())
            {
                contentManager.Register(reader);
            }
        }

        /// <summary>
        /// Registers a bunch of <see cref="IContentResolver"/>s in the <see cref="IContentManager"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IContentManager"/> will assume ownage and dispose the <see cref="IContentResolver"/> (in case it implements IDisposable)
        /// on its disposal.
        /// </remarks>
        /// <param name="contentManager">The <see cref="IContentManager"/> to register the <paramref name="resolvers"/> in.</param>
        /// <param name="readers">The <see cref="IContentResolver"/>s to register.</param>
        public static void Register(this IContentManager contentManager, IEnumerable<IContentResolver> resolvers)
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(resolvers != null);

            foreach (IContentResolver resolver in resolvers.FilterNull())
            {
                contentManager.Register(resolver);
            }
        }
    }
}
