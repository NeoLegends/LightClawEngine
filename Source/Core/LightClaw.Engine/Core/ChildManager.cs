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
    public abstract class ChildManager<T> : Manager
        where T : IControllable
    {
        private ObservableCollection<T> _Items = new ObservableCollection<T>();

        [ProtoMember(2, DynamicType = true)]
        protected virtual ObservableCollection<T> Items
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

            foreach (T item in items)
            {
                this.Items.Add(item);
            }
        }

        protected override void OnEnable()
        {
            this.PerformChildAction(item => item.Enable());
        }

        protected override void OnDisable()
        {
            this.PerformChildAction(item => item.Disable());
        }

        protected override void OnLoad()
        {
            this.PerformChildAction(item => item.Load());
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
                foreach (T item in items)
                {
                    action(item);
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
