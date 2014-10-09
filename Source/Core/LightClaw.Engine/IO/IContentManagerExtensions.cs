using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Contains extension methods to <see cref="IContentManager"/> extending the default behaviour on all instances.
    /// </summary>
    public static class IContentManagerExtensions
    {
        // We provide additional, commonly needed content manager functionality through extension methods to free custom
        // interface implementers of implementing those methods.

        /// <summary>
        /// Checks whether an asset with the specified resource string exists.
        /// </summary>
        /// <param name="resourceString">The resource string to check for.</param>
        /// <returns><c>true</c> if the asset exists, otherwise <c>false</c>.</returns>
        public static Task<bool> ExistsAsync(this IContentManager manager, ResourceString resourceString)
        {
            Contract.Requires<ArgumentNullException>(manager != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(resourceString));

            return manager.ExistsAsync(resourceString, CancellationToken.None);
        }

        /// <summary>
        /// Gets a <u>writable</u> <see cref="Stream"/> around a specific resource string. If there is no asset with the
        /// specified resource string, it will be created.
        /// </summary>
        /// <remarks>This is the engine's main asset output (save-file, etc.) interface.</remarks>
        /// <param name="resourceString">The resource string to obtain a <see cref="Stream"/> around.</param>
        /// <returns>A <see cref="Stream"/> wrapping the specified asset.</returns>
        /// <seealso cref="Stream"/>
        public static Task<Stream> GetStreamAsync(this IContentManager manager, ResourceString resourceString)
        {
            Contract.Requires<ArgumentNullException>(manager != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(resourceString));

            return manager.GetStreamAsync(resourceString, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously loads the asset with the specified resource string.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of asset to load.</typeparam>
        /// <param name="contentManager">The <see cref="IContentManager"/> to load the asset.</param>
        /// <param name="resourceString">The resource string of the asset to load.</param>
        /// <param name="parameter">
        /// A custom parameter that is handed to the <see cref="IContentReader"/> s to provide them with additional
        /// information about the asset being read. 
        /// <example> 
        /// Imagine a content reader reading texture files to a generic texture class. It needs information about 
        /// the file type of the image to load to be able to properly load it.
        /// </example>
        /// </param>
        /// <returns>The loaded asset.</returns>
        /// <exception cref="FileNotFoundException">The asset could not be found.</exception>
        /// <exception cref="InvalidOperationException">The asset could not be deserialized from the stream.</exception>
        public static Task<T> LoadAsync<T>(this IContentManager contentManager, ResourceString resourceString)
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<T>>() != null);

            return LoadAsync<T>(contentManager, resourceString, null, CancellationToken.None, false);
        }

        /// <summary>
        /// Asynchronously loads the asset with the specified resource string.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of asset to load.</typeparam>
        /// <param name="contentManager">The <see cref="IContentManager"/> to load the asset.</param>
        /// <param name="resourceString">The resource string of the asset to load.</param>
        /// <param name="parameter">
        /// A custom parameter that is handed to the <see cref="IContentReader"/> s to provide them with additional
        /// information about the asset being read. 
        /// <example> 
        /// Imagine a content reader reading texture files to a
        /// generic texture class. It needs information about the file type of the image to load to be able to properly
        /// load it.
        /// </example>
        /// </param>
        /// <returns>The loaded asset.</returns>
        /// <exception cref="FileNotFoundException">The asset could not be found.</exception>
        /// <exception cref="InvalidOperationException">The asset could not be deserialized from the stream.</exception>
        public static Task<T> LoadAsync<T>(
                this IContentManager contentManager,
                ResourceString resourceString,
                object parameter
            )
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<T>>() != null);

            return LoadAsync<T>(contentManager, resourceString, parameter, CancellationToken.None, false);
        }

        /// <summary>
        /// Asynchronously loads the asset with the specified resource string.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of asset to load.</typeparam>
        /// <param name="contentManager">The <see cref="IContentManager"/> to load the asset.</param>
        /// <param name="resourceString">The resource string of the asset to load.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to indicate cancellation of the content loading process.</param>
        /// <returns>The loaded asset.</returns>
        /// <exception cref="FileNotFoundException">The asset could not be found.</exception>
        /// <exception cref="InvalidOperationException">The asset could not be deserialized from the stream.</exception>
        public static Task<T> LoadAsync<T>(
                this IContentManager contentManager,
                ResourceString resourceString,
                CancellationToken token
            )
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<T>>() != null);

            return LoadAsync<T>(contentManager, resourceString, null, token, false);
        }

        /// <summary>
        /// Asynchronously loads the asset with the specified resource string.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of asset to load.</typeparam>
        /// <param name="contentManager">The <see cref="IContentManager"/> to load the asset.</param>
        /// <param name="resourceString">The resource string of the asset to load.</param>
        /// <param name="parameter">
        /// A custom parameter that is handed to the <see cref="IContentReader"/> s to provide them with additional
        /// information about the asset being read. 
        /// <example> 
        /// Imagine a content reader reading texture files to a
        /// generic texture class. It needs information about the file type of the image to load to be able to properly
        /// load it.
        /// </example>
        /// </param>
        /// <param name="token">A <see cref="CancellationToken"/> used to indicate cancellation of the content loading process.</param>
        /// <returns>The loaded asset.</returns>
        /// <exception cref="FileNotFoundException">The asset could not be found.</exception>
        /// <exception cref="InvalidOperationException">The asset could not be deserialized from the stream.</exception>
        public static Task<T> LoadAsync<T>(
                this IContentManager contentManager,
                ResourceString resourceString,
                object parameter,
                CancellationToken token
            )
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<T>>() != null);

            return LoadAsync<T>(contentManager, resourceString, parameter, token, false);
        }

        /// <summary>
        /// Asynchronously loads the asset with the specified resource string.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of asset to load.</typeparam>
        /// <param name="contentManager">The <see cref="IContentManager"/> to load the asset.</param>
        /// <param name="resourceString">The resource string of the asset to load.</param>
        /// <param name="parameter">
        /// A custom parameter that is handed to the <see cref="IContentReader"/> s to provide them with additional
        /// information about the asset being read. 
        /// <example> 
        /// Imagine a content reader reading texture files to a
        /// generic texture class. It needs information about the file type of the image to load to be able to properly
        /// load it.
        /// </example>
        /// </param>
        /// <param name="token">A <see cref="CancellationToken"/> used to indicate cancellation of the content loading process.</param>
        /// <param name="forceReload">
        /// Indicates whether to force-load the asset from the disk and bypass any caching structures.
        /// </param>
        /// <returns>The loaded asset.</returns>
        /// <exception cref="FileNotFoundException">The asset could not be found.</exception>
        /// <exception cref="InvalidOperationException">The asset could not be deserialized from the stream.</exception>
        public static async Task<T> LoadAsync<T>(
                this IContentManager contentManager,
                ResourceString resourceString,
                object parameter,
                CancellationToken token,
                bool forceReload
            )
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<T>>() != null);

            return (T)await contentManager.LoadAsync(resourceString, typeof(T), parameter, token, forceReload);
        }

        /// <summary>
        /// Registers a bunch of <see cref="IContentReader"/> s in the <see cref="IContentManager"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IContentManager"/> will assume ownage and dispose the <see cref="IContentReader"/> (in case
        /// it implements IDisposable) on its disposal.
        /// </remarks>
        /// <param name="contentManager">
        /// The <see cref="IContentManager"/> to register the <paramref name="readers"/> in.
        /// </param>
        /// <param name="readers">The <see cref="IContentReader"/> s to register.</param>
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
        /// Registers a bunch of <see cref="IContentResolver"/> s in the <see cref="IContentManager"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IContentManager"/> will assume ownage and dispose the <see cref="IContentResolver"/> (in case
        /// it implements IDisposable) on its disposal.
        /// </remarks>
        /// <param name="contentManager">
        /// The <see cref="IContentManager"/> to register the <paramref name="resolvers"/> in.
        /// </param>
        /// <param name="readers">The <see cref="IContentResolver"/> s to register.</param>
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
