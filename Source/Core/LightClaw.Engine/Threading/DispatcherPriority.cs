using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Threading
{
    /// <summary>
    /// Represents the priority the <see cref="Dispatcher"/> sorts its invocation queue with.
    /// </summary>
    public enum DispatcherPriority
    {
        /// <summary>
        /// The task to execute has low priority, it runs in the background.
        /// </summary>
        /// <remarks>Use this for e.g. Dispose-requests that do not need to be executed immediately.</remarks>
        Background = 0,

        /// <summary>
        /// The task has normal priority, it's likely to be executed soon.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// The task has high priority, it will be amongst the first to be executed on the next round.
        /// </summary>
        High = 2,

        /// <summary>
        /// Highest priority, the action will be executed immediately.
        /// </summary>
        Immediate = 3
    }
}
