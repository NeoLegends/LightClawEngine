using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract]
    public abstract class ChildEntity<T> : Entity
        where T : IControllable
    {
        private IEnumerable<T> _Items = new List<T>();

        [ProtoMember(2, DynamicType = true)]
        protected virtual IEnumerable<T> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                _Items = value;
            }
        }

        protected ChildEntity() { }

        protected ChildEntity(IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);

            Contract.Assume(this.Items != null);
            ((List<T>)this.Items).AddRange(items);
        }

        protected override void OnLoad()
        {
            this.PerformChildAction(item => item.Load());
        }

        protected override void OnShutdown()
        {
            this.PerformChildAction(item => item.Unload());
        }

        protected override void OnUpdate()
        {
            this.PerformChildAction(item => item.Update());
        }

        private void PerformChildAction(Action<IControllable> action)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            IEnumerable<T> items = this.Items;
            if (items != null)
            {
                lock (items)
                {
#pragma warning disable 0728
                    items = items.ToArray();
#pragma warning restore 0728
                }
                foreach (IControllable item in items)
                {
                    action(item);
                }
            }
        }
    }
}
