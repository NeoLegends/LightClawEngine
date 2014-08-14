using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public class DataTooLargeException : ArgumentOutOfRangeException
    {
        public int Offset { get; private set; }

        public BufferRange Range { get; private set; }

        public int SizeInBytes { get; private set; }

        public DataTooLargeException() { }

        public DataTooLargeException(string message) : base(message) { }

        public DataTooLargeException(int sizeInBytes, int offset, BufferRange range) : this(null, sizeInBytes, offset, range) { }

        public DataTooLargeException(string message, int sizeInBytes, int offset, BufferRange range)
            : this(message)
        {
            this.SizeInBytes = sizeInBytes;
            this.Offset = offset;
            this.Range = range;
        }

        public DataTooLargeException(string message, Exception inner) : base(message, inner) { }

        public DataTooLargeException(string message, Exception inner, int sizeInBytes, int offset, BufferRange range)
            : this(message, inner)
        {
            this.SizeInBytes = sizeInBytes;
            this.Offset = offset;
            this.Range = range;
        }
    }
}
