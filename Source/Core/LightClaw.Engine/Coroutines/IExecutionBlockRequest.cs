using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Coroutines
{
    /// <summary>
    /// Represents a request to the coroutine context to stop execution until it is allowed to execute again.
    /// </summary>
    public interface IExecutionBlockRequest
    {
        /// <summary>
        /// Determines whether the coroutine can be executed again.
        /// </summary>
        /// <returns><c>true</c> if the coroutine can be stepped, otherwise <c>false</c>.</returns>
        bool CanExecute();
    }
}
