using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Defines a mechanism to update an instance after any main updating has occured.
    /// </summary>
    /// <remarks>
    /// Late-updating is not supposed to alter the game's state anymore, instead shall be used to fetch required values.
    /// Those are guaranteed to stay constant for the rest of the frame and can be used safely.
    /// </remarks>
    public interface ILateUpdateable
    {
        /// <summary>
        /// Notifies about the start of the late updating process.
        /// </summary>
        /// <remarks>Raised before any late updating operations.</remarks>
        event EventHandler<ParameterEventArgs> LateUpdating;

        /// <summary>
        /// Notifies about the finsih of the late updating process.
        /// </summary>
        /// <remarks>Raised after any late updating operations.</remarks>
        event EventHandler<ParameterEventArgs> LateUpdated;

        /// <summary>
        /// Updates the instance.
        /// </summary>
        void LateUpdate();
    }
}
