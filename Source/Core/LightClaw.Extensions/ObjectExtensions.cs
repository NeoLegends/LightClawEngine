using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    /// <summary>
    /// Contains extension methods to <see cref="Object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// "Converts" the specified <paramref name="item"/> into a collection containing the item.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the item.</typeparam>
        /// <param name="item">The item to yield back as <see cref="IEnumerable{T}"/>.</param>
        /// <returns>The item in the newly generated collection.</returns>
        [Pure]
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// "Converts" the specified <paramref name="item"/> into an array containing the item.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the item.</typeparam>
        /// <param name="item">The item to yield back as <see cref="Array"/>.</param>
        /// <returns>The item in the newly generated array.</returns>
        [Pure]
        public static T[] YieldArray<T>(this T item)
        {
            return new[] { item };
        }
    }
}
