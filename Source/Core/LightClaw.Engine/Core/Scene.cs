using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract(IgnoreListHandling = true)]
    public class Scene : ListChildManager<GameObject>, IDrawable
    {
        public event EventHandler Saving;

        public event EventHandler Saved;

        public void Draw()
        {
            throw new NotImplementedException();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        [ProtoAfterDeserialization]
        private void InitializeGameObjects()
        {
            foreach (GameObject gameObject in this)
            {
                gameObject.Scene = this;
            }
        }

        private void RaiseSaved()
        {
            EventHandler handler = this.Saved;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void RaiseSaving()
        {
            EventHandler handler = this.Saving;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
