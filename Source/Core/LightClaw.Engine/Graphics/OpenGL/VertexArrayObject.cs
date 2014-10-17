using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    [DebuggerDisplay("Handle = {Handle}, Index Count = {IndexCount}, Draw Mode = {DrawMode}, Index Type = {IndexType}")]
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

        private DrawElementsType _IndexType;

        public DrawElementsType IndexType
        {
            get
            {
                return _IndexType;
            }
            private set
            {
                this.SetProperty(ref _IndexType, value);
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
                return this.IndexBuffer.Length / this.IndexType.GetElementSize();
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
            Contract.Requires<ArgumentException>(indexBuffer.Target == BufferTarget.ElementArrayBuffer);
        }

        public VertexArrayObject(IBuffer indexBuffer, BeginMode drawMode, DrawElementsType indexBufferType, params BufferDescription[] buffers)
        {
            Contract.Requires<ArgumentNullException>(buffers != null);
            Contract.Requires<ArgumentNullException>(indexBuffer != null);
            Contract.Requires<ArgumentException>(indexBuffer.Target == BufferTarget.ElementArrayBuffer);

            this.DrawMode = drawMode;
            this.IndexBuffer = indexBuffer;
            this.IndexType = indexBufferType;
            this.VertexBuffers = buffers.ToImmutableArray();
        }

        public void Bind()
        {
            this.Initialize();
            GL.BindVertexArray(this);
            this.EnableAttributeArrays();
        }

        void IDrawable.Draw()
        {
            this.DrawIndexed();
        }

        public void DrawIndexed()
        {
            this.DrawIndexed(0);
        }

        public void DrawIndexed(int offset)
        {
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);

            this.DrawIndexed(offset, this.IndexCount - offset);
        }

        public void DrawIndexed(int offset, int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);

            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn, count, count))
            {
                GL.DrawElements(this.DrawMode, count, this.IndexType, offset);
            }
        }

        public void Unbind()
        {
            this.DisableAttributeArrays();
            GL.BindVertexArray(0);
        }

        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        protected override void Dispose(bool disposing)
        {
            lock (this.initializationLock)
            {
                if (this.IsInitialized)
                {
                    try
                    {
                        GL.DeleteVertexArray(this);
                    }
                    catch (AccessViolationException ex)
                    {
                        Logger.Warn(e => "An {0} was thrown while disposing of a {1}. This might or might not be an unwanted condition.".FormatWith(e.GetType().Name, typeof(VertexArrayObject).Name), ex, ex);
                    }
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnInitialize()
        {
            Logger.Debug(() => "Initializing {0}.".FormatWith(typeof(VertexArrayObject).Name));

            this.Handle = GL.GenVertexArray();
            try // Can't use Binding and using clause here because it causes a stackoverflow
            {
                GL.BindVertexArray(this);
                foreach (BufferDescription desc in this.VertexBuffers)
                {
                    using (Binding vboBinding = new Binding(desc.Buffer))
                    {
                        foreach (VertexAttributePointer vertexPointer in desc.VertexAttributePointers)
                        {
                            using (Binding pointerBinding = new Binding(vertexPointer))
                            {
                                vertexPointer.Apply();
                            }
                        }
                    }
                }
                this.IndexBuffer.Bind();
            }
            finally
            {
                GL.BindVertexArray(0);
            }
            this.IndexBuffer.Unbind();
        }

        private void EnableAttributeArrays()
        {
            foreach (BufferDescription desc in this.VertexBuffers)
            {
                foreach (VertexAttributePointer vertexPointer in desc.VertexAttributePointers)
                {
                    vertexPointer.Enable();
                }
            }
        }

        private void DisableAttributeArrays()
        {
            foreach (BufferDescription desc in this.VertexBuffers)
            {
                foreach (VertexAttributePointer vertexPointer in desc.VertexAttributePointers)
                {
                    vertexPointer.Disable();
                }
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
