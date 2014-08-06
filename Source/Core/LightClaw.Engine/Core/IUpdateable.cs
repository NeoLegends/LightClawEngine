using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Defines a mechanism to update an instance.
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// Notifies about the start of the updating process.
        /// </summary>
        /// <remarks>Raised before any updating operations.</remarks>
        event EventHandler<ParameterEventArgs> Updating;

        /// <summary>
        /// Notifies about the finsih of the updating process.
        /// </summary>
        /// <remarks>Raised after any updating operations.</remarks>
        event EventHandler<ParameterEventArgs> Updated;

        /// <summary>
        /// Updates the instance with the specified <see cref="GameTime"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        void Update(GameTime gameTime);
    }
}
