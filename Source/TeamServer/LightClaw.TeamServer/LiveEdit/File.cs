using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LightClaw.TeamServer.Database;

namespace LightClaw.TeamServer.LiveEdit
{
    /// <summary>
    /// Represents a file stored by file path.
    /// </summary>
    public class File : StreamableObject
    {
        /// <summary>
        /// The path to the file on the local server.
        /// </summary>
        public String LocalFileName { get; set; }

        /// <summary>
        /// Initializes a new <see cref="File"/>.
        /// </summary>
        public File() { }

        /// <summary>
        /// Initializes a new <see cref="File"/>.
        /// </summary>
        /// <param name="localFileName">The path to the file on the local server.</param>
        public File(String localFileName) 
        {
            this.LocalFileName = localFileName;
        }

        /// <summary>
        /// Gets a new <see cref="FileStream"/> wrapping the file.
        /// </summary>
        /// <returns>A writable <see cref="FileStream"/> to the file.</returns>
        public override Stream GetStream()
        {
            return new FileStream(this.LocalFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
    }
}
