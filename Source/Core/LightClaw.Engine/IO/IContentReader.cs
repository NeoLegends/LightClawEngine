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
        bool CanRead(Type assetType, object parameter);

        Task<object> ReadAsync(string resourceString, Stream assetStream, Type assetType, object parameter);
    }

    [ContractClassFor(typeof(IContentReader))]
    abstract class IContentReaderContracts : IContentReader
    {
        bool IContentReader.CanRead(Type assetType, object parameter)
        {
            Contract.Requires<ArgumentNullException>(assetType != null);

            return false;
        }

        Task<object> IContentReader.ReadAsync(string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Requires<ArgumentNullException>(assetStream != null);
            Contract.Requires<ArgumentNullException>(assetType != null);
            Contract.Ensures(Contract.Result<Task<object>>() != null);

            return null;
        }
    }
}
