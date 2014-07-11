using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract(IgnoreListHandling = true)]
    public sealed class GameObject : ListChildManager<Component>
    {
        public event EventHandler<ValueChangedEventArgs<Scene>> SceneChanged;

        private Scene _Scene;

        [ProtoIgnore]
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
                EventHandler<ValueChangedEventArgs<Scene>> handler = this.SceneChanged;
                if (handler != null)
                {
                    handler(this, new ValueChangedEventArgs<Scene>(value, previousValue));
                }
            }
        }

        public GameObject() { }

        public GameObject(IEnumerable<Component> components)
        {
            Contract.Requires<ArgumentNullException>(components != null);

            this.AddRange(components);
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
            foreach (Component comp in this.Items)
            {
                comp.GameObject = null;
                base.Remove(comp);
            }
            this.Items.Clear();
        }

        public override void Insert(int index, Component item)
        {
            if (item.GetType().GetCustomAttributes<AttachmentValidatorAttribute>().All(attr => attr.Validate(this)))
            {
                item.GameObject = this;
                base.Insert(index, item);
            }
        }

        public override void InsertRange(int index, IEnumerable<Component> items)
        {
            List<Component> alreadyAttachedComponents = new List<Component>(8);
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

        public override bool Remove(Component item)
        {
            if (item.GetType().GetCustomAttributes<RemovalValidatorAttribute>().All(attr => attr.Validate(this)) && base.Remove(item))
            {
                item.GameObject = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void RemoveAt(int index)
        {
            if (this[index].GetType().GetCustomAttributes<RemovalValidatorAttribute>().All(attr => attr.Validate(this)))
            {
                this[index].GameObject = null;
                base.RemoveAt(index);
            }
        }

        [ProtoAfterDeserialization]
        private void InitializeComponents()
        {
            foreach (Component component in this)
            {
                component.GameObject = this;
            }
        }
    }
}
