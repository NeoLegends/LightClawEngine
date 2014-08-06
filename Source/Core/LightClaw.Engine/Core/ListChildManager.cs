﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;

namespace LightClaw.Engine.Core
{
    [DataContract]
    public class ListChildManager<T> : ChildManager<T>, IList<T>, INotifyCollectionChanged
        where T : IControllable, IDrawable
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

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

        public int Count
        {
            get 
            {
                return this.Items.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

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

        public ListChildManager(IEnumerable<T> items)
            : this()
        {
            Contract.Requires<ArgumentNullException>(items != null);

            this.AddRange(items);
        }

        public virtual void Add(T item)
        {
            lock (this.Items)
            {
                this.Items.Add(item);
            }
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);

            foreach (T item in items)
            {
                this.Add(item);
            }
        }

        public virtual void Clear()
        {
            lock (this.Items)
            {
                this.Items.Clear();
            }
        }

        public virtual bool Contains(T item)
        {
            lock (this.Items)
            {
                return this.Items.Contains(item);
            }
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            Contract.Assert(arrayIndex <= (array.Length - this.Count));

            lock (this.Items)
            {
                this.Items.CopyTo(array, arrayIndex);
            }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            lock (this.Items)
            {
                return this.Items.GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual int IndexOf(T item)
        {
            lock (this.Items)
            {
                return this.Items.IndexOf(item);
            }
        }

        public virtual void Insert(int index, T item)
        {
            lock (this.Items)
            {
                this.Items.Insert(index, item);
            }
        }

        public virtual void InsertRange(int index, IEnumerable<T> items)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentNullException>(items != null);
            Contract.Assert(index <= this.Count);

            foreach (T item in items)
            {
                this.Insert(index++, item);
            }
        }

        public virtual bool Remove(T item)
        {
            lock (this.Items)
            {
                return this.Items.Remove(item);
            }
        }

        public virtual void RemoveAt(int index)
        {
            Contract.Assert(index < this.Count);

            lock (this.Items)
            {
                this.Items.RemoveAt(index);
            }
        }

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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Items != null);
        }
    }
}
