using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Extensions;

namespace LightClaw.Engine.Core
{
    [DataContract]
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

        public Task Save(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);

            using (FileStream fs = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, resourceString)))
            {
                return this.Save(fs);
            }
        }

        public Task Save(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null);

            return Task.Run(() =>
            {
                this.RaiseSaving();
                new NetDataContractSerializer().WriteObject(s, this);
                this.RaiseSaved();
            });
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

        protected override void OnReset()
        {
            Parallel.ForEach(this.Items, item => item.Reset());
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            Parallel.ForEach(this.Items, item => item.Update(gameTime));
        }

        [OnDeserialized]
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

        public static Task<Scene> LoadFrom(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);

            return LightClawEngine.DefaultIocContainer.Resolve<IContentManager>().LoadAsync<Scene>(resourceString);
        }

        public static async Task<Scene> LoadFrom(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanRead);

            return (Scene)await new SceneReader().ReadAsync("Scene", s, typeof(Scene), null);
        }
    }
}
