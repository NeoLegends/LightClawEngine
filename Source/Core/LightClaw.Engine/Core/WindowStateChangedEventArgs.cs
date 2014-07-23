using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LightClaw.Engine
{
    public class WindowStateChangedEventArgs : EventArgs
    {
        public WindowState WindowState { get; private set; }

        public WindowStateChangedEventArgs(WindowState windowState)
        {
            this.WindowState = windowState;
        }
    }
}
