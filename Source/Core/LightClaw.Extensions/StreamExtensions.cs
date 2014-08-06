using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    /// <summary>
    /// Contains extension methods to <see cref="Stream"/>.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads exactly <paramref name="count"/> bytes from the <see cref="Stream"/>.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> to read from.</param>
        /// <param name="count">The amount of bytes to read.</param>
        /// <returns>The read bytes.</returns>
        /// <exception cref="EndOfStreamException">
        /// The end of the <see cref="Stream"/> was reached before the specified amount of bytes could be read.
        /// </exception>
        public static byte[] ReadExactly(this Stream s, long count)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanRead);

            byte[] result = new byte[count];

            int read = 0;
            int offset = 0;
            long leftToRead = result.LongLength;
            while (leftToRead > 0 && (read = s.Read(result, offset, (leftToRead <= int.MaxValue) ? (int)leftToRead : int.MaxValue)) > 0)
            {
                leftToRead -= read;
                offset += read;
            }
            if (leftToRead > 0)
            {
                throw new EndOfStreamException();
            }

            return result;
        }

        /// <summary>
        /// Asynchronously reads exactly <paramref name="count"/> bytes from the <see cref="Stream"/>.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> to read from.</param>
        /// <param name="count">The amount of bytes to read.</param>
        /// <returns>The read bytes.</returns>
        /// <exception cref="EndOfStreamException">
        /// The end of the <see cref="Stream"/> was reached before the specified amount of bytes could be read.
        /// </exception>
        public static async Task<byte[]> ReadExactlyAsync(this Stream s, long count)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanRead);

            byte[] result = new byte[count];

            int read = 0;
            int offset = 0;
            long leftToRead = result.LongLength;
            while (leftToRead > 0 && (read = await s.ReadAsync(result, offset, (leftToRead <= int.MaxValue) ? (int)leftToRead : int.MaxValue)) > 0)
            {
                leftToRead -= read;
                offset += read;
            }
            if (leftToRead > 0)
            {
                throw new EndOfStreamException();
            }

            return result;
        }
    }
}
