using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract(IgnoreListHandling = true)]
    [ProtoInclude(100, typeof(Transform)), ProtoInclude(101, typeof(Coroutines.CoroutineController))]
    [ProtoInclude(102, typeof(Graphics.Camera))]
    public abstract class Component : Manager
    {
        public event EventHandler<ValueChangedEventArgs<GameObject>> GameObjectChanged;

        [ProtoMember(1)]
        public int UpdatePriority { get; protected set; }

        private GameObject _GameObject;

        [ProtoIgnore]
        public GameObject GameObject
        {
            get
            {
                return _GameObject;
            }
            internal set
            {
                GameObject previousValue = this.GameObject;
                this.SetProperty(ref _GameObject, value);
                EventHandler<ValueChangedEventArgs<GameObject>> handler = this.GameObjectChanged;
                if (handler != null)
                {
                    handler(this, new ValueChangedEventArgs<GameObject>(value, previousValue));
                }
            }
        }

        protected override void OnEnable() { }

        protected override void OnDisable() { }

        protected override void OnLoad() { }

        protected override void OnReset() { }

        protected override void OnUpdate(GameTime gameTime) { }
    }
}
