using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    [ContractClass(typeof(IContentManagerContracts))]
    public interface IContentManager
    {
        Task<bool> ExistsAsync(string resourceString);

        void ForceReload(string resourceString);

        Task<Stream> GetStreamAsync(string resourceString);

        Task<object> LoadAsync(string resourceString, Type resourceType, object parameter = null);

        void Register(IContentReader reader);

        void Register(IContentResolver resolver);
    }

    [ContractClassFor(typeof(IContentManager))]
    abstract class IContentManagerContracts : IContentManager
    {
        Task<bool> IContentManager.ExistsAsync(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<bool>>() != null);

            return null;
        }

        void IContentManager.ForceReload(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
        }

        Task<Stream> IContentManager.GetStreamAsync(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<Stream>>() != null);

            return null;
        }

        Task<object> IContentManager.LoadAsync(string resourceString, Type assetType, object parameter)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Requires<ArgumentNullException>(assetType != null);
            Contract.Ensures(Contract.Result<Task<object>>() != null);

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
