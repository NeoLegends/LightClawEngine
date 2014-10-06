using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Defines a mechanism to control an item in the scene hierarchy.
    /// </summary>
    public interface IControllable : IDisposable, ILateUpdateable, IUpdateable
    {
        /// <summary>
        /// Notifies about the start of the enabling process.
        /// </summary>
        /// <remarks>Raised before any enabling operations.</remarks>
        event EventHandler<ParameterEventArgs> Enabling;

        /// <summary>
        /// Notifies about the end of the enabling process.
        /// </summary>
        /// <remarks>Raised after any enabling operations.</remarks>
        event EventHandler<ParameterEventArgs> Enabled;

        /// <summary>
        /// Notifies about the start of the disabling process.
        /// </summary>
        /// <remarks>Raised before any disabling operations.</remarks>
        event EventHandler<ParameterEventArgs> Disabling;

        /// <summary>
        /// Notifies about the end of the disabling process.
        /// </summary>
        /// <remarks>Raised after any disabling operations.</remarks>
        event EventHandler<ParameterEventArgs> Disabled;

        /// <summary>
        /// Notifies about the start of the loading process.
        /// </summary>
        /// <remarks>Raised before any loading operations.</remarks>
        event EventHandler<ParameterEventArgs> Loading;

        /// <summary>
        /// Notifies about the end of the loading process.
        /// </summary>
        /// <remarks>Raised after any loading operations.</remarks>
        event EventHandler<ParameterEventArgs> Loaded;

        /// <summary>
        /// Notifies about the start of the resetting process.
        /// </summary>
        /// <remarks>Raised before any resetting operations.</remarks>
        event EventHandler<ParameterEventArgs> Resetting;

        /// <summary>
        /// Notifies about the end of the resetting process.
        /// </summary>
        /// <remarks>Raised after any resetting operations.</remarks>
        event EventHandler<ParameterEventArgs> Resetted;

        /// <summary>
        /// Indicates whether the instance is enabled or not.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Indicates whether the instance is loaded or not.
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Enables the instance.
        /// </summary>
        /// <remarks>Calls to <see cref="M:Enable"/> shall be ignored if the instance is already enabled.</remarks>
        void Enable();

        /// <summary>
        /// Disables the instance
        /// </summary>
        /// <remarks>Calls to <see cref="M:Disable"/> shall be ignored if the instance is already disabled.</remarks>
        void Disable();

        /// <summary>
        /// Loads the instance.
        /// </summary>
        /// <remarks>
        /// Preferrably, loading will be done asynchronous and calls to other methods shall be ignored until loading is done.
        ///
        /// Calls to <see cref="M:Load"/> shall be ignored if the instance is already loaded.
        /// </remarks>
        void Load();

        /// <summary>
        /// Resets the instance to the default values.
        /// </summary>
        /// <example>
        /// <see cref="Transform"/> resets its local position / rotation / scaling to zero / identity / one.
        /// </example>
        void Reset();
    }
}
