using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    public static class StreamExtensions
    {
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
