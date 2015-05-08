using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a region treated as separate <see cref="IBuffer"/> inside another <see cref="IBuffer"/>.
    /// </summary>
    public class RangedBuffer : GLObject, IBuffer
    {
        /// <summary>
        /// Backing field.
        /// </summary>
        private IBuffer _BaseBuffer;

        /// <summary>
        /// The underlying <see cref="IBuffer"/> storing the data.
        /// </summary>
        public IBuffer BaseBuffer
        {
            get
            {
                Contract.Ensures(Contract.Result<IBuffer>() != null);

                return _BaseBuffer;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _BaseBuffer, value);
            }
        }

        /// <summary>
        /// The length of the buffer range.
        /// </summary>
        public int Length
        {
            get
            {
                return this.Range.Length;
            }
        }

        /// <summary>
        /// The <see cref="BufferUsageHint"/>.
        /// </summary>
        public BufferUsageHint Hint
        {
            get
            {
                return this.BaseBuffer.Hint;
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private int _Index = 0;

        /// <summary>
        /// The buffer binding index the buffer range will be bound to.
        /// </summary>
        public int Index
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _Index;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _Index, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _OwnsBaseBuffer;

        /// <summary>
        /// Indicates whether the <see cref="RangedBuffer"/> owns the underlying <see cref="IBuffer"/> and can dispose
        /// of it when it is disposed itself.
        /// </summary>
        public bool OwnsBaseBuffer
        {
            get
            {
                return _OwnsBaseBuffer;
            }
            private set
            {
                this.SetProperty(ref _OwnsBaseBuffer, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private BufferRange _Range;

        /// <summary>
        /// The actual range definition inside the underlying <see cref="IBuffer"/>.
        /// </summary>
        public BufferRange Range
        {
            get
            {
                return _Range;
            }
            private set
            {
                this.SetProperty(ref _Range, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private BufferRangeTarget _RangeTarget;

        /// <summary>
        /// The <see cref="BufferRangeTarget"/> the buffer range will be bound to.
        /// </summary>
        public BufferRangeTarget RangeTarget
        {
            get
            {
                return _RangeTarget;
            }
            private set
            {
                this.SetProperty(ref _RangeTarget, value);
            }
        }

        /// <summary>
        /// The <see cref="BufferTarget"/> the underlying <see cref="IBuffer"/> will be bound to.
        /// </summary>
        public BufferTarget Target
        {
            get
            {
                return this.BaseBuffer.Target;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="RangedBuffer"/> without taking ownage of the <paramref name="baseBuffer"/>.
        /// </summary>
        /// <param name="baseBuffer">The buffer containing the actual data.</param>
        /// <param name="range">The range the buffer occupies inside the base buffer.</param>
        /// <param name="index">The buffer binding index the <see cref="RangedBuffer"/> will be bound to.</param>
        /// <param name="rangeTarget">The <see cref="BufferRangeTarget"/> the buffer range will be bound to.</param>
        public RangedBuffer(IBuffer baseBuffer, BufferRange range, int index, BufferRangeTarget rangeTarget)
            : this(baseBuffer, range, index, rangeTarget, false)
        {
            Contract.Requires<ArgumentNullException>(baseBuffer != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
        }

        /// <summary>
        /// Initializes a new <see cref="RangedBuffer"/>.
        /// </summary>
        /// <param name="baseBuffer">The buffer containing the actual data.</param>
        /// <param name="range">The range the buffer occupies inside the base buffer.</param>
        /// <param name="index">The buffer binding index the <see cref="RangedBuffer"/> will be bound to.</param>
        /// <param name="rangeTarget">The <see cref="BufferRangeTarget"/> the buffer range will be bound to.</param>
        /// <param name="ownsBaseBuffer">
        /// Indicates whether the <see cref="RangedBuffer"/> owns the underlying <see cref="IBuffer"/> and can dispose
        /// of it when it is disposed itself.
        /// </param>
        public RangedBuffer(IBuffer baseBuffer, BufferRange range, int index, BufferRangeTarget rangeTarget, bool ownsBaseBuffer)
            : base(baseBuffer.Handle)
        {
            Contract.Requires<ArgumentNullException>(baseBuffer != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            this.BaseBuffer = baseBuffer;
            this.Index = index;
            this.OwnsBaseBuffer = ownsBaseBuffer;
            this.Range = range;
            this.RangeTarget = rangeTarget;
        }

        /// <summary>
        /// Binds the <see cref="RangedBuffer"/> to its <see cref="RangeTarget"/>.
        /// </summary>
        public void Bind()
        {
            this.Initialize();
            GL.BindBufferRange(this.RangeTarget, this.Index, this.BaseBuffer.Handle, (IntPtr)this.Range.Start, (IntPtr)this.Range.Length);
        }

        /// <summary>
        /// Unbinds the <see cref="RangedBuffer"/>.
        /// </summary>
        public void Unbind()
        {
            GL.BindBufferRange(this.RangeTarget, this.Index, 0, (IntPtr)this.Range.Start, (IntPtr)this.Range.Length);
        }

        /// <summary>
        /// Gets all the data in the <see cref="RangedBuffer"/>.
        /// </summary>
        /// <remarks>
        /// This operation cannot be done asynchronously, thus it may seriously affect performance if called repeatedly!
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> to get the data as.</typeparam>
        /// <returns>The data inside the buffer as structs of the specified <see cref="Type"/>.</returns>
        public T[] Get<T>()
            where T : struct
        {
            return this.GetRange<T>(0, this.Range.Length);
        }

        /// <summary>
        /// Gets a range of the buffers data.
        /// </summary>
        /// <remarks>
        /// This operation cannot be done asynchronously, thus it may seriously affect performance if called repeatedly!
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> to get the data as.</typeparam>
        /// <param name="offset">The offset (in bytes).</param>
        /// <param name="count">The amount of bytes to receive.</param>
        /// <returns>The data inside the buffer as structs of the specified <see cref="Type"/>.</returns>
        public T[] GetRange<T>(int offset, int count)
            where T : struct
        {
            return this.BaseBuffer.GetRange<T>(this.Range.Start + offset, count);
        }

        /// <summary>
        /// Maps the buffer with read-write access.
        /// </summary>
        /// <remarks>
        /// Mapping the buffer means obtaining a pointer that points directly to the buffer memory, without any copy in-between.
        /// This is useful in situations where it's beneficial to directly write the data into the buffer instead of loading it
        /// into managed memory first and then transferring it to video memory.
        ///
        /// This operation will bind the buffer and overwrite any previous bindings.
        /// </remarks>
        /// <returns>A pointer to the buffer's memory.</returns>
        /// <seealso cref="BufferAccess"/>
        public IntPtr Map()
        {
            return this.Map(BufferAccess.ReadWrite);
        }

        /// <summary>
        /// Maps the buffer with specified <paramref name="access"/>.
        /// </summary>
        /// <remarks>
        /// Mapping the buffer means obtaining a pointer that points directly to the buffer memory, without any copy in-between.
        /// This is useful in situations where it's beneficial to directly write the data into the buffer instead of loading it
        /// into managed memory first and then transferring it to video memory.
        ///
        /// This operation will bind the buffer and overwrite any previous bindings.
        /// </remarks>
        /// <param name="access">An access mask used to indicate what one's intention with the data pointer is.</param>
        /// <returns>A pointer to the buffer's memory.</returns>
        /// <seealso cref="BufferAccess"/>
        public IntPtr Map(BufferAccess access)
        {
            this.Bind();
            return GL.MapBufferRange(this.Target, (IntPtr)this.Range.Start, (IntPtr)this.Range.Length, access.ToAccessMask());
        }

        /// <summary>
        /// Unmaps the buffer.
        /// </summary>
        /// <remarks>This method expects the current <see cref="IBuffer"/> to be bound.</remarks>
        public void Unmap()
        {
            GL.UnmapBuffer(this.Target);
            this.Unbind();
        }

        /// <summary>
        /// Sets the data in the buffer invalidating all previous data.
        /// </summary>
        /// <remarks>
        /// Although the data can be specified in any data type, the data the buffer will receive will be the underlying bytes.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of data to set the buffer to.</typeparam>
        /// <param name="data">The new buffer's data.</param>
        public void Set<T>(T data)
            where T : struct
        {
            if (Marshal.SizeOf(typeof(T)) > this.Range.Length)
            {
                throw new DataTooLargeException(
                    "The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name),
                    Marshal.SizeOf(typeof(T)),
                    0,
                    this.Range
                );
            }

            this.BaseBuffer.SetRange(data, this.Range.Start);
        }

        /// <summary>
        /// Sets the data in the buffer invalidating all previous data.
        /// </summary>
        /// <remarks>
        /// Although the data can be specified in any data type, the data the buffer will receive will be the underlying bytes.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of data to set the buffer to.</typeparam>
        /// <param name="data">The new buffer's data.</param>
        public void Set<T>(T[] data)
            where T : struct
        {
            if (Marshal.SizeOf(typeof(T)) * data.Length > this.Range.Length)
            {
                throw new DataTooLargeException(
                    "The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name),
                    Marshal.SizeOf(typeof(T)) * data.Length,
                    0,
                    this.Range
                );
            }

            this.BaseBuffer.SetRange(data, this.Range.Start);
        }

        /// <summary>
        /// Sets a range inside the buffer without invalidating the previous contents.
        /// </summary>
        /// <remarks>
        /// Although the data can be specified in any data type, the data the buffer will receive will be the underlying bytes.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of data to set the buffer to.</typeparam>
        /// <param name="data">The new buffer's data.</param>
        /// <param name="offset">The offset inside the buffer.</param>
        public void SetRange<T>(T data, int offset)
            where T : struct
        {
            if (Marshal.SizeOf(typeof(T)) > this.Range.Length - offset)
            {
                throw new DataTooLargeException(
                    "The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name),
                    Marshal.SizeOf(typeof(T)),
                    offset,
                    this.Range
                );
            }

            this.BaseBuffer.SetRange(data, this.Range.Start + offset);
        }

        /// <summary>
        /// Sets a range inside the buffer without invalidating the previous contents.
        /// </summary>
        /// <remarks>
        /// Although the data can be specified in any data type, the data the buffer will receive will be the underlying bytes.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of data to set the buffer to.</typeparam>
        /// <param name="data">The new buffer's data.</param>
        /// <param name="offset">The offset inside the buffer.</param>
        public void SetRange<T>(T[] data, int offset)
            where T : struct
        {
            if (Marshal.SizeOf(typeof(T)) * data.Length > this.Range.Length - offset)
            {
                throw new DataTooLargeException(
                    "The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name),
                    Marshal.SizeOf(typeof(T)) * data.Length,
                    offset,
                    this.Range
                );
            }

            this.BaseBuffer.SetRange(data, this.Range.Start + offset);
        }

        /// <summary>
        /// Sets the data in the buffer invalidating all previous data.
        /// </summary>
        /// <param name="data">The new buffer's data.</param>
        /// <param name="sizeInBytes">The size of the new data in bytes.</param>
        public void Set(IntPtr data, int sizeInBytes)
        {
            this.SetRange(data, 0, sizeInBytes);
        }

        /// <summary>
        /// Sets a range inside the buffer without invalidating the previous contents.
        /// </summary>
        /// <param name="data">The new buffer's data.</param>
        /// <param name="offset">The offset inside the buffer.</param>
        /// <param name="sizeInBytes">The size of the new data in bytes.</param>
        public void SetRange(IntPtr data, int offset, int sizeInBytes)
        {
            if (sizeInBytes > this.Range.Length - offset)
            {
                throw new DataTooLargeException(
                    "The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name),
                    sizeInBytes,
                    offset,
                    this.Range
                );
            }

            this.BaseBuffer.SetRange(data, this.Range.Start + offset, sizeInBytes);
        }

        /// <summary>
        /// Zeros-out the <see cref="RangedBuffer"/>.
        /// </summary>
        /// <returns>The <see cref="RangedBuffer"/> itself.</returns>
        public RangedBuffer Zeroed()
        {
            byte[] zeros = new byte[Math.Max(this.Range.Length, 1)];
            zeros.Initialize();
            this.Set(zeros);

            return this;
        }

        /// <summary>
        /// Disposes the <see cref="RangedBuffer"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.OwnsBaseBuffer)
                {
                    this.BaseBuffer.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Gets the event arguments for <see cref="E:Disposed"/>.
        /// </summary>
        /// <returns>The event argument parameter.</returns>
        protected override object GetDisposedArgument()
        {
            return this.Range;
        }

        /// <summary>
        /// Initialization callback.
        /// </summary>
        protected override void OnInitialize() 
        {
            this.BaseBuffer.Initialize();
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._BaseBuffer != null);
            Contract.Invariant(this._Index >= 0);
        }
    }
}
