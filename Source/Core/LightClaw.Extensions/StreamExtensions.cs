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
        public static byte[] ReadExactly(this Stream stream, int count)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Ensures(Contract.Result<byte[]>() != null);

            return ReadExactlyAsync(stream, count).Result;
        }

        public static async Task<byte[]> ReadExactlyAsync(this Stream stream, int count)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Ensures(Contract.Result<Task<byte[]>>() != null);

            byte[] buffer = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int read = await stream.ReadAsync(buffer, offset, count - offset);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }
                offset += read;
            }
            return buffer;
        }
    }
}
