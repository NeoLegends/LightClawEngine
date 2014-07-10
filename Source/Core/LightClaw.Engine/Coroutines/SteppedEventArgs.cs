using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Coroutines
{
    public class SteppedEventArgs : EventArgs
    {
        public object Current { get; private set; }

        public bool Result { get; private set; }

        public SteppedEventArgs(object current, bool result)
        {
            this.Current = current;
            this.Result = result;
        }
    }
}
