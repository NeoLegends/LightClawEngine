using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Threading
{
    public static class DispatcherObjectExtensions
    {
        /// <summary>
        /// Gets whether the specified <paramref name="dispatcherObject"/> may be accessed from the current thread.
        /// </summary>
        /// <param name="dispatcherObject">The <see cref="IDispatcherObject"/> to check.</param>
        /// <returns><c>true</c> if the <see cref="DispatcherObject"/> may be accessed, otherwise <c>false</c>.</returns>
        public static bool CheckAccess(this IDispatcherObject dispatcherObject)
        {
            Contract.Requires<ArgumentNullException>(dispatcherObject != null);

            return ThreadF.IsCurrentThread(dispatcherObject.Dispatcher.Thread);
        }

        /// <summary>
        /// Throws, if the specified <paramref name="dispatcherObject"/> may not be accessed from the calling thread.
        /// </summary>
        /// <param name="dispatcherObject">The <see cref="IDispatcherObject"/> to check.</param>
        [Conditional("DEBUG")]
        public static void VerifyAccess(this IDispatcherObject dispatcherObject)
        {
            Contract.Requires<ArgumentNullException>(dispatcherObject != null);

            ThreadF.ThrowIfNotCurrentThread(dispatcherObject.Dispatcher.Thread);
        }
    }
}
