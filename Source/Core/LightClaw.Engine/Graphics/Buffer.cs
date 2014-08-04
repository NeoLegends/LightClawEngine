using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a data store on GPU memory.
    /// </summary>
    public class Buffer : GLObject, IBindable
    {
        /// <summary>
        /// Indicates whether the <see cref="Buffer"/> still needs an OpenGL buffer name.
        /// </summary>
        private bool requiresNameGeneration = true;

        /// <summary>
        /// Indicates whether the data has yet to be uploaded to the GPU.
        /// </summary>
        private bool requiresDataUpload = false;

        /// <summary>
        /// The handle to the data.
        /// </summary>
        private GCHandle dataHandle;

        /// <summary>
        /// The size of the data in bytes.
        /// </summary>
        private IntPtr dataSize;

        /// <summary>
        /// The length of the buffer in bytes.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="Buffer"/>.
        /// </summary>
        public BufferUsageHint Hint { get; private set; }

        /// <summary>
        /// The <see cref="BufferTarget"/>.
        /// </summary>
        public BufferTarget Target { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="Buffer"/> setting usage hint and target.
        /// </summary>
        /// <param name="target">The <see cref="BufferTarget"/>.</param>
        /// <param name="hint">The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="Buffer"/>.</param>
        public Buffer(BufferTarget target, BufferUsageHint hint)
        {
            this.Hint = hint;
            this.Target = target;
        }

        /// <summary>
        /// Initializes a new <see cref="Buffer"/> setting usage hint and target and specifying the data to upload.
        /// </summary>
        /// <param name="target">The <see cref="BufferTarget"/>.</param>
        /// <param name="hint">The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="Buffer"/>.</param>
        /// <param name="dataHandle">A managed handle to the data.</param>
        /// <param name="size">The size of the data in bytes.</param>
        /// <remarks>
        /// The data will be uploaded lazily, meaning that the upload will take place on the first call to <see cref="M:Bind"/>.
        /// This allows for data loading and <see cref="Buffer"/>-creation on a background thread.
        /// </remarks>
        public Buffer(BufferTarget target, BufferUsageHint hint, GCHandle dataHandle, IntPtr size)
            : this(target, hint)
        {
            this.dataHandle = dataHandle;
            this.dataSize = size;
            this.requiresDataUpload = true;
        }

        /// <summary>
        /// Uploads the data to the GPU if required and binds the buffer to the specified <see cref="BufferTarget"/>.
        /// </summary>
        public void Bind()
        {
            if (this.requiresNameGeneration) // Buffer name will be generated lazily in order to allow for multithreaded content loading
            {
                this.Handle = GL.GenBuffer();
                this.requiresNameGeneration = false;
            }
            if (this.requiresDataUpload) // Same goes for data, upload when required on drawing thread
            {
                this.Update(this.dataHandle.AddrOfPinnedObject(), this.dataSize);
                this.dataHandle.Free();
                this.dataSize = IntPtr.Zero;
                this.requiresDataUpload = false;
            }
            GL.BindBuffer(this.Target, this);
        }

        /// <summary>
        /// Unbinds the buffer from the current <see cref="BufferTarget"/>.
        /// </summary>
        public void Unbind()
        {
            GL.BindBuffer(this.Target, 0);
        }

        /// <summary>
        /// Updates the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the new data.</typeparam>
        /// <param name="data">The data itself.</param>
        public void Update<T>(T[] data)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);

            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            this.Update(dataHandle.AddrOfPinnedObject(), (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length));
            dataHandle.Free();
        }

        /// <summary>
        /// Updates a range of the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the new data.</typeparam>
        /// <param name="data">The data itself.</param>
        /// <param name="offset">The offset in bytes to start applying the new data at.</param>
        public void UpdateRange<T>(T[] data, int offset)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);

            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            this.UpdateRange(dataHandle.AddrOfPinnedObject(), (IntPtr)offset, (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length));
            dataHandle.Free();
        }

        /// <summary>
        /// Updates the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <param name="size">The size of the data in bytes.</param>
        public void Update(IntPtr data, IntPtr size)
        {
            using (GLBinding bufferBinding = new GLBinding(this))
            {
                GL.BufferData(this.Target, size, data, this.Hint);
                this.CalculateCount((int)size, 0);
            }
        }

        /// <summary>
        /// Updates the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <param name="size">The size of the data in bytes.</param>
        /// <param name="offset">The offset in bytes to start applying the new data at.</param>
        public void UpdateRange(IntPtr data, IntPtr offset, IntPtr size)
        {
            using (GLBinding bufferBindg = new GLBinding(this))
            {
                GL.BufferSubData(this.Target, offset, size, data);
                this.CalculateCount((int)size, (int)offset);
            }
        }

        /// <summary>
        /// Disposes the <see cref="Buffer"/> removing it from the GPU memory.
        /// </summary>
        /// <param name="disposing">A boolean indicating whether to dispose managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.DeleteBuffer(this);
            }
            catch (AccessViolationException)
            {
                throw; // Log and swallow in the future!
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Recalculates the size of the <see cref="Buffer"/> after a change.
        /// </summary>
        /// <param name="sizeOfNewData">The size of the new data in bytes.</param>
        /// <param name="offset">The offset of the new data.</param>
        private void CalculateCount(int sizeOfNewData, int offset)
        {
            Contract.Requires<ArgumentOutOfRangeException>(sizeOfNewData >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);

            int difference = (sizeOfNewData - (this.Count - offset));
            this.Count = this.Count + ((difference >= 0) ? difference : 0);
        }

        /// <summary>
        /// Creates a new buffer from an array.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of data to upload.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="target">The <see cref="BufferTarget"/> the <see cref="Buffer"/> will be bound to.</param>
        /// <remarks>
        /// The data will be uploaded lazily, meaning that the upload will take place on the first call to <see cref="M:Bind"/>.
        /// This allows for data loading and <see cref="Buffer"/>-creation on a background thread.
        /// </remarks>
        /// <returns>The newly created <see cref="Buffer"/>.</returns>
        public static Buffer Create<T>(T[] data, BufferTarget target)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<Buffer>() != null);

            return Create(data, target, BufferUsageHint.StaticDraw);
        }

        /// <summary>
        /// Creates a new buffer from an array.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of data to upload.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="target">The <see cref="BufferTarget"/> the <see cref="Buffer"/> will be bound to.</param>
        /// <param name="hint">The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="Buffer"/>.</param>
        /// <remarks>
        /// The data will be uploaded lazily, meaning that the upload will take place on the first call to <see cref="M:Bind"/>.
        /// This allows for data loading and <see cref="Buffer"/>-creation on a background thread.
        /// </remarks>
        /// <returns>The newly created <see cref="Buffer"/>.</returns>
        public static Buffer Create<T>(T[] data, BufferTarget target, BufferUsageHint hint)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<Buffer>() != null);

            return new Buffer(target, hint, GCHandle.Alloc(data, GCHandleType.Pinned), (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length));
        }
    }
}
