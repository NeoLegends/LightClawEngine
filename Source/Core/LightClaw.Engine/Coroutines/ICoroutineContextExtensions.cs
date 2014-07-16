using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Coroutines
{
    /// <summary>
    /// Contains extensions to <see cref="ICoroutineContext"/>s.
    /// </summary>
    public static class ICoroutineContextExtensions
    {
        /// <summary>
        /// Steps the <see cref="ICoroutineContext"/> once without returning the current object.
        /// </summary>
        /// <param name="context">The <see cref="ICoroutineContext"/> to step.</param>
        /// <returns><c>true</c> if the context finished execution, otherwise <c>false</c>.</returns>
        public static bool Step(this ICoroutineContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null);

            object current;
            return context.Step(out current);
        }

        /// <summary>
        /// Steps the <see cref="ICoroutineContext"/> until the specified <paramref name="timeOut"/>.
        /// </summary>
        /// <param name="context">The <see cref="ICoroutineContext"/> to step.</param>
        /// <param name="timeOut">The time to step.</param>
        /// <returns><c>true</c> if the <see cref="ICoroutineContext"/> finished execution during the specified time, otherwise <c>false</c>.</returns>
        public static bool StepUntil(this ICoroutineContext context, TimeSpan timeOut)
        {
            Contract.Requires<ArgumentNullException>(context != null);

            Stopwatch st = Stopwatch.StartNew();
            bool isFinished = false;
            while (!isFinished && (st.Elapsed < timeOut) && (isFinished = context.Step())) { }
            return isFinished;
        }
    }
}
