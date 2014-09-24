using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    [ContractClass(typeof(IBufferContracts))]
    public interface IBuffer : IBindable, IGLObject
    {
        int Count { get; }

        BufferUsageHint Hint { get; }

        BufferTarget Target { get; }

        T[] Get<T>() where T : struct;

        T[] GetRange<T>(int offset, int count) where T : struct;

        IntPtr Map(BufferAccess access);

        void Unmap();

        void Set<T>(T data) where T : struct;

        void Set<T>(T[] data) where T : struct;

        void SetRange<T>(T data, int offset) where T : struct;

        void SetRange<T>(T[] data, int offset) where T : struct;

        void Set(IntPtr data, int sizeInBytes);

        void SetRange(IntPtr data, int offset, int sizeInBytes);
    }

    [ContractClassFor(typeof(IBuffer))]
    abstract class IBufferContracts : IBuffer
    {
        int IBuffer.Count
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

        void IBindable.Unbind() { }

        void IDisposable.Dispose() { }
    }
}
