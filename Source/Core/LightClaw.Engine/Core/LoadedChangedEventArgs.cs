using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public class LoadedChangedEventArgs : ControllableEventArgs
    {
        public bool IsLoaded { get; private set; }

        public LoadedChangedEventArgs(bool isLoaded) : this(isLoaded, null) { }

        public LoadedChangedEventArgs(bool isLoaded, object parameter)
            : base(parameter)
        {
            this.IsLoaded = isLoaded;
        }
    }
}
