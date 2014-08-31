using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL4;

using LCBuffer = LightClaw.Engine.Graphics.OpenGL.Buffer;

namespace LightClaw.Engine.Graphics
{
    public class UniformBufferManager : DisposableEntity
    {
        private readonly bool[] allocatedBufferBindings = new bool[GL.GetInteger(GetPName.MaxUniformBufferBindings)];

        private readonly IBuffer buffer = new LCBuffer(BufferTarget.UniformBuffer, BufferUsageHint.DynamicDraw);

        public UniformBufferManager() { }

        public RangedBuffer GetBuffer(int size)
        {
            Contract.Requires<ArgumentOutOfRangeException>(size > 0);
            Contract.Requires<ObjectDisposedException>(!this.IsDisposed);
            Contract.Ensures(Contract.Result<RangedBuffer>() != null);

            int bufferBindingIndex = -1;
            for (int i = 0; i < allocatedBufferBindings.Length; i++)
            {
                if (!allocatedBufferBindings[i])
                {
                    allocatedBufferBindings[i] = true;
                    bufferBindingIndex = i;
                }
            }

            if (bufferBindingIndex < 0)
            {
                throw new InvalidOperationException("No free buffer binding indices.");
            }

            RangedBuffer buffer = new RangedBuffer(
                this.buffer,
                new BufferRange(this.buffer.Count, size),
                bufferBindingIndex,
                BufferRangeTarget.UniformBuffer
            ).Zeroed();
            buffer.Disposed += (s, e) => this.allocatedBufferBindings[bufferBindingIndex] = false;

            return buffer;
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                this.buffer.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}
