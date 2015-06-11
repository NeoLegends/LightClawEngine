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
    /// Represents a read-only OpenGL buffer, data in video memory.
    /// </summary>
    [ContractClass(typeof(IReadOnlyBufferContracts))]
    public interface IReadOnlyBuffer : IBindable, IGLObject
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
        /// Binds the buffer.
        /// </summary>
        /// <returns>A <see cref="Binding"/> to release the buffer.</returns>
        Binding Bind();

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
    }

    [ContractClassFor(typeof(IReadOnlyBuffer))]
    abstract class IReadOnlyBufferContracts : IReadOnlyBuffer
    {
        int IReadOnlyBuffer.Length
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

        BufferUsageHint IReadOnlyBuffer.Hint
        {
            get
            {
                return 0;
            }
        }

        BufferTarget IReadOnlyBuffer.Target
        {
            get
            {
                return 0;
            }
        }

        Binding IReadOnlyBuffer.Bind()
        {
            return default(Binding);
        }

        T[] IReadOnlyBuffer.Get<T>()
        {
            Contract.Ensures(Contract.Result<T[]>() != null);

            return null;
        }

        T[] IReadOnlyBuffer.GetRange<T>(int offset, int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Ensures(Contract.Result<T[]>() != null);

            return null;
        }

        void IBindable.Unbind() { }

        void IDisposable.Dispose() { }
    }
}
