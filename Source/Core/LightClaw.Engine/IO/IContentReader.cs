using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents an object that converts the asset stream into an object.
    /// </summary>
    /// <remarks>
    /// <see cref="IContentReader"/>s are supposed to be cheap objects that are easy to create. Preferrably, they also have a default
    /// constructor because that is what the <see cref="ContentManager"/> that ships with LightClaw needs to create an <see cref="IContentReader"/>
    /// at runtime on demand.
    /// 
    /// Also, they have to be thread-safe in respect to deserializing different objects (different resource strings) at once.
    /// </remarks>
    /// <seealso cref="IContentManager"/>
    [ContractClass(typeof(IContentReaderContracts))]
    public interface IContentReader
    {
        /// <summary>
        /// Checks whether the <see cref="IContentReader"/> can read assets of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="assetType">The type of the asset that is about to be read.</param>
        /// <returns><c>true</c> if the <see cref="IContentReader"/> can read assets of the specified <see cref="Type"/>, otherwise <c>false</c>.</returns>
        bool CanRead(Type assetType);

        /// <summary>
        /// Asynchronously converts from the specified <paramref name="assetStream"/> into a usable asset of
        /// type <paramref name="assetType"/>.
        /// </summary>
        /// <param name="parameters"><see cref="ContentReadParameters"/> containing information about the asset to be loaded.</param>
        /// <returns>
        /// The deserialized asset or <c>null</c> if an error occured or the specified type of asset cannot be read.
        /// </returns>
        Task<object> ReadAsync(ContentReadParameters parameters);
    }

    [ContractClassFor(typeof(IContentReader))]
    abstract class IContentReaderContracts : IContentReader
    {
        bool IContentReader.CanRead(Type assetType)
        {
            Contract.Requires<ArgumentNullException>(assetType != null);

            return default(bool);
        }

        Task<object> IContentReader.ReadAsync(ContentReadParameters parameters)
        {
            Contract.Requires<ArgumentNullException>(parameters.ContentManager != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(parameters.ResourceString));
            Contract.Requires<ArgumentNullException>(parameters.AssetStream != null);
            Contract.Requires<ArgumentNullException>(parameters.AssetType != null);
            Contract.Requires<ArgumentException>(parameters.AssetStream.CanRead);
            Contract.Ensures(Contract.Result<Task<object>>() != null);

            return null;
        }
    }
}
