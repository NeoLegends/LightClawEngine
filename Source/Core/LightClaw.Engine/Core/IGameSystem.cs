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
        void Register(IGame game);
    }
    
    [ContractClassFor(typeof(IGame))]
    abstract class IGameSystemContracts : IGameSystem
    {
        void IGameSystem.Register(IGame game)
        {
            Contract.Requires<ArgumentNullException>(game != null);
        }
    }
}
