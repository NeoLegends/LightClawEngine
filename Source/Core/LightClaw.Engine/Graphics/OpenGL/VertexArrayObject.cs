using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Core;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a vertex array object.
    /// </summary>
    [DebuggerDisplay("Index Count: {IndexCount}, Draw Mode: {DrawMode}, Index Type: {IndexType}")]
    public class VertexArrayObject : GLObject, IBindable, IDrawable
    {
        /// <summary>
        /// Notifies about the start of a drawing operation.
        /// </summary>
        public event EventHandler<ParameterEventArgs> Drawing;

        /// <summary>
        /// Notifies about the end of a drawing operation.
        /// </summary>
        public event EventHandler<ParameterEventArgs> Drawn;

        private BeginMode _DrawMode;

        /// <summary>
        /// The draw mode.
        /// </summary>
        /// <seealso cref="BeginMode"/>
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

        private IReadOnlyBuffer _IndexBuffer;

        /// <summary>
        /// The <see cref="IBuffer"/> containing the index data.
        /// </summary>
        public IReadOnlyBuffer IndexBuffer
        {
            get
            {
                Contract.Ensures(Contract.Result<IReadOnlyBuffer>() != null);

                return _IndexBuffer;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _IndexBuffer, value);
            }
        }

        /// <summary>
        /// The amount of indices stored.
        /// </summary>
        public int IndexCount
        {
            get
            {
                return this.IndexBuffer.Length / this.IndexType.GetElementSize();
            }
        }

        private DrawElementsType _IndexType;

        /// <summary>
        /// The type of the index values.
        /// </summary>
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

        private ImmutableArray<BufferDescription> _VertexBuffers;

        /// <summary>
        /// The buffers containing the actual vertex data.
        /// </summary>
        public ImmutableArray<BufferDescription> VertexBuffers
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

        /// <summary>
        /// Initializes a new <see cref="VertexArrayObject"/> drawing triangles using <see cref="UInt16"/> indices.
        /// </summary>
        /// <param name="indexBuffer">The <see cref="IBuffer"/> containing the index data.</param>
        /// <param name="buffers">The buffers containing the actual vertex data.</param>
        public VertexArrayObject(IReadOnlyBuffer indexBuffer, params BufferDescription[] buffers)
            : this(indexBuffer, BeginMode.Triangles, DrawElementsType.UnsignedShort, buffers)
        {
            Contract.Requires<ArgumentNullException>(buffers != null);
            Contract.Requires<ArgumentNullException>(indexBuffer != null);
            Contract.Requires<ArgumentException>(indexBuffer.Target == BufferTarget.ElementArrayBuffer);
        }

        /// <summary>
        /// Initializes a new <see cref="VertexArrayObject"/>.
        /// </summary>
        /// <param name="indexBuffer">The <see cref="IBuffer"/> containing the index data.</param>
        /// <param name="drawMode">The <see cref="BeginMode"/>.</param>
        /// <param name="indexBufferType">The type of the indices.</param>
        /// <param name="buffers">The buffers containing the actual vertex data.</param>
        public VertexArrayObject(IReadOnlyBuffer indexBuffer, BeginMode drawMode, DrawElementsType indexBufferType, params BufferDescription[] buffers)
        {
            Contract.Requires<ArgumentNullException>(buffers != null);
            Contract.Requires<ArgumentNullException>(indexBuffer != null);
            Contract.Requires<ArgumentException>(indexBuffer.Target == BufferTarget.ElementArrayBuffer);

            this.VerifyAccess();

            this.DrawMode = drawMode;
            this.IndexBuffer = indexBuffer;
            this.IndexType = indexBufferType;
            this.VertexBuffers = buffers.ToImmutableArray();

            this.Handle = GL.GenVertexArray();

            // Can't use Binding and using clause here because it causes a stack overflow (Bindable.Bind -> Initialize)
            try
            {
                GL.BindVertexArray(this);
                foreach (BufferDescription desc in this.VertexBuffers)
                {
                    if (desc.Buffer != null)
                    {
                        using (Binding vboBinding = desc.Buffer.Bind())
                        {
                            foreach (VertexAttributePointer vertexPointer in desc.VertexAttributePointers.OrderBy(vap => vap.Index))
                            {
                                vertexPointer.Enable();
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

        /// <summary>
        /// Binds the <see cref="VertexArrayObject"/>.
        /// </summary>
        public Binding Bind()
        {
            this.VerifyAccess();
            GL.BindVertexArray(this);

            // Use for loops for performance
            for (int i = 0; i < this.VertexBuffers.Length; i++)
            {
                ImmutableArray<VertexAttributePointer> pointers = this.VertexBuffers[i].VertexAttributePointers;
                for (int j = 0; j < pointers.Length; j++)
                {
                    pointers[j].Enable();
                }
            }

            return new Binding(this);
        }

        /// <summary>
        /// Draws the <see cref="VertexArrayObject"/>.
        /// </summary>
        void IDrawable.Draw()
        {
            this.DrawIndexed();
        }

        /// <summary>
        /// Draws all vertices.
        /// </summary>
        /// <remarks>
        /// DOES NOT BIND THE <see cref="VertexArrayObject"/>, THIS NEEDS TO BE DONE MANUALLY!
        /// </remarks>
        public void DrawIndexed()
        {
            if (this.IndexCount > 0)
            {
                this.DrawIndexed(0);
            }
        }

        /// <summary>
        /// Draws the vertices starting at a given offset.
        /// </summary>
        /// <remarks>
        /// DOES NOT BIND THE <see cref="VertexArrayObject"/>, THIS NEEDS TO BE DONE MANUALLY!
        /// </remarks>
        /// <param name="offset">The offset to start drawing at.</param>
        public void DrawIndexed(int offset)
        {
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(offset <= this.IndexCount);

            this.DrawIndexed(offset, this.IndexCount - offset);
        }

        /// <summary>
        /// Draws a specified amount of vertices starting at a given offset.
        /// </summary>
        /// <remarks>
        /// DOES NOT BIND THE <see cref="VertexArrayObject"/>, THIS NEEDS TO BE DONE MANUALLY!
        /// </remarks>
        /// <param name="offset">The offset to start drawing at.</param>
        /// <param name="count">The amount of vertices to draw.</param>
        public void DrawIndexed(int offset, int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);

            this.VerifyAccess();
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn, count, count))
            {
                GL.DrawElements(this.DrawMode, count, this.IndexType, offset);
            }
        }

        /// <summary>
        /// Unbinds the <see cref="VertexArrayObject"/> from the pipeline.
        /// </summary>
        public void Unbind()
        {
            this.VerifyAccess();

            // Use for loops for performance
            for (int i = 0; i < this.VertexBuffers.Length; i++)
            {
                ImmutableArray<VertexAttributePointer> pointers = this.VertexBuffers[i].VertexAttributePointers;
                for (int j = 0; j < pointers.Length; j++)
                {
                    pointers[j].Disable();
                }
            }
            GL.BindVertexArray(0);
        }

        protected override void Dispose(bool disposing)
        {
            this.Dispatcher.ImmediateOr(this.DeleteVertexArrayObject, disposing, DispatcherPriority.Background);
        }

        [System.Security.SecurityCritical]
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private void DeleteVertexArrayObject(bool disposing)
        {
            try
            {
                GL.DeleteVertexArray(this);
                this.IndexBuffer.Dispose();
                foreach (IReadOnlyBuffer buf in this.VertexBuffers.Select(vbo => vbo.Buffer))
                {
                    buf.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.Warn(
                    ex,
                    "A {0} was thrown while disposing of a {1}. In most cases, this should be nothing to worry about. Check the error message to make sure there really is nothing to worry about, though.",
                    ex.GetType().Name, typeof(VertexArrayObject).Name
                );
            }
            finally
            {
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
