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

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class VertexArrayObject : GLObject, IBindable
    {
        private readonly object initializationLock = new object();

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
                return this.IndexBuffer.Count;
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

        private ImmutableList<BufferDescription> _VertexBuffers;

        public ImmutableList<BufferDescription> VertexBuffers
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableList<BufferDescription>>() != null);

                return _VertexBuffers;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _VertexBuffers, value);
            }
        }

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
                lock (this.initializationLock)
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
                                            vertexPointer.Normalize,
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
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.DeleteVertexArray(this);
            }
            catch (Exception ex)
            {
                Logger.Warn(() => "An exception of type '{0}' was thrown while disposing the {1}'s underlying OpenGL vertex array object.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(VertexArrayObject).Name), ex);
            }
            base.Dispose(disposing);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._IndexBuffer != null);
            Contract.Invariant(this._VertexBuffers != null);
        }
    }
}
