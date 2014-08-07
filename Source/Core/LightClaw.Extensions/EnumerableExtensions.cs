using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    /// <summary>
    /// Contains extension methods to <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether there are any duplicates in the specified <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of collection.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the element in the collection to check for duplicates.</typeparam>
        /// <param name="collection">The collection to check for duplicates.</param>
        /// <param name="selector">A transformation function that is applied to the <paramref name="collection"/> before it is checked for duplicates.</param>
        /// <returns></returns>
        [Pure]
        public static bool Duplicates<T1, T2>(this IEnumerable<T1> collection, Func<T1, T2> selector)
        {
            Contract.Requires<ArgumentNullException>(selector != null);

            return (collection != null) ? collection.GroupBy(selector).Where(x => x.Skip(1).Any()).Any() : false;
        }

        /// <summary>
        /// Removes a value from a collection.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the collection.</typeparam>
        /// <param name="collection">The collection to remove the value from.</param>
        /// <param name="valueToRemove">The value to remove from the <paramref name="collection"/>.</param>
        /// <returns>The collection without the specified value.</returns>
        [Pure]
        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T valueToRemove)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return collection.GroupBy(s => s).SelectMany(g => g.Key.Equals(valueToRemove) ? g.Skip(1) : g);
        }

        /// <summary>
        /// Removes all elements that are null from the specified collection.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the collection.</typeparam>
        /// <param name="collection">The collection to remove the null-values from.</param>
        /// <returns>The collection without the null-values.</returns>
        [Pure]
        public static IEnumerable<T> FilterNull<T>(this IEnumerable<T> collection)
        {
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<T>>(), t => t != null));

            return (collection != null) ? 
                collection.Where(item => item != null) ?? Enumerable.Empty<T>() : // Enumerable.Empty<T>() required to fulfill contract
                Enumerable.Empty<T>();
        }

        /// <summary>
        /// An asynchronous version of .First().
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the collection.</typeparam>
        /// <param name="collection">The collection to filter.</param>
        /// <param name="predicate">The predicate used to select the element.</param>
        /// <returns>The first element that passed the <paramref name="predicate"/>.</returns>
        /// <exception cref="InvalidOperationException">No task fulfilled the predicate.</exception>
        public static async Task<T> FirstAsync<T>(this IEnumerable<Task<T>> collection, Predicate<Task<T>> predicate)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(predicate != null);

            List<Task<T>> workingCopy = collection.ToList();

            while (workingCopy.Count > 0)
            {
                Task<T> finishedTask = await Task.WhenAny(workingCopy);
                if (predicate(finishedTask))
                {
                    return finishedTask.Result;
                }
                else
                {
                    workingCopy.Remove(finishedTask);
                }
            }

            throw new InvalidOperationException("No task fulfilled the predicate.");
        }

        /// <summary>
        /// An asynchronous version of .FirstOrDefault().
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the collection.</typeparam>
        /// <param name="collection">The collection to filter.</param>
        /// <param name="predicate">The predicate used to select the element.</param>
        /// <returns>The first element that passed the <paramref name="predicate"/> or <c>default(T)</c> if no element passed the test..</returns>
        public static async Task<T> FirstOrDefaultAsync<T>(this IEnumerable<Task<T>> collection, Predicate<Task<T>> predicate)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(predicate != null);

            List<Task<T>> workingCopy = collection.ToList();

            while (workingCopy.Count > 0)
            {
                Task<T> finishedTask = await Task.WhenAny(workingCopy);
                if (predicate(finishedTask))
                {
                    return finishedTask.Result;
                }
                else
                {
                    workingCopy.Remove(finishedTask);
                }
            }

            return default(T);
        }

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
    }
}
