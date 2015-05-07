using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a <see cref="Manager"/> capable of routing calls to its children.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of children to manage.</typeparam>
    [DataContract]
    [ThreadMode(true)]
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
        protected ChildManager()
        {
        }

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
            foreach (T item in this.Items)
            {
                item.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Callback enabling all children.
        /// </summary>
        protected override void OnEnable()
        {
            foreach (T item in this.Items)
            {
                item.Enable();
            }
        }

        /// <summary>
        /// Callback disabling all children.
        /// </summary>
        protected override void OnDisable()
        {
            foreach (T item in this.Items)
            {
                item.Disable();
            }
        }

        /// <summary>
        /// Callback drawing all children.
        /// </summary>
        protected override void OnDraw()
        {
            foreach (T item in this.Items)
            {
                item.Draw();
            }
        }

        /// <summary>
        /// Callback loading all children.
        /// </summary>
        protected override void OnLoad()
        {
            foreach (T item in this.Items)
            {
                item.Load();
            }
        }

        /// <summary>
        /// Callback resetting all children.
        /// </summary>
        protected override void OnReset()
        {
            foreach (T item in this.Items)
            {
                item.Reset();
            }
        }

        /// <summary>
        /// Callback updating the children with the specified <see cref="GameTime"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        protected override bool OnUpdate(GameTime gameTime, int pass)
        {
            bool result = true;
            foreach (T item in this.Items)
            {
                result &= item.Update(gameTime, pass);
            }
            return result;
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
