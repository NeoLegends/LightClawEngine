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
    [DataContract]
    public class Scene : ListChildManager<GameObject>, IDrawable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Scene));

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
                logger.Info("Saving scene to a stream.");

                this.Raise(this.Saving);
                using (DeflateStream deflateStream = new DeflateStream(s, CompressionLevel.Optimal, true))
                {
                    new NetDataContractSerializer().WriteObject(deflateStream, this);
                }
                this.Raise(this.Saved);

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
                    return (Scene)new NetDataContractSerializer().ReadObject(deflateStream);
                }
            });
        }
    }
}
