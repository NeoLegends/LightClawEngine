using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Coroutines
{
    /// <summary>
    /// Represents a request of a coroutine to wait for the next step until the specified time has passed.
    /// </summary>
    [DataContract]
    public struct TimeBlockRequest : IEquatable<TimeBlockRequest>, IExecutionBlockRequest
    {
        /// <summary>
        /// The elapsed time when the <see cref="TimeBlockRequest"/> was created.
        /// </summary>
        [DataMember]
        private readonly long startTicks;

        /// <summary>
        /// The time (in ticks) that should pass until the coroutine can be executed again.
        /// </summary>
        [DataMember]
        public long BlockingTicks { get; private set; }

        /// <summary>
        /// The time (as <see cref="TimeSpan"/>) that should pass until the coroutine can be executed again.
        /// </summary>
        public TimeSpan BlockingTime
        {
            get
            {
                return new TimeSpan(this.BlockingTicks);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="TimeBlockRequest"/> setting the blocking time.
        /// </summary>
        /// <param name="blockingTime">The time that should pass until the coroutine is executed again.</param>
        public TimeBlockRequest(TimeSpan blockingTime)
            : this(blockingTime.Ticks)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="TimeBlockRequest"/> setting the blocking time.
        /// </summary>
        /// <param name="ticks">The amount of ticks the <see cref="TimeBlockRequest"/> blocks execution.</param>
        /// <remarks>
        /// Using the other constructor (accepting a <see cref="TimeSpan"/>) is the preferred way of initializing the
        /// <see cref="TimeBlockRequest"/>. It uses ticks internally for an efficient implementation, but representing
        /// time as <see cref="TimeSpan"/> is much more natural (and readable / maintainable).
        /// </remarks>
        public TimeBlockRequest(long ticks)
            : this()
        {
            this.BlockingTicks = ticks;
            this.startTicks = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Determines whether the coroutine can be executed again.
        /// </summary>
        /// <returns><c>true</c> if the coroutine can be stepped, otherwise <c>false</c>.</returns>
        public bool CanExecute()
        {
            return ((Stopwatch.GetTimestamp() - this.startTicks) > this.BlockingTicks);
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

            return (obj is TimeBlockRequest) ? this.Equals((TimeBlockRequest)obj) : false;
        }

        /// <summary>
        /// Tests for equality with the <paramref name="other"/> specified <see cref="TimeBlockRequest"/>.
        /// </summary>
        /// <param name="other">The <see cref="TimeBlockRequest"/> to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public bool Equals(TimeBlockRequest other)
        {
            return (this.startTicks == other.startTicks) && (this.BlockingTicks == other.BlockingTicks);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.startTicks, this.BlockingTicks);
        }

        /// <summary>
        /// Checks whether the two <see cref="TimeBlockRequest"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="TimeBlockRequest"/>s are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(TimeBlockRequest left, TimeBlockRequest right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether the two <see cref="TimeBlockRequest"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="TimeBlockRequest"/>s are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(TimeBlockRequest left, TimeBlockRequest right)
        {
            return !(left == right);
        }
    }
}
