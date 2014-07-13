using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

        public Scene() { }

        public Scene(IEnumerable<GameObject> gameObjects)
        {
            Contract.Requires<ArgumentNullException>(gameObjects != null);

            this.AddRange(gameObjects);
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Add(GameObject item)
        {
            item.Scene = this;
            base.Add(item);
        }

        public override void AddRange(IEnumerable<GameObject> items)
        {
            foreach (GameObject gameObject in items)
            {
                gameObject.Scene = this;
            }
            base.AddRange(items);
        }

        public override void Clear()
        {
            foreach (GameObject gameObject in this)
            {
                gameObject.Scene = null;
            }
            base.Clear();
        }

        public override void Insert(int index, GameObject item)
        {
            item.Scene = this;
            base.Insert(index, item);
        }

        public override void InsertRange(int index, IEnumerable<GameObject> items)
        {
            foreach (GameObject gameObject in items)
            {
                gameObject.Scene = this;
            }
            base.InsertRange(index, items);
        }

        public override bool Remove(GameObject item)
        {
            if (base.Remove(item))
            {
                item.Scene = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void RemoveAt(int index)
        {
            this[index].Scene = null;
            base.RemoveAt(index);
        }

        protected override void OnEnable()
        {
            Parallel.ForEach(this.Items, item => item.Enable());
        }

        protected override void OnDisable()
        {
            Parallel.ForEach(this.Items, item => item.Disable());
        }

        protected override void OnLoad()
        {
            Parallel.ForEach(this.Items, item => item.Load());
        }

        protected override void OnUpdate()
        {
            Parallel.ForEach(this.Items, item => item.Update());
        }

        [ProtoAfterDeserialization]
        private void InitializeGameObjects()
        {
            foreach (GameObject gameObject in this)
            {
                gameObject.Scene = this;
            }
        }

        [ProtoAfterSerialization]
        private void RaiseSaved()
        {
            EventHandler handler = this.Saved;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        [ProtoBeforeSerialization]
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
