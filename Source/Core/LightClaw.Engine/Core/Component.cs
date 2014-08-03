using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents the base class of a class in the game hierarchy.
    /// </summary>
    [DataContract(IsReference = true)]
    public abstract class Component : Manager
    {
        /// <summary>
        /// Notifies when the owning <see cref="GameObject"/> changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<GameObject>> GameObjectChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private GameObject _GameObject;

        /// <summary>
        /// The <see cref="GameObject"/> that owns this <see cref="Component"/>.
        /// </summary>
        [IgnoreDataMember]
        public GameObject GameObject
        {
            get
            {
                return _GameObject;
            }
            internal set
            {
                GameObject previous = this.GameObject;
                this.SetProperty(ref _GameObject, value);
                this.Raise(this.GameObjectChanged, value, previous);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Component"/>.
        /// </summary>
        protected Component() { }

        protected override void OnEnable() { }

        protected override void OnDisable() { }

        protected override void OnDraw() { }

        protected override void OnLoad() { }

        protected override void OnReset() { }

        protected override void OnUpdate(GameTime gameTime) { }

        protected override void OnLateUpdate() { }

        /// <summary>
        /// Raises the specified <paramref name="handler"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of value that changed.</typeparam>
        /// <param name="handler">The <see cref="EventHandler{T}"/> to raise.</param>
        /// <param name="newValue">The value of the variable after the change.</param>
        /// <param name="previousValue">The value of the variable before the change.</param>
        protected void Raise<T>(EventHandler<ValueChangedEventArgs<T>> handler, T newValue, T previousValue)
        {
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<T>(newValue, previousValue));
            }
        }
    }
}
