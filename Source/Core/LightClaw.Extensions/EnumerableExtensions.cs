using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    public static class EnumerableExtensions
    {
        [Pure]
        public static bool Duplicates<T1, T2>(this IEnumerable<T1> collection, Func<T1, T2> selector)
        {
            Contract.Requires<ArgumentNullException>(selector != null);

            return (collection != null) ? collection.GroupBy(selector).Where(x => x.Skip(1).Any()).Any() : false;
        }

        [Pure]
        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T valueToRemove)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return collection.GroupBy(s => s).SelectMany(g => g.Key.Equals(valueToRemove) ? g.Skip(1) : g);
        }

        [Pure]
        public static IEnumerable<T> FilterNull<T>(this IEnumerable<T> collection)
        {
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return (collection != null) ? 
                collection.Where(item => item != null) ?? Enumerable.Empty<T>() : // Enumerable.Empty<T>() required to fulfill contract
                Enumerable.Empty<T>();
        }

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

        [Pure]
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
