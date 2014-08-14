﻿using System;
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
        private readonly object initializationLock = new object();

        private Buffer _IndexBuffer;

        public Buffer IndexBuffer
        {
            get
            {
                return _IndexBuffer;
            }
            private set
            {
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
                return _VertexBuffers;
            }
            private set
            {
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
