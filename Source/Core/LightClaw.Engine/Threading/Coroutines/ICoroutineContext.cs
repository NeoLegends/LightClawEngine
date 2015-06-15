using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Threading.Coroutines
{
    /// <summary>
    /// Represents an abstract coroutine context controlling the execution of a single coroutine.
    /// </summary>
    public interface ICoroutineContext
    {
        /// <summary>
        /// Indicates whether the coroutine is currently blocked by an <see cref="IExecutionBlockRequest"/>.
        /// </summary>
        bool IsBlocked { get; }

        /// <summary>
        /// Indicates whether the <see cref="ICoroutineContext"/> is enabled and will step upon request.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Indicates whether the coroutine has finished execution.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Resets the <see cref="ICoroutineContext"/> reprocessing the coroutine.
        /// </summary>
        void Reset();

        /// <summary>
        /// Steps the <see cref="ICoroutineContext"/> returning the current object in the coroutine.
        /// </summary>
        /// <param name="current">The current return value of the coroutine.</param>
        /// <returns><c>true</c> if the coroutine has finished execution, otherwise <c>false</c>.</returns>
        bool Step(out object current);
    }
}
