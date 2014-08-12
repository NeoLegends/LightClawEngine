using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class VertexArrayObject : GLObject, IBindable
    {
        public Buffer IndexBuffer { get; private set; }

        public int IndexCount
        {
            get
            {
                return this.IndexBuffer.Count;
            }
        }

        public bool IsInitialized { get; private set; }

        public ImmutableList<BufferDescription> VertexBuffers { get; private set; }

        public VertexArrayObject(IEnumerable<BufferDescription> buffers, Buffer indexBuffer)
            : base(GL.GenVertexArray())
        {
            Contract.Requires<ArgumentNullException>(buffers != null);
            Contract.Requires<ArgumentNullException>(indexBuffer != null);
            Contract.Requires<ArgumentException>(!buffers.Any(buffer => buffer.Buffer.Target == BufferTarget.ElementArrayBuffer));

            this.IndexBuffer = indexBuffer;
            this.VertexBuffers = buffers.ToImmutableList();
        }

        public void Bind()
        {
            this.Initialize();
            GL.BindVertexArray(this);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                using (GLBinding vaoBinding = new GLBinding(this))
                {
                    foreach (BufferDescription bufferConfig in this.VertexBuffers)
                    {
                        using (GLBinding vboBinding = new GLBinding(bufferConfig.Buffer))
                        {
                            foreach (VertexAttributePointer vertexPointer in bufferConfig.VertexAttributePointers)
                            {
                                GL.EnableVertexAttribArray(vertexPointer.Index);
                                GL.VertexAttribPointer(
                                    vertexPointer.Index,
                                    vertexPointer.Size,
                                    vertexPointer.Type,
                                    vertexPointer.IsNormalized,
                                    vertexPointer.Stride,
                                    vertexPointer.Offset
                                );
                            }
                        }
                    }
                    this.IndexBuffer.Bind();
                }
                this.IndexBuffer.Unbind();
                this.IsInitialized = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                this.Unbind();
                GL.DeleteVertexArray(this);
            }
            catch (AccessViolationException)
            {
                throw; // Log and swallow in the future
            }
            base.Dispose(disposing);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.IndexBuffer != null);
            Contract.Invariant(this.VertexBuffers != null);
        }
    }
}
