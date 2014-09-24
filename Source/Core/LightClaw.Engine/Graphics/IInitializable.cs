using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Defines a mechanism to initialize an instance's graphics resources after it has been constructed.
    /// </summary>
    /// <remarks>
    /// This interface allows for initialization of the instance's graphics resources later in time.
    /// Specifically, this is used to be able to load an instance's data into managed memory and pass it off
    /// to OpenGL on the main thread when the resources are required. By loading the data into managed memory
    /// we are able to perform expensive IO and loading operations during the loading screens and then just
    /// push the resources over to GPU memory (which is pretty cheap) when rendering begins.
    /// </remarks>
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
