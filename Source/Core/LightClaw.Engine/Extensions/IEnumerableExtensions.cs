using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(action != null);

            foreach (T item in collection)
            {
                action(item);
            }
            return collection;
        }

        public static IEnumerable<T> ForEachLazy<T>(this IEnumerable<T> collection, Action<T> action)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(action != null);

            foreach (T item in collection)
            {
                action(item);
                yield return item;
            }
        }
    }
}
