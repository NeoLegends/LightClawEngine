using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public class ParameterEventArgs : EventArgs
    {
        public object Parameter { get; private set; }

        public ParameterEventArgs() { }

        public ParameterEventArgs(object parameter)
        {
            this.Parameter = parameter;
        }
    }
}
