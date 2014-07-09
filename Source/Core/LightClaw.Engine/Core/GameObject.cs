using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract(IgnoreListHandling = true)]
    public class GameObject : ListChildManager<Component>
    {
        public event EventHandler<ValueChangedEventArgs<Scene>> SceneChanged;

        private Scene _Scene;

        [ProtoIgnore]
        public Scene Scene
        {
            get
            {
                return _Scene;
            }
            internal set
            {
                Scene previousValue = this.Scene;
                this.SetProperty(ref _Scene, value);
                EventHandler<ValueChangedEventArgs<Scene>> handler = this.SceneChanged;
                if (handler != null)
                {
                    handler(this, new ValueChangedEventArgs<Scene>(value, previousValue));
                }
            }
        }

        public GameObject() { }

        public GameObject(IEnumerable<Component> components)
        {
            Contract.Requires<ArgumentNullException>(components != null);

            this.Items.AddRange(components);
        }

        [ProtoAfterDeserialization]
        private void InitializeComponents()
        {
            foreach (Component component in this)
            {
                component.GameObject = this;
            }
        }
    }
}
