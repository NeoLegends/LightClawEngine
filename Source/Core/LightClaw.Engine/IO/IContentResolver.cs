using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    [ContractClass(typeof(IContentResolverContracts))]
    public interface IContentResolver
    {
        Task<bool> ExistsAsync(string resourceString);

        Task<Stream> GetStreamAsync(string resourceString);
    }

    [ContractClassFor(typeof(IContentResolver))]
    abstract class IContentResolverContracts : IContentResolver
    {
        Task<bool> IContentResolver.ExistsAsync(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<bool>>() != null);

            return null;
        }

        Task<Stream> IContentResolver.GetStreamAsync(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<Stream>>() != null);

            return null;
        }
    }
}
