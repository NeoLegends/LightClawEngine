using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;

namespace LightClaw.Engine.Core
{
    internal class GameCodeInterface : IGameCodeInterface
    {
        public Assembly GameCodeAssembly { get; private set; }

        public GameCodeInterface(Assembly gameCodeAssembly)
        {
            Contract.Requires<ArgumentNullException>(gameCodeAssembly != null);

            this.GameCodeAssembly = gameCodeAssembly;
        }
    
        public IEnumerable<Type> GetComponents()
        {
            return this.GameCodeAssembly.GetTypesByAttribute<GameComponentAttribute>();
        }
    }
}
