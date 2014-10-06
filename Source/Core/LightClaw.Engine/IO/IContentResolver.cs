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
    /// Represents an object that obtains <see cref="Stream"/> s around specified resource strings.
    /// </summary>
    /// <seealso cref="IContentManager"></seealso>
    [ContractClass(typeof(IContentResolverContracts))]
    public interface IContentResolver
    {
        /// <summary>
        /// Checks whether the asset with the specified resource string exists.
        /// </summary>
        /// <param name="resourceString">The resource string of the asset to check for.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the process.</param>
        /// <returns><c>true</c> if the asset exists, otherwise <c>false</c> .</returns>
        Task<bool> ExistsAsync(ResourceString resourceString, CancellationToken token);

        /// <summary>
        /// Gets a <see cref="Stream"/> around the specified asset.
        /// </summary>
        /// <param name="resourceString">The resource string of the asset to obtain a <see cref="Stream"/> around.</param>
        /// <param name="writable">Indicates whether the <see cref="Stream"/> has to be writable.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the content resolving process.</param>
        /// <returns>The <see cref="Stream"/> around the asset or <c>null</c> if the asset could not be found.</returns>
        Task<Stream> GetStreamAsync(ResourceString resourceString, bool writable, CancellationToken token);
    }

    [ContractClassFor(typeof(IContentResolver))]
    internal abstract class IContentResolverContracts : IContentResolver
    {
        Task<bool> IContentResolver.ExistsAsync(ResourceString resourceString, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Ensures(Contract.Result<Task<bool>>() != null);

            return null;
        }

        Task<Stream> IContentResolver.GetStreamAsync(ResourceString resourceString, bool writable, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Ensures(Contract.Result<Task<Stream>>() != null);
            Contract.Ensures(
                Contract.Result<Task<Stream>>().Result == null ||
                (Contract.Result<Task<Stream>>().Result.CanRead && (!writable || Contract.Result<Task<Stream>>().Result.CanWrite))
            );

            return null;
        }
    }
}
