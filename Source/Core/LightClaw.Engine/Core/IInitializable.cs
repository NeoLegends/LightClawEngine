using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Defines a mechanism to initialize an instance after it has been constructed.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Indicates whether the instance has already been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the instance.
        /// </summary>
        /// <remarks>Additional calls to <see cref="M:Initialize"/> shall be ignored.</remarks>
        void Initialize();
    }
}
