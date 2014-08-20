using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
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
    public sealed class UpdateCountBlockRequest : IExecutionBlockRequest // Class because this is instance is mutable
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

        /// <summary>
        /// Checks whether the <see cref="TimeBlockRequest"/> and the specified object are equal.
        /// </summary>
        /// <param name="obj">The object to check against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is UpdateCountBlockRequest) ? this.Equals((UpdateCountBlockRequest)obj) : false;
        }

        /// <summary>
        /// Tests for equality with the <paramref name="other"/> specified <see cref="UpdateCountBlockRequest"/>.
        /// </summary>
        /// <param name="other">The <see cref="UpdateCountBlockRequest"/> to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public bool Equals(UpdateCountBlockRequest other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(other, this))
                return true;

            return (this.tries == other.tries) && (this.RequiredTries == other.RequiredTries);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.tries, this.RequiredTries);
        }

        /// <summary>
        /// Checks whether the two <see cref="UpdateCountBlockRequest"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="UpdateCountBlockRequest"/>s are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(UpdateCountBlockRequest left, UpdateCountBlockRequest right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether the two <see cref="UpdateCountBlockRequest"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="UpdateCountBlockRequest"/>s are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(UpdateCountBlockRequest left, UpdateCountBlockRequest right)
        {
            return !(left == right);
        }
    }
}
