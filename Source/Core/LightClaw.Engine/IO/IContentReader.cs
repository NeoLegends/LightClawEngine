using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    [ContractClass(typeof(IContentReaderContracts))]
    public interface IContentReader
    {
        Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter);
    }

    [ContractClassFor(typeof(IContentReader))]
    abstract class IContentReaderContracts : IContentReader
    {
        Task<object> IContentReader.ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
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
