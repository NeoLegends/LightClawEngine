using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;

namespace LightClaw.Engine.Core
{
    [DataContract]
    public abstract class Component : Manager
    {
        public event EventHandler<ValueChangedEventArgs<GameObject>> GameObjectChanged;

        private GameObject _GameObject;

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

        protected override void OnEnable() { }

        protected override void OnDisable() { }

        protected override void OnDraw() { }

        protected override void OnLoad() { }

        protected override void OnReset() { }

        protected override void OnUpdate(GameTime gameTime) { }

        protected override void OnLateUpdate() { }

        protected void Raise<T>(EventHandler<ValueChangedEventArgs<T>> handler, T newValue, T previousValue)
        {
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<T>(newValue, previousValue));
            }
        }
    }
}
