using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract(IgnoreListHandling = true)]
    public class GameObject : ListChildManager<Component>
    {
        protected event EventHandler<ValueChangedEventArgs<Scene>> SceneChanged;

        private Scene _Scene;

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
    }
}
