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
        private ILog logger = LogManager.GetLogger(typeof(VertexArrayObject));

        public Buffer IndexBuffer { get; private set; }

        public int IndexCount
        {
            get
            {
                return this.IndexBuffer.Count;
            }
        }

        public ImmutableList<BufferConfiguration> VertexBuffers { get; private set; }

        public VertexArrayObject(IEnumerable<BufferConfiguration> buffers, Buffer indexBuffer)
            : base(GL.GenVertexArray())
        {
            Contract.Requires<ArgumentNullException>(buffers != null);
            Contract.Requires<ArgumentNullException>(indexBuffer != null);

            this.IndexBuffer = indexBuffer;
            this.VertexBuffers = buffers.ToImmutableList();

            logger.Debug("Initializing a new VertexArrayObject with {0} vertex buffers and {1} indices.".FormatWith(this.VertexBuffers.Count, indexBuffer.Count));

            this.Bind();
            foreach (BufferConfiguration bufferConfig in this.VertexBuffers)
            {
                using (BindableClause releaser = new BindableClause(bufferConfig.VertexBuffer))
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
            this.Unbind();

            logger.Debug("VertexArrayObject initialized.");
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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.IndexBuffer != null);
            Contract.Invariant(this.VertexBuffers != null);
        }
    }
}
