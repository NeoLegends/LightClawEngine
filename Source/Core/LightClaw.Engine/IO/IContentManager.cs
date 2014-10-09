using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents an instance enabling access to the game's assets and content.
    /// </summary>
    /// <seealso cref="ContentManager"/>
    [ContractClass(typeof(IContentManagerContracts))]
    public interface IContentManager
    {
        /// <summary>
        /// Notifies about the start of an asset loading process.
        /// </summary>
        /// <seealso cref="LoadAsync"/>
        event EventHandler<ParameterEventArgs> AssetLoading;

        /// <summary>
        /// Notifies about the end of an asset loading process.
        /// </summary>
        /// <seealso cref="LoadAsync"/>
        event EventHandler<ParameterEventArgs> AssetLoaded;

        /// <summary>
        /// Notifies about the start of <see cref="GetStreamAsync"/>.
        /// </summary>
        /// <seealso cref="GetStreamAsync"/>
        event EventHandler<ParameterEventArgs> StreamObtaining;

        /// <summary>
        /// Notifies about the end of <see cref="GetStreamAsync"/>.
        /// </summary>
        /// <seealso cref="GetStreamAsync"/>
        event EventHandler<ParameterEventArgs> StreamObtained;

        /// <summary>
        /// Checks whether an asset with the specified resource string exists.
        /// </summary>
        /// <param name="resourceString">The resource string to check for.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of process.</param>
        /// <returns><c>true</c> if the asset exists, otherwise <c>false</c>.</returns>
        Task<bool> ExistsAsync(ResourceString resourceString, CancellationToken token);

        /// <summary>
        /// Gets a <u>writable</u> <see cref="Stream"/> around a specific resource string. If there is no asset with the
        /// specified resource string, it will be created.
        /// </summary>
        /// <remarks>This is the engine's main asset output (save-file, etc.) interface.</remarks>
        /// <param name="resourceString">The resource string to obtain a <see cref="Stream"/> around.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the stream obtaining process.</param>
        /// <returns>A <see cref="Stream"/> wrapping the specified asset.</returns>
        /// <seealso cref="Stream"/>
        Task<Stream> GetStreamAsync(ResourceString resourceString, CancellationToken token);

        /// <summary>
        /// Asynchronously loads the asset with the specified resource string.
        /// </summary>
        /// <param name="resourceString">The resource string of the asset to load.</param>
        /// <param name="assetType">The <see cref="Type"/> of asset to load.</param>
        /// <param name="parameter">
        /// A custom parameter that is handed to the <see cref="IContentReader"/> s to provide them with additional
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
        /// <exception cref="OperationCanceledException">The content loading operation was canceled.</exception>
        Task<object> LoadAsync(ResourceString resourceString, Type assetType, object parameter, CancellationToken token, bool forceReload);
        
        /// <summary>
        /// Registers a new <see cref="IContentReader"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IContentManager"/> will assume ownage and dispose the <see cref="IContentReader"/> (in case
        /// it implements IDisposable) on its disposal.
        /// </remarks>
        /// <param name="reader">The <see cref="IContentReader"/> to register.</param>
        /// <seealso cref="IContentReader"/>
        void Register(IContentReader reader);

        /// <summary>
        /// Registers a new <see cref="IContentResolver"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IContentManager"/> will assume ownage and dispose the <see cref="IContentResolver"/> (in case
        /// it implements IDisposable) on its disposal.
        /// </remarks>
        /// <param name="resolver">The <see cref="IContentResolver"/> to register.</param>
        /// <seealso cref="IContentResolver"/>
        void Register(IContentResolver resolver);
    }

    [ContractClassFor(typeof(IContentManager))]
    internal abstract class IContentManagerContracts : IContentManager
    {
        event EventHandler<ParameterEventArgs> IContentManager.AssetLoading { add { } remove { } }

        event EventHandler<ParameterEventArgs> IContentManager.AssetLoaded { add { } remove { } }

        event EventHandler<ParameterEventArgs> IContentManager.StreamObtaining { add { } remove { } }

        event EventHandler<ParameterEventArgs> IContentManager.StreamObtained { add { } remove { } }

        Task<bool> IContentManager.ExistsAsync(ResourceString resourceString, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Ensures(Contract.Result<Task<bool>>() != null);

            return null;
        }

        Task<Stream> IContentManager.GetStreamAsync(ResourceString resourceString, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Ensures(Contract.Result<Task<Stream>>() != null);
            Contract.Ensures(Contract.Result<Task<Stream>>().Result.CanWrite);
            Contract.Ensures(Contract.Result<Task<Stream>>().Result.CanRead);

            return null;
        }

        Task<object> IContentManager.LoadAsync(ResourceString resourceString, Type assetType, object parameter, CancellationToken token, bool forceReload)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Requires<ArgumentNullException>(assetType != null);
            Contract.Ensures(Contract.Result<Task<object>>() != null);
            Contract.Ensures(Contract.Result<Task<object>>().Result != null);

            return null;
        }

        void IContentManager.Register(IContentReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
        }

        void IContentManager.Register(IContentResolver resolver)
        {
            Contract.Requires<ArgumentNullException>(resolver != null);
        }
    }
}
