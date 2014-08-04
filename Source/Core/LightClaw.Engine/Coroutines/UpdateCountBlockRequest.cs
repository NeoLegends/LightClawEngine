using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Coroutines
{
    /// <summary>
    /// Represents an <see cref="IExecutionBlockRequest"/> with a specified amount of calls to <see cref="M:CanExecute"/>
    /// required before execution is allowed again.
    /// </summary>
    /// <remarks>
    /// This struct can be used to block execution for a specified amount of frames when used together with
    /// a <see cref="CoroutineController"/> calling <see cref="M:ICoroutineContext.Step"/> (which results in
    /// <see cref="M:CanExecute"/> being called) every frame.
    /// </remarks>
    [DataContract, ProtoContract]
    public struct UpdateCountBlockRequest : IExecutionBlockRequest
    {
        /// <summary>
        /// The amount of tries.
        /// </summary>
        [DataMember, ProtoMember(1)]
        private int tries;

        /// <summary>
        /// The required amount of tries to unblock execution again.
        /// </summary>
        [DataMember, ProtoMember(2)]
        public int RequiredTries { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="UpdateCountBlockRequest"/>.
        /// </summary>
        /// <param name="requiredTries">The required amount of tries to unblock execution again.</param>
        public UpdateCountBlockRequest(int requiredTries)
            : this()
        {
            this.RequiredTries = requiredTries;
        }

        /// <summary>
        /// Determines whether the coroutine can be executed again.
        /// </summary>
        /// <returns><c>true</c> if the coroutine can be stepped, otherwise <c>false</c>.</returns>
        public bool CanExecute()
        {
            return (Interlocked.Increment(ref this.tries) > this.RequiredTries);
        }
    }
}
