using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Defines a mechanism for drawing an object to the currently active screen / camera.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Notifies about the start of the drawing process.
        /// </summary>
        /// <remarks>Raised before any binding / drawing occurs.</remarks>
        event EventHandler<ParameterEventArgs> Drawing;

        /// <summary>
        /// Notifies about the finish of the drawing process.
        /// </summary>
        /// <remarks>Raised after any binding / drawing operations.</remarks>
        event EventHandler<ParameterEventArgs> Drawn;

        /// <summary>
        /// Triggers binding and drawing of the element.
        /// </summary>
        void Draw();
    }
}
