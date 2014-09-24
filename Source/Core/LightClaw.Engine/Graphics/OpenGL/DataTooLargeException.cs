using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// The exception being thrown when the data to store in a <see cref="RangedBuffer"/> was too large.
    /// </summary>
    [Serializable]
    public class DataTooLargeException : InvalidOperationException
    {
        /// <summary>
        /// The offset of the data to be set inside the <see cref="RangedBuffer"/>'s range.
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// The range the buffer occupies of the underlying buffer.
        /// </summary>
        public BufferRange Range { get; private set; }

        /// <summary>
        /// The size of the data that was supposed to be set.
        /// </summary>
        public int SizeInBytes { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="DataTooLargeException"/>.
        /// </summary>
        public DataTooLargeException() { }

        /// <summary>
        /// Initializes a new <see cref="DataTooLargeException"/> setting the message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DataTooLargeException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new <see cref="DataTooLargeException"/> setting the size of the data to set, the offset and the range.
        /// </summary>
        /// <param name="sizeInBytes">The size of the data that was supposed to be set.</param>
        /// <param name="offset">The offset of the data to be set inside the <see cref="RangedBuffer"/>'s range.</param>
        /// <param name="range">The range the buffer occupies of the underlying buffer.</param>
        public DataTooLargeException(int sizeInBytes, int offset, BufferRange range) : this(null, sizeInBytes, offset, range) { }

        /// <summary>
        /// Initializes a new <see cref="DataTooLargeException"/> setting the message, the size of the data to set, the offset and the range.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="sizeInBytes">The size of the data that was supposed to be set.</param>
        /// <param name="offset">The offset of the data to be set inside the <see cref="RangedBuffer"/>'s range.</param>
        /// <param name="range">The range the buffer occupies of the underlying buffer.</param>
        public DataTooLargeException(string message, int sizeInBytes, int offset, BufferRange range) : this(message, null, sizeInBytes, offset, range) { }

        /// <summary>
        /// Initializes a new <see cref="DataTooLargeException"/> setting the message and the inner exception
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The <see cref="Exception"/> that caused this exception to be thrown.</param>
        public DataTooLargeException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new <see cref="DataTooLargeException"/> setting the message, the inner exception, the size of the data to set,
        /// the offset and the range.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The <see cref="Exception"/> that caused this exception to be thrown.</param>
        /// <param name="sizeInBytes">The size of the data that was supposed to be set.</param>
        /// <param name="offset">The offset of the data to be set inside the <see cref="RangedBuffer"/>'s range.</param>
        /// <param name="range">The range the buffer occupies of the underlying buffer.</param>
        public DataTooLargeException(string message, Exception inner, int sizeInBytes, int offset, BufferRange range)
            : this(message, inner)
        {
            this.SizeInBytes = sizeInBytes;
            this.Offset = offset;
            this.Range = range;
        }
    }
}
