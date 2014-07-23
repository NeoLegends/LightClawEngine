using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine
{
    public class ResizeEventArgs : EventArgs
    {
        public int Height { get; private set; }

        public int Width { get; private set; }

        public ResizeEventArgs(int width, int height)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);

            this.Height = height;
            this.Width = width;
        }
    }
}
