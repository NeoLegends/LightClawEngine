using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents an <see cref="IComparer{T}"/> that compares it's element in reverse.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of elements to compare.</typeparam>
    public sealed class ReverseComparer<T> : IComparer<T>
    {
        /// <summary>
        /// The underlying <see cref="IComparer{T}"/> used for the actual comparison.
        /// </summary>
        private readonly IComparer<T> inner;

        /// <summary>
        /// Initializes a new <see cref="ReverseComparer{T}"/> using the default comparer.
        /// </summary>
        /// <seealso cref="P:Comparer{T}.Default"/>
        public ReverseComparer() : this(Comparer<T>.Default) { }

        /// <summary>
        /// Initializes a new <see cref="ReverseComparer{T}"/> using the specified <see cref="IComparer{T}"/>.
        /// </summary>
        /// <param name="inner">The <see cref="IComparer{T}"/> performing the actual comparison.</param>
        public ReverseComparer(IComparer<T> inner)
        {
            Contract.Requires<ArgumentNullException>(inner != null);

            this.inner = inner;
        }

        /// <summary>
        /// Compares two instances.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns>An integer representing the relationship of the two operands.</returns>
        public int Compare(T left, T right)
        {
            return inner.Compare(right, left);
        }
    }
}
