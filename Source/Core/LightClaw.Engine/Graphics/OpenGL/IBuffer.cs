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
    /// Represents an OpenGL buffer, data in video memory.
    /// </summary>
    [ContractClass(typeof(IBufferContracts))]
    public interface IBuffer : IBindable, IGLObject, IInitializable
    {
        /// <summary>
        /// Gets the length of the buffer.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the <see cref="BufferUsageHint"/>.
        /// </summary>
        BufferUsageHint Hint { get; }

        /// <summary>
        /// Gets the <see cref="BufferTarget"/> the <see cref="IBuffer"/> will be bound to..
        /// </summary>
        BufferTarget Target { get; }

        /// <summary>
        /// Gets all the data in the buffer.
        /// </summary>
        /// <remarks>
        /// This operation cannot be done asynchronously, thus it may seriously affect performance if called repeatedly!
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> to get the data as.</typeparam>
        /// <returns>The data inside the buffer as structs of the specified <see cref="Type"/>.</returns>
        T[] Get<T>() where T : struct;

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
        T[] GetRange<T>(int offset, int count) where T : struct;

        /// <summary>
        /// Maps the buffer.
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
        IntPtr Map(BufferAccess access);

        /// <summary>
        /// Unmaps the buffer.
        /// </summary>
        /// <remarks>
        /// Expects the current <see cref="IBuffer"/> to be bound.
        /// </remarks>
        void Unmap();

        /// <summary>
        /// Sets the data in the buffer invalidating all previous data.
        /// </summary>
        /// <remarks>
        /// Although the data can be specified in any data type, the data the buffer will receive will be the underlying bytes.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of data to set the buffer to.</typeparam>
        /// <param name="data">The new buffer's data.</param>
        void Set<T>(T data) where T : struct;

        /// <summary>
        /// Sets the data in the buffer invalidating all previous data.
        /// </summary>
        /// <remarks>
        /// Although the data can be specified in any data type, the data the buffer will receive will be the underlying bytes.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of data to set the buffer to.</typeparam>
        /// <param name="data">The new buffer's data.</param>
        void Set<T>(T[] data) where T : struct;

        /// <summary>
        /// Sets a range inside the buffer without invalidating the previous contents.
        /// </summary>
        /// <remarks>
        /// Although the data can be specified in any data type, the data the buffer will receive will be the underlying bytes.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of data to set the buffer to.</typeparam>
        /// <param name="data">The new buffer's data.</param>
        /// <param name="offset">The offset inside the buffer.</param>
        void SetRange<T>(T data, int offset) where T : struct;

        /// <summary>
        /// Sets a range inside the buffer without invalidating the previous contents.
        /// </summary>
        /// <remarks>
        /// Although the data can be specified in any data type, the data the buffer will receive will be the underlying bytes.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of data to set the buffer to.</typeparam>
        /// <param name="data">The new buffer's data.</param>
        /// <param name="offset">The offset inside the buffer.</param>
        void SetRange<T>(T[] data, int offset) where T : struct;

        /// <summary>
        /// Sets the data in the buffer invalidating all previous data.
        /// </summary>
        /// <param name="data">The new buffer's data.</param>
        /// <param name="sizeInBytes">The size of the new data in bytes.</param>
        void Set(IntPtr data, int sizeInBytes);

        /// <summary>
        /// Sets a range inside the buffer without invalidating the previous contents.
        /// </summary>
        /// <param name="data">The new buffer's data.</param>
        /// <param name="offset">The offset inside the buffer.</param>
        /// <param name="sizeInBytes">The size of the new data in bytes.</param>
        void SetRange(IntPtr data, int offset, int sizeInBytes);
    }

    [ContractClassFor(typeof(IBuffer))]
    abstract class IBufferContracts : IBuffer
    {
        int IBuffer.Length
        {
            get
            {
                return 0;
            }
        }

        int IGLObject.Handle
        {
            get
            {
                return 0;
            }
        }

        BufferUsageHint IBuffer.Hint
        {
            get
            {
                return 0;
            }
        }

        bool IInitializable.IsInitialized
        {
            get
            {
                return false;
            }
        }

        BufferTarget IBuffer.Target
        {
            get
            {
                return 0;
            }
        }

        T[] IBuffer.Get<T>() 
        {
            Contract.Ensures(Contract.Result<T[]>() != null);

            return null;
        }

        T[] IBuffer.GetRange<T>(int offset, int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Ensures(Contract.Result<T[]>() != null);

            return null;
        }

        IntPtr IBuffer.Map(BufferAccess access)
        {
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(BufferAccess), access));

            return IntPtr.Zero;
        }

        void IBuffer.Unmap() { }

        void IBuffer.Set<T>(T data) { }

        void IBuffer.Set<T>(T[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Length > 0);
        }

        void IBuffer.SetRange<T>(T data, int offset)
        {
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
        }

        void IBuffer.SetRange<T>(T[] data, int offset)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Length > 0);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
        }

        void IBuffer.Set(IntPtr data, int sizeInBytes)
        {
            Contract.Requires<ArgumentException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(sizeInBytes > 0);
        }

        void IBuffer.SetRange(IntPtr data, int offset, int sizeInBytes)
        {
            Contract.Requires<ArgumentException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(sizeInBytes > 0);
        }

        void IBindable.Bind() { }

        void IInitializable.Initialize() { }

        void IBindable.Unbind() { }

        void IDisposable.Dispose() { }
    }
}
