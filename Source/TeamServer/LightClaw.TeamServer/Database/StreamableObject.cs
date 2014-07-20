using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LightClaw.TeamServer.Database
{
    /// <summary>
    /// Represents an entry in the MongoDB which is capable of returning a <see cref="Stream"/> of entry data.
    /// </summary>
    public abstract class StreamableObject : MongoObject
    {
        /// <summary>
        /// Returns a <see cref="Stream"/> wrapping instance data.
        /// </summary>
        /// <returns>A <see cref="Stream"/> wrapping instance data.</returns>
        public abstract Stream GetStream();
    }
}
