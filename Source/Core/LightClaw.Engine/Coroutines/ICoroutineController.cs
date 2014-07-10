using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Coroutines
{
    [ContractClass(typeof(ICoroutineControllerContracts))]
    public interface ICoroutineController : IUpdateable
    {
        void Add(IEnumerable coroutine);

        void Add(Func<IEnumerable> coroutine);
    }

    [ContractClassFor(typeof(ICoroutineController))]
    abstract class ICoroutineControllerContracts : ICoroutineController
    {
        void ICoroutineController.Add(IEnumerable coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);
        }

        void ICoroutineController.Add(Func<IEnumerable> coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);
        }

        void IUpdateable.Update()
        {
        }
    }
}
