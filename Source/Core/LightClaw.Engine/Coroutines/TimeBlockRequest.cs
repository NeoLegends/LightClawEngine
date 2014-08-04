using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Coroutines
{
    /// <summary>
    /// Represents a request of a coroutine to wait for the next step until the specified time has passed.
    /// </summary>
    public struct TimeBlockRequest : IExecutionBlockRequest
    {
        /// <summary>
        /// The <see cref="Stopwatch"/> tracking the time that passed since the block started.
        /// </summary>
        private readonly Stopwatch watch;

        /// <summary>
        /// The time that should pass until the coroutine is executed again.
        /// </summary>
        public TimeSpan BlockingTime { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="TimeBlockRequest"/>.
        /// </summary>
        /// <param name="blockingTime">The time that should pass until the coroutine is executed again.</param>
        public TimeBlockRequest(TimeSpan blockingTime)
            : this()
        {
            this.BlockingTime = blockingTime;
            this.watch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Determines whether the coroutine can be executed again.
        /// </summary>
        /// <returns><c>true</c> if the coroutine can be stepped, otherwise <c>false</c>.</returns>
        public bool CanExecute()
        {
            return (this.watch.Elapsed > this.BlockingTime);
        }
    }
}
