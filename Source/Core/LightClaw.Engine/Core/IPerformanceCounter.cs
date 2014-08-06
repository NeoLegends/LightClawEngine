using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a performance counter.
    /// </summary>
    [ContractClass(typeof(IPerformanceCounterContracts))]
    public interface IPerformanceCounter
    {
        /// <summary>
        /// Adds the specified count to the specified counter.
        /// </summary>
        /// <param name="counterName">The name of the counter to add to.</param>
        /// <param name="count">The amount to add.</param>
        int Add(string counterName, int count);

        /// <summary>
        /// Decrements the value of the specified counter.
        /// </summary>
        /// <param name="counterName">The name of the counter to decrement.</param>
        int Decrement(string counterName);

        /// <summary>
        /// Gets the value of the specified counter.
        /// </summary>
        /// <param name="counterName">The name of the counter to obtain the value of.</param>
        /// <returns>The value of the specified counter.</returns>
        int GetValue(string counterName);

        /// <summary>
        /// Increments the value of the specified counter.
        /// </summary>
        /// <param name="counterName">The name of the counter to increment.</param>
        int Increment(string counterName);

        /// <summary>
        /// Resets the specified counter to 0.
        /// </summary>
        /// <param name="counterName">The counter to reset.</param>
        void Reset(string counterName);
    }

    [ContractClassFor(typeof(IPerformanceCounter))]
    abstract class IPerformanceCounterContracts : IPerformanceCounter
    {
        int IPerformanceCounter.Add(string counterName, int count)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(counterName));

            return 0;
        }

        int IPerformanceCounter.Decrement(string counterName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(counterName));

            return 0;
        }

        int IPerformanceCounter.GetValue(string counterName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(counterName));

            return 0;
        }

        int IPerformanceCounter.Increment(string counterName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(counterName));

            return 0;
        }

        void IPerformanceCounter.Reset(string counterName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(counterName));
        }
    }
}
