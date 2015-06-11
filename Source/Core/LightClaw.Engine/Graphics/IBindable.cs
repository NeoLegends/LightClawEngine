using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Defines a mechanism for binding a specific object to the graphics pipeline.
    /// </summary>
    public interface IBindable
    {
        /// <summary>
        /// Unbinds the element from the graphics pipeline.
        /// </summary>
        void Unbind();
    }
}
