using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a region inside which a buffer is mapped so it can be written to through a pointer.
    /// </summary>
    public struct BufferMapping : IDisposable
    {
        /// <summary>
        /// The <see cref="BufferAccess"/>.
        /// </summary>
        private readonly BufferAccess access;

        /// <summary>
        /// The <see cref="IBuffer"/> to map.
        /// </summary>
        private readonly IBuffer buffer;

        /// <summary>
        /// The buffer data handle.
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="BufferMapping"/>.
        /// </summary>
        /// <param name="buffer">The <see cref="IBuffer"/> to map.</param>
        /// <param name="access">The <see cref="BufferAccess"/>.</param>
        public BufferMapping(IBuffer buffer, BufferAccess access)
            : this(buffer, access, true)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);
        }

        /// <summary>
        /// Initializes a new <see cref="BufferMapping"/>.
        /// </summary>
        /// <param name="buffer">The <see cref="IBuffer"/> to map.</param>
        /// <param name="access">The <see cref="BufferAccess"/>.</param>
        /// <param name="mapImmediately"><c>true</c> if the <paramref name="buffer"/> shall be mapped directly, otherwise <c>false</c>.</param>
        public BufferMapping(IBuffer buffer, BufferAccess access, bool mapImmediately = true)
            : this()
        {
            Contract.Requires<ArgumentNullException>(buffer != null);

            this.access = access;
            this.buffer = buffer;
            if (mapImmediately)
            {
                this.Map();
            }
        }

        /// <summary>
        /// Disposes the <see cref="BufferMapping"/> unmapping the buffer.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Unmap();
        }

        /// <summary>
        /// Maps the buffer and assigns the <see cref="P:Handle"/>.
        /// </summary>
        /// <returns></returns>
        public IntPtr Map()
        {
            return (this.Handle = this.buffer.Map(this.access));
        }

        /// <summary>
        /// Unmaps the buffer and destroys the <see cref="P:Handle"/>. See remarks.
        /// </summary>
        /// <remarks><see cref="P:Handle"/> will be invalidated with this method. Any previous copies may not be used anymore.</remarks>
        public void Unmap()
        {
            this.Handle = IntPtr.Zero;
            this.buffer.Unmap();
        }
    }
}
