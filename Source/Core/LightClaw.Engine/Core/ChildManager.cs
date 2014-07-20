using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract(IgnoreListHandling = true)]
    [ProtoInclude(100, typeof(ListChildManager<Component>))]
    public abstract class ChildManager<T> : Manager
        where T : IControllable
    {
        private List<T> _Items = new List<T>();

        [ProtoMember(1, DynamicType = true, IsRequired = true)]
        protected virtual List<T> Items
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

        protected ChildManager() { }

        protected ChildManager(IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);

            this.Items.AddRange(items);
        }

        protected override void OnEnable()
        {
            this.PerformChildAction(item => item.Enable());
        }

        protected override void OnDisable()
        {
            this.PerformChildAction(item => item.Disable());
        }

        protected override void OnDraw()
        {
            this.PerformChildAction(item => item.Draw());
        }

        protected override void OnLoad()
        {
            this.PerformChildAction(item => item.Load());
        }

        protected override void OnReset()
        {
            this.PerformChildAction(item => item.Reset());
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            this.PerformChildAction(item => item.Update(gameTime));
        }

        private void PerformChildAction(Action<IControllable> action)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            foreach (T item in this.Items)
            {
                action(item);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Items != null);
        }
    }
}
