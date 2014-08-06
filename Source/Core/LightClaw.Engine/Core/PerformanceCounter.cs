using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// The default implementation of <see cref="IPerformanceCounter"/>.
    /// </summary>
    internal class PerformanceCounter : IPerformanceCounter
    {
        /// <summary>
        /// A <see cref="ConcurrentDictionary{TKey, TValue}"/> storing the performance counters.
        /// </summary>
        private readonly ConcurrentDictionary<string, Counter> counters = new ConcurrentDictionary<string, Counter>();

        /// <summary>
        /// Initializes a new <see cref="PerformanceCounter"/>.
        /// </summary>
        public PerformanceCounter() { }

        /// <summary>
        /// Adds the specified count to the specified counter.
        /// </summary>
        /// <param name="counterName">The name of the counter to add to.</param>
        /// <param name="count">The amount to add.</param>
        public int Add(string counterName, int count)
        {
            return this.counters.GetOrAdd(counterName, new Counter()).Add(count);
        }

        /// <summary>
        /// Decrements the value of the specified counter.
        /// </summary>
        /// <param name="counterName">The name of the counter to decrement.</param>
        public int Decrement(string counterName)
        {
            return this.counters.GetOrAdd(counterName, new Counter()).Decrement();
        }

        /// <summary>
        /// Gets the value of the specified counter.
        /// </summary>
        /// <param name="counterName">The name of the counter to obtain the value of.</param>
        /// <returns>The value of the specified counter.</returns>
        public int GetValue(string counterName)
        {
            return this.counters.GetOrAdd(counterName, new Counter()).Value;
        }

        /// <summary>
        /// Increments the value of the specified counter.
        /// </summary>
        /// <param name="counterName">The name of the counter to increment.</param>
        public int Increment(string counterName)
        {
            return this.counters.GetOrAdd(counterName, new Counter()).Increment();
        }

        /// <summary>
        /// Resets the specified counter to 0.
        /// </summary>
        /// <param name="counterName">The counter to reset.</param>
        public void Reset(string counterName)
        {
            Counter c;
            if (this.counters.TryGetValue(counterName, out c) && (c != null))
            {
                c.Reset();
            }
        }

        /// <summary>
        /// The underlying <see cref="Counter"/> wrapping the value.
        /// </summary>
        private class Counter
        {
            /// <summary>
            /// Backing field.
            /// </summary>
            private int _Value;

            /// <summary>
            /// The current value of the counter.
            /// </summary>
            public int Value
            {
                get
                {
                    return _Value;
                }
                private set
                {
                    _Value = value;
                }
            }

            /// <summary>
            /// Initializes a new <see cref="Counter"/>.
            /// </summary>
            public Counter() { }

            /// <summary>
            /// Initializes a new <see cref="Counter"/> from a start count.
            /// </summary>
            /// <param name="count">The <see cref="Counter"/>'s initial value.</param>
            public Counter(int count)
            {
                this.Value = Value;
            }

            /// <summary>
            /// Adds the specified value to the counter.
            /// </summary>
            /// <param name="count">The value to add.</param>
            /// <returns>The <see cref="Counter"/>'s new value.</returns>
            public int Add(int count)
            {
                return Interlocked.Add(ref _Value, count);
            }

            /// <summary>
            /// Decrements the <see cref="Counter"/> once.
            /// </summary>
            /// <returns>The <see cref="Counter"/>'s new value.</returns>
            public int Decrement()
            {
                return Interlocked.Decrement(ref _Value);
            }

            /// <summary>
            /// Increments the <see cref="Counter"/> once.
            /// </summary>
            /// <returns>The <see cref="Counter"/>'s new value.</returns>
            public int Increment()
            {
                return Interlocked.Increment(ref _Value);
            }

            /// <summary>
            /// Resets the <see cref="Counter"/> to zero.
            /// </summary>
            public void Reset()
            {
                this._Value = 0;
            }
        }
    }
}
