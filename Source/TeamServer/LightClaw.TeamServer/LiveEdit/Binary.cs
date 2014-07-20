using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LightClaw.Common;
using LightClaw.TeamServer.Database;

namespace LightClaw.TeamServer.LiveEdit
{
    /// <summary>
    /// Represents binary data that is stored directly in the database.
    /// </summary>
    public class Binary : StreamableObject
    {
        /// <summary>
        /// The stored data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Initializes a new <see cref="Binary"/>.
        /// </summary>
        public Binary() { }

        /// <summary>
        /// Creates a new <see cref="Binary"/> from the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to use as data source.</param>
        public Binary(Stream stream)
        {
            MemoryStream memoryStream = stream as MemoryStream;
            if (memoryStream != null)
            {
                this.Data = memoryStream.GetBuffer();
            }
            else
            {
                this.Data = stream.ReadAllBytes();
            }
        }

        /// <summary>
        /// Creates a new <see cref="Binary"/> form the specified <see cref="Array"/>.
        /// </summary>
        /// <param name="data">An <see cref="Array"/> of <see cref="Byte"/>s containing the data to be stored in the database.</param>
        public Binary(byte[] data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets a <see cref="MemoryStream"/> wrapping the contained data.
        /// </summary>
        /// <returns>A <see cref="MemoryStream"/> wrapping the contained binary data.</returns>
        public override Stream GetStream()
        {
            return new MemoryStream(this.Data);
        }
    }
}
