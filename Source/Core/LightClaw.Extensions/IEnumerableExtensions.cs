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
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness" is determined via a projection and 
        /// the default comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although a set of already-seen keys is retained.
        /// If a key is seen multiple times, only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence.</typeparam>
        /// <typeparam name="TKey">Type of the projected element.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="selector">Projection for determining "distinctness".</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence, comparing them by the specified key projection.</returns>
        [Pure]
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);
            
            return DistinctBy(source, selector, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness" is determined via a projection and 
        /// the specified comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although a set of already-seen keys is retained.
        /// If a key is seen multiple times, only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence.</typeparam>
        /// <typeparam name="TKey">Type of the projected element.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="selector">Projection for determining "distinctness".</param>
        /// <param name="comparer">
        /// The equality comparer to use to determine whether or not keys are equal.
        /// If null, the default equality comparer for <c>TSource</c> is used.
        /// </param>
        /// <returns>A sequence consisting of distinct elements from the source sequence, comparing them by the specified key projection.</returns>
        [Pure]
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IEqualityComparer<TKey> comparer)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Requires<ArgumentNullException>(comparer != null);

            HashSet<TKey> knownKeys = new HashSet<TKey>(comparer);
            foreach (TSource element in source)
            {
                if (knownKeys.Add(selector(element)))
                {
                    yield return element;
                }
            }
        }

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
        /// Makes sure that the return value is not null and returns an empty enumerable if <paramref name="source"/> was null.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of values inside the <paramref name="source"/>.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns><paramref name="source"/> or an empty enumerable if <paramref name="source"/> was null.</returns>
        [Pure]
        public static IEnumerable<T> EnsureNonNull<T>(this IEnumerable<T> source)
        {
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return source ?? Enumerable.Empty<T>();
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
            Contract.Ensures(Contract.Result<IEnumerable<T>>().All(item => item != null));

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
        /// <returns>The first element that passed the <paramref name="predicate"/> or <c>default(T)</c> if no element passed the test.</returns>
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
        /// Checks whether the specified <paramref name="subset"/> is a subset of the specified <paramref name="superset"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of values in the sets.</typeparam>
        /// <param name="subset">The subset.</param>
        /// <param name="superset">The superset.</param>
        /// <returns><c>true</c> if <paramref name="subset"/> is a subset of <paramref name="superset"/>.</returns>
        [Pure]
        public static bool IsSubsetOf<T>(IEnumerable<T> subset, IEnumerable<T> superset)
        {
            Contract.Requires<ArgumentNullException>(subset != null);
            Contract.Requires<ArgumentNullException>(superset != null);
            
            HashSet<T> hashSet = new HashSet<T>(subset);
            return hashSet.IsSubsetOf(superset);
        }
    }
}
