using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract]
    public abstract class Component : Manager
    {
        public event EventHandler<ValueChangedEventArgs<GameObject>> GameObjectChanged;

        [ProtoMember(1)]
        public int UpdatePriority { get; protected set; }

        private GameObject _GameObject;

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
    }
}
