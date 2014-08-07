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
    [DataContract(IsReference = true)]
    public sealed class GameObject : ListChildManager<Component>
    {
        public event EventHandler<ValueChangedEventArgs<Scene>> SceneChanged;

        private Scene _Scene;

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

                this.EnsureAttachability(value);
                lock (this.Items)
                {
                    this.EnsureRemovability(this[index]);
                    base[index] = value;
                }
            }
        }

        public GameObject() : this(new Component[] { }) { }

        public GameObject(params Component[] components)
        {
            Contract.Requires<ArgumentNullException>(components != null);

            this.AddRange(components);
            if (!components.Any(component => component is Transform))
            {
                this.Add(new Transform());
            }
        }

        public override void Add(Component item)
        {
            this.Insert(this.Count, item);
        }

        public override void AddRange(IEnumerable<Component> items)
        {
            this.InsertRange(this.Count, items);
        }

        public override void Clear()
        {
            lock (this.Items)
            {
                foreach (Component comp in this)
                {
                    this.EnsureRemovability(comp);
                }
                base.Clear();
            }
        }

        public Component Find(string name)
        {
            return this.FirstOrDefault(component => component.Name == name);
        }

        public override void Insert(int index, Component item)
        {
            this.EnsureAttachability(item);

            item.GameObject = this;
            base.Insert(index, item);
        }

        public override void InsertRange(int index, IEnumerable<Component> items)
        {
            items = items.Where(item => item != null);
            foreach (Component item in items)
            {
                this.EnsureAttachability(item);
            }

            List<Component> alreadyAttachedComponents = new List<Component>();
            foreach (Component comp in items)
            {
                alreadyAttachedComponents.Add(comp);
                if (comp.GetType().GetCustomAttributes<AttachmentValidatorAttribute>()
                                  .All(attr => attr.Validate(this, items.Except(alreadyAttachedComponents))))
                {
                    comp.GameObject = this;
                    base.Insert(index++, comp);
                }
            }
        }

        public T OfType<T>()
            where T : Component
        {
            return (T)this.FirstOrDefault(component => component is T);
        }

        public override bool Remove(Component item)
        {
            if (this.Contains(item))
            {
                this.EnsureRemovability(item);

                item.GameObject = null;
                return base.Remove(item);
            }
            else
            {
                return false;
            }
        }

        public override void RemoveAt(int index)
        {
            lock (this.Items)
            {
                if (this[index] != null)
                {
                    this.EnsureRemovability(this[index]);

                    this[index].GameObject = null;
                }
                base.RemoveAt(index);
            }
        }

        public bool TryAdd(Component item)
        {
            Contract.Requires<ArgumentNullException>(item != null);

            try
            {
                this.Add(item);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryAddRange(IEnumerable<Component> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);

            try
            {
                this.AddRange(items);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryInsert(int index, Component item)
        {
            Contract.Requires<ArgumentNullException>(item != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            try
            {
                this.Insert(index, item);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryInsertRange(int index, IEnumerable<Component> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            try
            {
                this.InsertRange(index, items);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void EnsureAttachability(Component component)
        {
            Contract.Requires<ArgumentNullException>(component != null);

            if (!component.GetType().GetCustomAttributes<AttachmentValidatorAttribute>().All(attr => attr.Validate(this)))
            {
                throw new NotSupportedException("The component cannot be attached. At least one of the validators was not successful.");
            }
        }

        private void EnsureAttachability(Component component, IEnumerable<Component> gameObjectsToAttach)
        {
            Contract.Requires<ArgumentNullException>(component != null);
            Contract.Requires<ArgumentNullException>(gameObjectsToAttach != null);

            if (!component.GetType().GetCustomAttributes<AttachmentValidatorAttribute>().All(attr => attr.Validate(this, gameObjectsToAttach)))
            {
                throw new NotSupportedException("The component cannot be attached. At least one of the validators was not successful.");
            }
        }

        private void EnsureRemovability(Component component)
        {
            Contract.Requires<ArgumentNullException>(component != null);

            if (!component.GetType().GetCustomAttributes<RemovalValidatorAttribute>().All(attr => attr.Validate(this)))
            {
                throw new NotSupportedException("The component cannot be removed. At least one of the validators was not successful.");
            }
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            this.InitializeComponents();
        }

        private void InitializeComponents()
        {
            foreach (Component component in this)
            {
                component.GameObject = this;
            }
        }
    }
}
