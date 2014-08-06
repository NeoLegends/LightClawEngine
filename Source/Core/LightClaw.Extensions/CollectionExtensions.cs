using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    /// <summary>
    /// Contains extension methods to <see cref="Collection{T}"/>.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds a range of items to the <see cref="Collection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of items to add.</typeparam>
        /// <param name="collection">The <see cref="Collection{T}"/> to add the items to.</param>
        /// <param name="items">The items to add.</param>
        public static void AddRange<T>(this Collection<T> collection, IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(items != null);

            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Inserts a range of items into the <see cref="Collection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of items to insert.</typeparam>
        /// <param name="collection">The <see cref="Collection{T}"/> to insert the items into.</param>
        /// <param name="index">The index to start inserting at.</param>
        /// <param name="items">The items to insert.</param>
        public static void InsertRange<T>(this Collection<T> collection, int index, IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(items != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            foreach (T item in items)
            {
                collection.Insert(index++, item);
            }
        }
    }
}
