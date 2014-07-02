using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [ContractClass(typeof(IGameSystemContracts))]
    public interface IGameSystem
    {
        void Initialize(Game game);
    }

    [ContractClassFor(typeof(IGameSystem))]
    abstract class IGameSystemContracts : IGameSystem
    {
        void IGameSystem.Initialize(Game game)
        {
            Contract.Requires<ArgumentNullException>(game != null);
        }
    }
}
