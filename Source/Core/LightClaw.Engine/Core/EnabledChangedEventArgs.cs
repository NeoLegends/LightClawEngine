using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public class EnabledChangedEventArgs : ControllableEventArgs
    {
        public bool IsEnabled { get; private set; }

        public EnabledChangedEventArgs(bool isEnabled) : this(isEnabled, null) { }

        public EnabledChangedEventArgs(bool isEnabled, object parameter)
            : base(parameter)
        {
            this.IsEnabled = isEnabled;
        }
    }
}
