using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public interface IDisposer : IDisposable
    {
        void Register(object instance);

        void Unregister(object instance);

        void UnregisterAll();
    }
}
