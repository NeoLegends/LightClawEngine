using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class VertexArrayObject : GLObject, IBindable, IDrawable
    {
        private readonly object initializationLock = new object();

        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        private BeginMode _DrawMode;

        public BeginMode DrawMode
        {
            get
            {
                return _DrawMode;
            }
            private set
            {
                this.SetProperty(ref _DrawMode, value);
            }
        }

        private DrawElementsType _IndexBufferType;

        public DrawElementsType IndexBufferType
        {
            get
            {
                return _IndexBufferType;
            }
            private set
            {
                this.SetProperty(ref _IndexBufferType, value);
            }
        }

        private IBuffer _IndexBuffer;

        public IBuffer IndexBuffer
        {
            get
            {
                Contract.Ensures(Contract.Result<IBuffer>() != null);

                return _IndexBuffer;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _IndexBuffer, value);
            }
        }

        public int IndexCount
        {
            get
            {
                return this.IndexBuffer.Length;
            }
        }

        private bool _IsInitialized;

        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            private set
            {
                this.SetProperty(ref _IsInitialized, value);
            }
        }

        private ImmutableArray<BufferDescription> _VertexBuffers;

        public ImmutableArray<BufferDescription> VertexBuffers
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableArray<BufferDescription>>() != null);

                return _VertexBuffers;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _VertexBuffers, value);
            }
        }

        public VertexArrayObject(IBuffer indexBuffer, params BufferDescription[] buffers)
            : this(indexBuffer, BeginMode.Triangles, DrawElementsType.UnsignedShort, buffers)
        {
            Contract.Requires<ArgumentNullException>(buffers != null);
            Contract.Requires<ArgumentNullException>(indexBuffer != null);
            Contract.Requires<ArgumentException>(!buffers.Any(desc => desc.Buffer.Target == BufferTarget.ElementArrayBuffer));
        }

        public VertexArrayObject(IBuffer indexBuffer, BeginMode drawMode, DrawElementsType indexBufferType, params BufferDescription[] buffers)
        {
            Contract.Requires<ArgumentNullException>(buffers != null);
            Contract.Requires<ArgumentNullException>(indexBuffer != null);
            Contract.Requires<ArgumentException>(!buffers.Any(desc => desc.Buffer.Target == BufferTarget.ElementArrayBuffer));

            this.DrawMode = drawMode;
            this.IndexBuffer = indexBuffer;
            this.IndexBufferType = indexBufferType;
            this.VertexBuffers = buffers.ToImmutableArray();
        }

        public void Bind()
        {
            this.Initialize();
            GL.BindVertexArray(this);
        }

        void IDrawable.Draw()
        {
            this.DrawIndexed();
        }

        public void DrawIndexed()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn, this.IndexCount, this.IndexCount))
            using (Binding vaoBinding = new Binding(this))
            {
                GL.DrawElements(this.DrawMode, this.IndexCount, this.IndexBufferType, 0);
            }
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Handle = GL.GenVertexArray();
                        using (Binding vaoBinding = new Binding(this))
                        {
                            foreach (BufferDescription bufferConfig in this.VertexBuffers)
                            {
                                using (Binding vboBinding = new Binding(bufferConfig.Buffer))
                                {
                                    foreach (VertexAttributePointer vertexPointer in bufferConfig.VertexAttributePointers)
                                    {
                                        GL.EnableVertexAttribArray(vertexPointer.Index);
                                        GL.VertexAttribPointer(
                                            vertexPointer.Index,
                                            vertexPointer.Size,
                                            vertexPointer.Type,
                                            vertexPointer.Normalize,
                                            vertexPointer.Stride,
                                            vertexPointer.Offset
                                        );
                                        GL.DisableVertexAttribArray(vertexPointer.Index);
                                    }
                                }
                            }
                            this.IndexBuffer.Bind();
                        }
                        this.IndexBuffer.Unbind();

                        this.IsInitialized = true;
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (this.IsInitialized)
                {
                    GL.DeleteVertexArray(this);
                }
                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._IndexBuffer != null);
            Contract.Invariant(this._VertexBuffers != null);
        }
    }
}
