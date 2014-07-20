using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public class ControllableEventArgs : EventArgs
    {
        public object State { get; private set; }

        public ControllableEventArgs() { }

        public ControllableEventArgs(object state)
        {
            this.State = state;
        }
    }
}
