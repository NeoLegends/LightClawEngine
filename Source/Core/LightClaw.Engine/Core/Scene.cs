using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;

namespace LightClaw.Engine.Core
{
    [DataContract(IsReference = true)]
    public class Scene : ListChildManager<GameObject>, IDrawable
    {
        public event EventHandler<ParameterEventArgs> Saving;

        public event EventHandler<ParameterEventArgs> Saved;

        public Scene() 
        {
            logger.Info("Initializing a new scene.");
        }

        public Scene(IEnumerable<GameObject> gameObjects)
            : this()
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

        public async Task Save(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            using (Stream s = await this.IocC.Resolve<IContentManager>().GetStreamAsync(resourceString))
            {
                await this.Save(s);
            }
        }

        public Task Save(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanWrite);

            return this.Save(s, CompressionLevel.Optimal);
        }

        public Task Save(Stream s, CompressionLevel level)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanWrite);

            return Task.Run(() =>
            {
                logger.Info("Saving compressed with level '{0}' scene to a stream.".FormatWith(level));

                using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Saving, this.Saved))
                using (DeflateStream deflateStream = new DeflateStream(s, level, true))
                {
                    new NetDataContractSerializer().WriteObject(deflateStream, this);
                }

                logger.Info("Scene saved.");
            });
        }

        public async Task SaveXml(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            using (Stream s = await this.IocC.Resolve<IContentManager>().GetStreamAsync(resourceString))
            {
                await this.SaveXml(s);
            }
        }

        public Task SaveXml(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanWrite);

            return Task.Run(() =>
            {
                logger.Info("Saving scene as XML to a stream.");

                using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Saving, this.Saved))
                {
                    new NetDataContractSerializer().WriteObject(s, this);
                }

                logger.Info("Scene saved.");
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

        protected override void OnLateUpdate()
        {
            Parallel.ForEach(this.Items, item => item.LateUpdate());
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            this.InitializeGameObjects();
        }

        private void InitializeGameObjects()
        {
            foreach (GameObject gameObject in this)
            {
                gameObject.Scene = this;
            }
        }

        public static Task<Scene> Load(Stream s)
        {
            return Task.Run(() =>
            {
                using (DeflateStream deflateStream = new DeflateStream(s, CompressionMode.Decompress, true))
                {
                    return (Scene)new NetDataContractSerializer().ReadObject(s);
                }
            });
        }

        public static Task<Scene> LoadXml(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanRead);

            return Task.Run(() => (Scene)new NetDataContractSerializer().ReadObject(s));
        }
    }
}
