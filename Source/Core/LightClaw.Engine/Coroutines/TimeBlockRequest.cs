using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Coroutines
{
    /// <summary>
    /// Represents a request of a coroutine to wait for the next step until the specified time has passed.
    /// </summary>
    [DataContract, ProtoContract]
    public struct TimeBlockRequest : IExecutionBlockRequest
    {
        /// <summary>
        /// The elapsed time when the <see cref="TimeBlockRequest"/> was created.
        /// </summary>
        [DataMember, ProtoMember(1)]
        private readonly long startTicks;

        /// <summary>
        /// The time (in ticks) that should pass until the coroutine can be executed again.
        /// </summary>
        [DataMember, ProtoMember(2)]
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
        public TimeBlockRequest(TimeSpan blockingTime) : this(blockingTime.Ticks) { }

        /// <summary>
        /// Initializes a new <see cref="TimeBlockRequest"/> setting the blocking time.
        /// </summary>
        /// <param name="ticks">The amount of ticks the <see cref="TimeBlockRequest"/> blocks execution.</param>
        /// <remarks>
        /// Using the other constructor ´(accepting a <see cref="TimeSpan"/>) is the preferred way of initializing the
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
    }
}
