using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents an object wrapping an OpenGL resource through a handle.
    /// </summary>
    public interface IGLObject : IDisposable
    {
        /// <summary>
        /// The handle to the underlying OpenGL resource.
        /// </summary>
        int Handle { get; }
    }
}
