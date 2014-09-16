using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents an object that converts the asset stream into an object.
    /// </summary>
    /// <seealso cref="IContentManager"/>
    [ContractClass(typeof(IContentReaderContracts))]
    public interface IContentReader
    {
        /// <summary>
        /// Asynchronously converts from the specified <paramref name="assetStream"/> into a usable asset of
        /// type <paramref name="assetType"/>.
        /// </summary>
        /// <param name="contentManager">The <see cref="IContentManager"/> that triggered the loading process.</param>
        /// <param name="resourceString">The resource string of the asset to be loaded.</param>
        /// <param name="assetStream">A <see cref="Stream"/> of the asset's data.</param>
        /// <param name="assetType">The <see cref="Type"/> of asset to read.</param>
        /// <param name="parameter">A parameter the client specifies when requesting an asset.</param>
        /// <returns>
        /// The deserialized asset or <c>null</c> if an error occured or the specified <paramref name="assetType"/>
        /// cannot be read.
        /// </returns>
        Task<object> ReadAsync(IContentManager contentManager, ResourceString resourceString, Stream assetStream, Type assetType, object parameter);
    }

    [ContractClassFor(typeof(IContentReader))]
    abstract class IContentReaderContracts : IContentReader
    {
        Task<object> IContentReader.ReadAsync(IContentManager contentManager, ResourceString resourceString, Stream assetStream, Type assetType, object parameter)
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Requires<ArgumentNullException>(assetStream != null);
            Contract.Requires<ArgumentNullException>(assetType != null);
            Contract.Ensures(Contract.Result<Task<object>>() != null);

            return null;
        }
    }
}
