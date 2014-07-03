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

        Task<T> LoadAsync<T>(string resourceString, object parameter = null);

        void Register(Type type, IContentReader reader);

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

        Task<T> IContentManager.LoadAsync<T>(string resourceString, object parameter)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Ensures(Contract.Result<Task<T>>() != null);

            return null;
        }

        void IContentManager.Register(Type type, IContentReader reader)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            Contract.Requires<ArgumentNullException>(reader != null);
        }

        void IContentManager.Register(IContentResolver resolver)
        {
            Contract.Requires<ArgumentNullException>(resolver != null);
        }
    }
}
