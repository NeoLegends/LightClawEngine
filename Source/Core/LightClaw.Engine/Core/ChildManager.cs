using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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
    /// Represents a <see cref="Manager"/> capable of routing calls to its children.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of children to manage.</typeparam>
    [DataContract]
    public abstract class ChildManager<T> : Manager
        where T : IControllable, IDrawable
    {
        /// <summary>
        /// Backing field.
        /// </summary>
        private ObservableCollection<T> _Items = new ObservableCollection<T>();

        /// <summary>
        /// All attached children.
        /// </summary>
        [DataMember]
        protected virtual ObservableCollection<T> Items
        {
            get
            {
                Contract.Ensures(Contract.Result<ObservableCollection<T>>() != null);

                return _Items;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                _Items = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="ChildManager{T}"/>.
        /// </summary>
        protected ChildManager() { }

        /// <summary>
        /// Initializes a new <see cref="ChildManager{T}"/> using a collection of children.
        /// </summary>
        /// <param name="items"></param>
        protected ChildManager(IEnumerable<T> items)
        {
            Contract.Requires<ArgumentNullException>(items != null);

            this.Items.AddRange(items);
        }

        /// <summary>
        /// Disposes the <see cref="ChildManager{T}"/> disposing all of its children.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            this.PerformChildAction(item => item.Dispose());
            base.Dispose(disposing);
        }

        /// <summary>
        /// Callback enabling all children.
        /// </summary>
        protected override void OnEnable()
        {
            this.PerformChildAction(item => item.Enable());
        }

        /// <summary>
        /// Callback disabling all children.
        /// </summary>
        protected override void OnDisable()
        {
            this.PerformChildAction(item => item.Disable());
        }

        /// <summary>
        /// Callback drawing all children.
        /// </summary>
        protected override void OnDraw()
        {
            this.PerformChildAction(item => item.Draw());
        }

        /// <summary>
        /// Callback loading all children.
        /// </summary>
        protected override void OnLoad()
        {
            this.PerformChildAction(item => item.Load());
        }

        /// <summary>
        /// Callback resetting all children.
        /// </summary>
        protected override void OnReset()
        {
            this.PerformChildAction(item => item.Reset());
        }

        /// <summary>
        /// Callback updating the children with the specified <see cref="GameTime"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        protected override void OnUpdate(GameTime gameTime)
        {
            this.PerformChildAction(item => item.Update(gameTime));
        }

        /// <summary>
        /// Callback triggering the late update in all children.
        /// </summary>
        protected override void OnLateUpdate()
        {
            this.PerformChildAction(item => item.LateUpdate());
        }

        /// <summary>
        /// Executes the specified <see cref="Action{T}"/> for all children.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> to execute for all children.</param>
        private void PerformChildAction(Action<T> action)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            ObservableCollection<T> items = this.Items;
            if (items != null)
            {
                foreach (T item in items)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Items != null);
        }
    }
}
