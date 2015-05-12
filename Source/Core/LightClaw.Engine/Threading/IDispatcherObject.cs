using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Threading
{
    /// <summary>
    /// Represents an object that is associated with a <see cref="Dispatcher"/>.
    /// </summary>
    [ContractClass(typeof(IDispatcherObjectContracts))]
    public interface IDispatcherObject
    {
        /// <summary>
        /// Gets the <see cref="Dispatcher"/> associated with the <see cref="IDispatcherObject"/>.
        /// </summary>
        Dispatcher Dispatcher { get; }
    }

    [ContractClassFor(typeof(IDispatcherObject))]
    abstract class IDispatcherObjectContracts : IDispatcherObject
    {
        public Dispatcher Dispatcher
        {
            get 
            {
                Contract.Ensures(Contract.Result<Dispatcher>() != null);

                return null;
            }
        }
    }
}
