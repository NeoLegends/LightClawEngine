using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Coroutines
{
    /// <summary>
    /// Event arguments for the <see cref="E:CoroutineContext.Stepped"/>-event.
    /// </summary>
    public class SteppedEventArgs : EventArgs
    {
        /// <summary>
        /// The current return value of the coroutine.
        /// </summary>
        public object Current { get; private set; }

        /// <summary>
        /// Indicates whether the coroutine has finished execution.
        /// </summary>
        public bool Result { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="SteppedEventArgs"/>.
        /// </summary>
        /// <param name="current">The current return value of the coroutine.</param>
        /// <param name="result"><c>true</c> if the coroutine has finished execution, otherwise <c>false</c>.</param>
        public SteppedEventArgs(object current, bool result)
        {
            this.Current = current;
            this.Result = result;
        }
    }
}
