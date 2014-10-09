using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class VertexArrayObjectManager<TVertex> : DisposableEntity
        where TVertex : struct
    {
        private IBuffer indexBuffer;

        private IBuffer vertexBuffer;

        private VertexArrayObject vertexArrayObject;

        private ImmutableArray<VertexAttributePointer> _VertexAttributePointers;

        public ImmutableArray<VertexAttributePointer> VertexAttributePointers
        {
            get
            {
                return _VertexAttributePointers;
            }
            set
            {
                this.SetProperty(ref _VertexAttributePointers, value);
            }
        }

        public VertexArrayObjectManager(IEnumerable<VertexAttributePointer> vertexAttributePointers)
        {
            Contract.Requires<ArgumentNullException>(vertexAttributePointers != null);

            this.VertexAttributePointers = vertexAttributePointers.ToImmutableArray();
        }

        [CLSCompliant(false)]
        public VertexArrayObjectManager(
                    IEnumerable<VertexAttributePointer> vertexAttributePointers,
                    TVertex[] vertices,
                    ushort[] indices
                )
            : this(vertexAttributePointers)
        {
            Contract.Requires<ArgumentNullException>(vertexAttributePointers != null);
            Contract.Requires<ArgumentNullException>(vertices != null);
            Contract.Requires<ArgumentNullException>(indices != null);

            this.Set(indices);
            this.Set(vertices);
        }

        public void Draw()
        {
            VertexArrayObject vao = this.vertexArrayObject;
            if (vao == null)
            {
                IBuffer indexBuffer = this.indexBuffer;
                if (indexBuffer == null)
                {
                    throw new InvalidOperationException("The index buffer was null. Set it before drawing.");
                }
                IBuffer vertexBuffer = this.vertexBuffer;
                if (vertexBuffer == null)
                {
                    throw new InvalidOperationException("The vertex buffer was null. Set it before drawing.");
                }

                this.vertexArrayObject = vao = new VertexArrayObject(
                    indexBuffer,
                    new BufferDescription(vertexBuffer, this.VertexAttributePointers.ToArray())
                );
            }

            vao.DrawIndexed();
        }

        [CLSCompliant(false)]
        public void Set(ushort[] indices)
        {
            Contract.Requires<ArgumentNullException>(indices != null);
            Contract.Requires<ArgumentException>(indices.Any());

            this.SetBuffer(indices, BufferTarget.ElementArrayBuffer, ref this.indexBuffer);
        }

        public void Set(TVertex[] vertices)
        {
            Contract.Requires<ArgumentNullException>(vertices != null);
            Contract.Requires<ArgumentException>(vertices.Any());

            this.SetBuffer(vertices, BufferTarget.ArrayBuffer, ref this.vertexBuffer);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                IBuffer indexBuffer = this.indexBuffer;
                if (indexBuffer != null)
                {
                    indexBuffer.Dispose();
                }
                IBuffer vertexBuffer = this.vertexBuffer;
                if (vertexBuffer != null)
                {
                    vertexBuffer.Dispose();
                }
                VertexArrayObject vao = this.vertexArrayObject;
                if (vao != null)
                {
                    vao.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        private void SetBuffer<T>(T[] data, BufferTarget target, ref IBuffer buffer)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);

            if (buffer != null)
            {
                buffer.Set(data);
            }
            else
            {
                IBuffer newBuffer = new BufferObject(target, BufferUsageHint.StaticDraw);
                this.SetBuffer(data, target, ref newBuffer);
                buffer = newBuffer;

                VertexArrayObject vao = Interlocked.Exchange(ref this.vertexArrayObject, null); // Destroy old VAO
                if (vao != null)
                {
                    vao.Dispose();
                }
            }
        }
    }
}
