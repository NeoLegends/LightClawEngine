using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using LightClaw.Extensions;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a subdivison of a <see cref="Scene"/>. Hosts multiple <see cref="Component"/>s.
    /// </summary>
    [DataContract(IsReference = true)]
    public sealed class GameObject : ListChildManager<Component>
    {
        /// <summary>
        /// Notifies about a change in the <see cref="P:Scene"/>.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Scene>> SceneChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private Scene _Scene;

        /// <summary>
        /// The <see cref="Scene"/> the <see cref="GameObject"/> is loaded in.
        /// </summary>
        [IgnoreDataMember]
        public Scene Scene
        {
            get
            {
                return _Scene;
            }
            internal set
            {
                Scene previousValue = this.Scene;
                this.SetProperty(ref _Scene, value);
                this.Raise(this.SceneChanged, value, previousValue);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Component"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to get or set the <see cref="Component"/> at.</param>
        /// <returns></returns>
        public override Component this[int index]
        {
            get
            {
                Contract.Requires<ArgumentOutOfRangeException>(index < this.Count);

                return base[index];
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(index < this.Count);
                Contract.Assume(value != null);

                this.CheckAttachability(value);
                lock (this.Items)
                {
                    this.CheckRemovability(this[index]);
                    base[index] = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GameObject"/>.
        /// </summary>
        public GameObject() : this(new Component[] { }) { }

        /// <summary>
        /// Initializes a new <see cref="GameObject"/> from a set of <see cref="Component"/>s.
        /// </summary>
        /// <param name="components">An initial set of <see cref="Component"/>s to start with.</param>
        public GameObject(params Component[] components)
        {
            Contract.Requires<ArgumentNullException>(components != null);

            this.AddRange(components.FilterNull());
            if (!components.Any(component => component is Transform))
            {
                this.Add(new Transform());
            }
        }

        /// <summary>
        /// Adds the specified <see cref="Component"/> to the <see cref="GameObject"/>.
        /// </summary>
        /// <param name="item">The <see cref="Component"/> to add.</param>
        public override void Add(Component item)
        {
            if (!this.TryAdd(item))
            {
                throw new NotSupportedException("The item could not be added");
            }
        }

        /// <summary>
        /// Adds a range of <see cref="Component"/>s to the <see cref="GameObject"/>.
        /// </summary>
        /// <param name="items">The <see cref="Component"/>s to add.</param>
        public override void AddRange(IEnumerable<Component> items)
        {
            if (!this.TryAddRange(items))
            {
                throw new NotSupportedException("The items could not be added");
            }
        }

        /// <summary>
        /// Removes all <see cref="Component"/>s from the <see cref="GameObject"/> that are removable.
        /// </summary>
        public override void Clear()
        {
            lock (this.Items)
            {
                foreach (Component item in this)
                {
                    if (this.TryCheckRemovability(item))
                    {
                        base.Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="Component"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Component"/> to return.</param>
        /// <returns>The <see cref="Component"/> with the specified name, or <c>null</c> if no <see cref="Component"/> was found.</returns>
        public Component Find(string name)
        {
            return this.FirstOrDefault(component => component.Name == name);
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator{T}"/>.
        /// </summary>
        /// <returns>The <see cref="IEnumerator{T}"/>.</returns>
        public override IEnumerator<Component> GetEnumerator()
        {
            List<Component> items;
            lock (this.Items)
            {
                items = this.Items.ToList();
            }
            return (IEnumerator<Component>)items.GetEnumerator();
        }

        /// <summary>
        /// Inserts the specified <paramref name="item"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="item">The <see cref="Component"/> to insert.</param>
        public override void Insert(int index, Component item)
        {
            if (!this.TryInsert(index, item))
            {
                throw new NotSupportedException("The item could not be inserted.");
            }
        }

        /// <summary>
        /// Inserts a range of items into the <see cref="GameObject"/>.
        /// </summary>
        /// <param name="index">The index to start inserting at.</param>
        /// <param name="items">The items to insert.</param>
        public override void InsertRange(int index, IEnumerable<Component> items)
        {
            if (!this.TryInsertRange(index, items))
            {
                throw new NotSupportedException("The items could not be inserted.");
            }
        }

        /// <summary>
        /// Returns the first <see cref="Component"/> of the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see cref="Component"/> to get.</typeparam>
        /// <returns>The first <see cref="Component"/> of the specified <see cref="Type"/>, or <c>null</c> there was no match.</returns>
        public T OfType<T>()
            where T : Component
        {
            return (T)this.FirstOrDefault(component => component is T);
        }

        /// <summary>
        /// Removes the specified <see cref="Component"/> from the <see cref="GameObject"/>.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if the specified item could be removed, otherwise <c>false</c>.</returns>
        public override bool Remove(Component item)
        {
            lock (this.Items)
            {
                if (this.Contains(item) && this.TryCheckRemovability(item))
                {
                    return base.Remove(item);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Removes the <see cref="Component"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the <see cref="Component"/> to remove.</param>
        public override void RemoveAt(int index)
        {
            if (!this.TryRemoveAt(index))
            {
                throw new NotSupportedException("The item at index {0} could not be removed.".FormatWith(index));
            }
        }

        /// <summary>
        /// Tries to add the specified <see cref="Component"/>.
        /// </summary>
        /// <param name="item">The <see cref="Component"/> to add.</param>
        /// <returns><c>true</c> if the <see cref="Component"/> was added, otherwise <c>false</c>.</returns>
        public bool TryAdd(Component item)
        {
            Contract.Requires<ArgumentNullException>(item != null);

            lock (this.Items)
            {
                if (this.TryCheckAttachability(item))
                {
                    base.Add(item);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Tries to add a range of <see cref="Component"/>s.
        /// </summary>
        /// <param name="item">The <see cref="Component"/>s to add.</param>
        /// <returns><c>true</c> if the <see cref="Component"/>s was added, otherwise <c>false</c>.</returns>
        public bool TryAddRange(IEnumerable<Component> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);

            lock (this.Items)
            {
                if (this.TryCheckAttachability(items))
                {
                    base.AddRange(items);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Tries to insert the specified <see cref="Component"/>.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The item to insert.</param>
        /// <returns><c>true</c> if the item could be inserted, otherwise <c>false</c>.</returns>
        public bool TryInsert(int index, Component item)
        {
            Contract.Requires<ArgumentNullException>(item != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            lock (this.Items)
            {
                if (this.TryCheckAttachability(item))
                {
                    base.Insert(index, item);
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Tries to insert the specified <see cref="Component"/>s.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The items to insert.</param>
        /// <returns><c>true</c> if the items could be inserted, otherwise <c>false</c>.</returns>
        public bool TryInsertRange(int index, IEnumerable<Component> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            lock (this.Items)
            {
                List<Component> attachableComponents = new List<Component>();
                foreach (Component item in items)
                {
                    attachableComponents.Add(item);
                    if (!this.TryCheckAttachability(item, items.Except(attachableComponents)))
                    {
                        return false;
                    }
                }

                base.InsertRange(index, attachableComponents);
                return true;
            }
        }

        /// <summary>
        /// Tries to remove the <see cref="Component"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the <see cref="Component"/> to remove.</param>
        /// <returns><c>true</c> if the <see cref="Component"/> at the specified index was removed, otherwise <c>false</c>.</returns>
        public bool TryRemoveAt(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            lock (this.Items)
            {
                Component itemToRemove;
                if (this.TryGetItem(index, out itemToRemove) && this.TryCheckRemovability(itemToRemove))
                {
                    base.RemoveAt(index);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Ensures that the specified <see cref="Component"/> can be attached.
        /// </summary>
        /// <param name="component">The <see cref="Component"/> that is about to be attached</param>
        /// <exception cref="NotSupportedException">The <paramref name="component"/> cannot be attached.</exception>
        private void CheckAttachability(Component component)
        {
            Contract.Requires<ArgumentNullException>(component != null);

            if (!this.TryCheckAttachability(component))
            {
                throw new NotSupportedException("The component cannot be attached. At least one of the validators was not successful.");
            }
        }

        /// <summary>
        /// Ensures that the specified <see cref="Component"/>s can be attached.
        /// </summary>
        /// <param name="components">The <see cref="Component"/>s that are about to be attached</param>
        /// <exception cref="NotSupportedException">The <paramref name="components"/> cannot be attached.</exception>
        private void CheckAttachability(IEnumerable<Component> components)
        {
            Contract.Requires<ArgumentNullException>(components != null);

            if (!this.TryCheckAttachability(components))
            {
                throw new NotSupportedException("The components cannot be attached. At least one of the validators was not successful.");
            }
        }

        /// <summary>
        /// Ensures attachability of the specified <see cref="Component"/> and allows specification of another set of <see cref="Component"/>s
        /// that are about to be attached in the same transaction.
        /// </summary>
        /// <param name="component">The <see cref="Component"/> that is about to be attached.</param>
        /// <param name="gameObjectsToAttach">Other <see cref="Component"/>s that are about to be attached immediately.</param>
        /// <exception cref="NotSupportedException">The <paramref name="component"/> cannot be attached.</exception>
        private void CheckAttachability(Component component, IEnumerable<Component> gameObjectsToAttach)
        {
            Contract.Requires<ArgumentNullException>(component != null);
            Contract.Requires<ArgumentNullException>(gameObjectsToAttach != null);

            if (!this.TryCheckAttachability(component, gameObjectsToAttach))
            {
                throw new NotSupportedException("The component cannot be attached. At least one of the validators was not successful.");
            }
        }

        /// <summary>
        /// Ensures that the specified <see cref="Component"/> can be removed.
        /// </summary>
        /// <param name="component">The <see cref="Component"/> to remove.</param>
        /// <exception cref="NotSupportedException">The <paramref name="component"/> cannot be removed.</exception>
        private void CheckRemovability(Component component)
        {
            Contract.Requires<ArgumentNullException>(component != null);

            if (!this.TryCheckRemovability(component))
            {
                throw new NotSupportedException("The component cannot be removed. At least one of the validators was not successful.");
            }
        }

        /// <summary>
        /// Tries to ensure that the specified <see cref="Component"/> can be attached.
        /// </summary>
        /// <param name="component">The <see cref="Component"/> that is about to be attached</param>
        /// <returns><c>true</c> if the <paramref name="component"/> can be attached, otherwise <c>false</c>.</returns>
        private bool TryCheckAttachability(Component component)
        {
            Contract.Requires<ArgumentNullException>(component != null);

            return component.GetType().GetCustomAttributes<AttachmentValidatorAttribute>().All(attr => attr.Validate(this));
        }

        /// <summary>
        /// Tries to ensure that the specified <see cref="Component"/>s can be attached.
        /// </summary>
        /// <param name="component">The <see cref="Component"/>s that are about to be attached</param>
        /// <returns><c>true</c> if the <paramref name="components"/> can be attached, otherwise <c>false</c>.</returns>
        private bool TryCheckAttachability(IEnumerable<Component> components)
        {
            Contract.Requires<ArgumentNullException>(components != null);

            List<Component> attachables = new List<Component>();
            foreach (Component item in components)
            {
                attachables.Add(item);
                if (!this.TryCheckAttachability(item, components.Except(attachables)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to ensure attachability of the specified <see cref="Component"/> and allows specification of another set of
        /// <see cref="Component"/>s that are about to be attached in the same transaction.
        /// </summary>
        /// <param name="component">The <see cref="Component"/> that is about to be attached.</param>
        /// <param name="gameObjectsToAttach">Other <see cref="Component"/>s that are about to be attached immediately.</param>
        /// <returns><c>true</c> if the specified <see cref="Component"/> can be attached, otherwise <c>false</c>.</returns>
        private bool TryCheckAttachability(Component component, IEnumerable<Component> gameObjectsToAttach)
        {
            Contract.Requires<ArgumentNullException>(component != null);
            Contract.Requires<ArgumentNullException>(gameObjectsToAttach != null);

            return component.GetType().GetCustomAttributes<AttachmentValidatorAttribute>().All(attr => attr.Validate(this, gameObjectsToAttach));
        }

        /// <summary>
        /// Tries to ensure removability of the specified <see cref="Component"/>.
        /// </summary>
        /// <param name="component">The <see cref="Component"/> that is about to be removed.</param>
        /// <returns><c>true</c> if the <paramref name="component"/> can be removed, otherwise <c>false</c>.</returns>
        private bool TryCheckRemovability(Component component)
        {
            Contract.Requires<ArgumentNullException>(component != null);

            return component.GetType().GetCustomAttributes<RemovalValidatorAttribute>().All(attr => attr.Validate(this));
        }

        /// <summary>
        /// Callback after data contract deserialization initializing all <see cref="Component"/>s.
        /// </summary>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            this.InitializeComponents();
        }

        /// <summary>
        /// Initializes all components setting the reference to the <see cref="GameObject"/>.
        /// </summary>
        private void InitializeComponents()
        {
            foreach (Component component in this)
            {
                component.GameObject = this;
            }
        }
    }
}
