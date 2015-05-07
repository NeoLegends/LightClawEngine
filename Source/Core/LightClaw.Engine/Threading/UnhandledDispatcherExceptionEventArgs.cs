using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Threading
{
    /// <summary>
    /// Event arguments for <see cref="E:Dispatcher.UnhandledException"/>.
    /// </summary>
    public class UnhandledDispatcherExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="Exception"/> that was thrown.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets whether the <see cref="Exception"/> has been handled.
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        /// Initializes a new <see cref="UnhandledDispatcherExceptionEventArgs"/>.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> that was thrown.</param>
        public UnhandledDispatcherExceptionEventArgs(Exception ex)
        {
            Contract.Requires<ArgumentNullException>(ex != null);

            this.Exception = ex;
        }
    }
}
