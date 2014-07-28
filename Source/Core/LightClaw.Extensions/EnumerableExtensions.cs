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

            return collection.GroupBy(s => s).SelectMany(g => g.Key.Equals(valueToRemove) ? g.Skip(1) : g);
        }

        [Pure]
        public static IEnumerable<T> FilterNull<T>(this IEnumerable<T> collection)
        {
            return (collection != null) ? collection.Where(item => item != null) : Enumerable.Empty<T>();
        }

        [Pure]
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
