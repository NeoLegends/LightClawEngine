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

        public BufferUsageHint Hint
        {
            get
            {
                return this.BaseBuffer.Hint;
            }
        }

        private int _Index;

        public int Index
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return Math.Max(_Index, 0);
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _Index, value);
            }
        }

        private BufferRange _Range;

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

        private BufferRangeTarget _RangeTarget;

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

        public BufferTarget Target
        {
            get
            {
                return this.BaseBuffer.Target;
            }
        }

        public RangedBuffer(IBuffer baseBuffer, BufferRange range, int index, BufferRangeTarget rangeTarget)
            : base(baseBuffer.Handle)
        {
            Contract.Requires<ArgumentNullException>(baseBuffer != null);

            this.BaseBuffer = baseBuffer;
            this.Range = range;
            this.Index = index;
            this.RangeTarget = rangeTarget;
        }

        public void Bind()
        {
            this.Bind(this.Index);
        }

        public void Bind(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            GL.BindBufferRange(this.RangeTarget, this.Index, this.BaseBuffer.Handle, (IntPtr)this.Range.Start, (IntPtr)this.Range.Length);
        }

        public void Unbind()
        {
            this.Unbind(this.Index);
        }

        public void Unbind(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            GL.BindBufferBase(this.RangeTarget, index, this.BaseBuffer.Handle);
        }

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

        public void Set(IntPtr data, int sizeInBytes)
        {
            this.SetRange(data, 0, sizeInBytes);
        }

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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._BaseBuffer != null);
        }
    }
}
