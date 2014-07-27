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
        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T valueToRemove)
        {
            Contract.Requires<ArgumentNullException>(collection != null);

            return collection.GroupBy(s => s).SelectMany(g => g.Key.Equals(valueToRemove) ? g.Skip(1) : g);
        }

        public static IEnumerable<T> FilterNull<T>(this IEnumerable<T> collection)
        {
            return collection.Where(item => item != null);
        }

        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
