using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class RangedBufferDisposedEventArgs : EventArgs
    {
        public BufferRange Range { get; private set; }

        public BufferRangeTarget RangeTarget { get; private set; }

        public RangedBufferDisposedEventArgs(BufferRange range, BufferRangeTarget rangeTarget)
        {
            this.Range = range;
            this.RangeTarget = rangeTarget;
        }
    }
}
