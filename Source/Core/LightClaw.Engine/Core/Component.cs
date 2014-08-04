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

        /// <summary>
        /// Callback when the <see cref="Component"/> is enabled.
        /// </summary>
        protected override void OnEnable() { }

        /// <summary>
        /// Callback when the <see cref="Component"/> is disabled.
        /// </summary>
        protected override void OnDisable() { }

        /// <summary>
        /// Callback when the <see cref="Component"/> is being drawn.
        /// </summary>
        protected override void OnDraw() { }

        /// <summary>
        /// Callback when the <see cref="Component"/> is loaded.
        /// </summary>
        protected override void OnLoad() { }

        /// <summary>
        /// Callback when the <see cref="Component"/> is reset.
        /// </summary>
        protected override void OnReset() { }

        /// <summary>
        /// Callback when the <see cref="Component"/> is updates with the specified <see cref="GameTime"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        protected override void OnUpdate(GameTime gameTime) { }

        /// <summary>
        /// Callback when the <see cref="Component"/> is late-updated.
        /// </summary>
        protected override void OnLateUpdate() { }
    }
}
