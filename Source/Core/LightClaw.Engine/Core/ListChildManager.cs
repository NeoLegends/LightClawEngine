using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using LightClaw.Extensions;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a <see cref="ChildManager{T}"/> implementing <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/>s of item to store.</typeparam>
    [DataContract]
    public class ListChildManager<T> : ChildManager<T>, IList<T>, INotifyCollectionChanged
        where T : IControllable, IDrawable
    {
        /// <summary>
        /// Notifies about changes in the underlying collection.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets or sets the item at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the item to get or set.</param>
        /// <returns>The item at the specified <paramref name="index"/>.</returns>
        [IgnoreDataMember]
        public virtual T this[int index]
        {
            get
            {
                Contract.Assert(index < this.Count);

                lock (this.Items)
                {
                    return this.Items[index];
                }
            }
            set
            {
                Contract.Assert(index < this.Count);

                lock (this.Items)
                {
                    this.Items[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets the amount of stored items.
        /// </summary>
        public int Count
        {
            get 
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="ListChildManager{T}"/> is readonly.
        /// </summary>
        /// <value><c>false</c></value>
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Initializes a new, empty <see cref="ListChildManager{T}"/>.
        /// </summary>
        public ListChildManager() 
        {
            this.Items.CollectionChanged += (s, e) =>
            {
                NotifyCollectionChangedEventHandler handler = this.CollectionChanged;
                if (handler != null)
                {
                    handler(s, e);
                }
            };
        }

        /// <summary>
        /// Initializes a new <see cref="ListChildManager{T}"/> from a set of items.
        /// </summary>
        /// <param name="items">A set of initial items.</param>
        public ListChildManager(IEnumerable<T> items)
            : this()
        {
            Contract.Requires<ArgumentNullException>(items != null);

            this.AddRange(items);
        }

        /// <summary>
        /// Adds an item to the <see cref="ListChildManager{T}"/>.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public virtual void Add(T item)
        {
            lock (this.Items)
            {
                this.Items.Add(item);
            }
        }

        /// <summary>
        /// Adds a range of items to the <see cref="ListChildManager{T}"/>.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public virtual void AddRange(IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);

            lock (this.Items)
            {
                //foreach (T item in items)
                //{
                //    this.Items.Add(item);
                //}
                this.Items.AddRange(items);
            }
        }

        /// <summary>
        /// Clears out the <see cref="ListChildManager{T}"/> removing all items.
        /// </summary>
        public virtual void Clear()
        {
            lock (this.Items)
            {
                this.Items.Clear();
            }
        }

        /// <summary>
        /// Checks whether the <see cref="ListChildManager{T}"/> contains the specified item.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns><c>true</c> if the <see cref="ListChildManager{T}"/> contains the specified item, otherwise <c>false</c>.</returns>
        public virtual bool Contains(T item)
        {
            lock (this.Items)
            {
                return this.Items.Contains(item);
            }
        }

        /// <summary>
        /// Copies all items in the <see cref="ListChildManager{T}"/> into the specified <paramref name="array"/>.
        /// </summary>
        /// <param name="array">The array to copy the items into.</param>
        /// <param name="arrayIndex">The index to start copying at.</param>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            Contract.Assert(arrayIndex <= (array.Length - this.Count));

            lock (this.Items)
            {
                this.Items.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator{T}"/>.
        /// </summary>
        /// <returns>The <see cref="IEnumerator{T}"/>.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            T[] items;
            lock (this.Items)
            {
                items = this.Items.ToArray();
            }
            return ((IEnumerable<T>)items).GetEnumerator();
        }

        /// <summary>
        /// Gets the <see cref="System.Collections.IEnumerator"/>.
        /// </summary>
        /// <returns>The <see cref="System.Collections.IEnumerator."/></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of the specified item.
        /// </summary>
        /// <param name="item">The index of the specified item.</param>
        /// <returns>The zero-based index of the specified item inside the <see cref="ListChildManager{T}"/> or -1 if the item could not be found.</returns>
        public virtual int IndexOf(T item)
        {
            lock (this.Items)
            {
                return this.Items.IndexOf(item);
            }
        }

        /// <summary>
        /// Inserts the specified item into the <see cref="ListChildManager{T}"/>.
        /// </summary>
        /// <param name="index">The index to start inserting at.</param>
        /// <param name="item">The item to insert.</param>
        public virtual void Insert(int index, T item)
        {
            lock (this.Items)
            {
                this.Items.Insert(index, item);
            }
        }

        /// <summary>
        /// Inserts a range of items into the <see cref="ListChildManager{T}"/>.
        /// </summary>
        /// <param name="index">The index to start inserting at.</param>
        /// <param name="items">The items to insert.</param>
        public virtual void InsertRange(int index, IEnumerable<T> items)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentNullException>(items != null);
            Contract.Assert(index <= this.Count);

            lock (this.Items)
            {
                this.Items.InsertRange(index, items);
            }
        }

        /// <summary>
        /// Removes the specified item from the <see cref="ListChildManager{T}"/>.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if the specified item was removed, otherwise <c>false</c>.</returns>
        public virtual bool Remove(T item)
        {
            lock (this.Items)
            {
                return this.Items.Remove(item);
            }
        }

        /// <summary>
        /// Removes the item at the specified <paramref name="index"/> from the <see cref="ListChildManager{T}"/>.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        public virtual void RemoveAt(int index)
        {
            Contract.Assert(index < this.Count);

            lock (this.Items)
            {
                this.Items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Tries to get the item at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the item to obtain.</param>
        /// <param name="item">If the method succeeds, the item at the specified index.</param>
        /// <returns><c>true</c> if the item could be obtained, otherwise <c>false</c>.</returns>
        public virtual bool TryGetItem(int index, out T item)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            lock (this.Items)
            {
                try
                {
                    item = this.Items[index];
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    item = default(T);
                    return false;   
                }
            }
        }
    }
}
