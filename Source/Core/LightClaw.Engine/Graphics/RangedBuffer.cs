using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class RangedBuffer : GLObject, IBuffer
    {
        private IBuffer _BaseBuffer;

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

        private BufferRange _BufferRange;

        public BufferRange BufferRange
        {
            get
            {
                return _BufferRange;
            }
            private set
            {
                this.SetProperty(ref _BufferRange, value);
            }
        }

        public BufferUsageHint Hint
        {
            get
            {
                return this.BaseBuffer.Hint;
            }
        }

        public BufferTarget Target
        {
            get
            {
                return this.BaseBuffer.Target;
            }
        }

        public RangedBuffer(IBuffer baseBuffer, BufferRange range)
            : base(baseBuffer.Handle)
        {
            Contract.Requires<ArgumentNullException>(baseBuffer != null);

            this.BaseBuffer = baseBuffer;
            this.BufferRange = range;
        }

        public void Bind()
        {
            this.BaseBuffer.Bind();
        }

        public void Unbind()
        {
            this.BaseBuffer.Unbind();
        }

        public void Set<T>(T data)
            where T : struct
        {
            if (Marshal.SizeOf(typeof(T)) > this.BufferRange.Length)
            {
                throw new DataTooLargeException("The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name));
            }

            this.BaseBuffer.SetRange(data, this.BufferRange.Start);
        }

        public void Set<T>(T[] data)
            where T : struct
        {
            if (Marshal.SizeOf(typeof(T)) * data.Length > this.BufferRange.Length)
            {
                throw new DataTooLargeException("The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name));
            }

            this.BaseBuffer.SetRange(data, this.BufferRange.Start);
        }

        public void SetRange<T>(T data, int offset)
            where T : struct
        {
            if (Marshal.SizeOf(typeof(T)) > this.BufferRange.Length - offset)
            {
                throw new DataTooLargeException("The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name));
            }

            this.BaseBuffer.SetRange(data, this.BufferRange.Start + offset);
        }

        public void SetRange<T>(T[] data, int offset)
            where T : struct
        {
            if (Marshal.SizeOf(typeof(T)) * data.Length > this.BufferRange.Length - offset)
            {
                throw new DataTooLargeException("The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name));
            }

            this.BaseBuffer.SetRange(data, this.BufferRange.Start + offset);
        }

        public void Set(IntPtr data, int sizeInBytes)
        {
            if (sizeInBytes > this.BufferRange.Length)
            {
                throw new DataTooLargeException("The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name));
            }

            this.SetRange(data, 0, sizeInBytes);
        }

        public void SetRange(IntPtr data, int offset, int sizeInBytes)
        {
            if (sizeInBytes > this.BufferRange.Length - offset)
            {
                throw new DataTooLargeException("The data to store does not fit into the specified {0}'s range.".FormatWith(typeof(RangedBuffer).Name));
            }

            this.BaseBuffer.SetRange(data, this.BufferRange.Start + offset, sizeInBytes);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._BaseBuffer != null);
        }
    }
}
