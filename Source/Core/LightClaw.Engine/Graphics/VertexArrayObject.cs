using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class VertexArrayObject : GLObject
    {
        public BufferConfiguration[] Buffers { get; private set; }

        public VertexArrayObject(IEnumerable<BufferConfiguration> buffers)
            : base(GL.GenVertexArray())
        {
            Contract.Requires<ArgumentNullException>(buffers != null);

            this.Buffers = buffers.ToArray();
            this.Bind();
            foreach (BufferConfiguration bufferConfig in this.Buffers)
            {
                int attributePointerCount = 0;
                bufferConfig.VertexBuffer.Bind();
                foreach (VertexAttributePointer vertexPointer in bufferConfig.VertexAttributePointers)
                {
                    GL.EnableVertexAttribArray(attributePointerCount++);
                    GL.VertexAttribPointer(
                        vertexPointer.Index, 
                        vertexPointer.Size, 
                        vertexPointer.Type, 
                        vertexPointer.IsNormalized, 
                        vertexPointer.Stride, 
                        vertexPointer.Offset
                    );
                }
                bufferConfig.IndexBuffer.Bind();
            }
            this.Unbind();
        }

        public void Bind()
        {
            GL.BindVertexArray(this);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.DeleteVertexArray(this);
            }
            catch (AccessViolationException)
            {
                throw; // Log and swallow in the future
            }
            base.Dispose(disposing);
        }
    }
}
