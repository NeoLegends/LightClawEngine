using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this Collection<T> collection, IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(items != null);

            foreach (T item in items)
            {
                collection.Add(item);
            }
        }
    }
}
