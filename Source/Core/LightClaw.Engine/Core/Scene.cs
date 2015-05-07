using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using Newtonsoft.Json;
using NLog;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a layer on the final composed image that is presented to the screen.
    /// </summary>
    [ContentReader(typeof(SceneReader))]
    [DataContract(IsReference = true), JsonObject]
    public class Scene : ListChildManager<GameObject>, IDrawable
    {
        /// <summary>
        /// A static logger instance.
        /// </summary>
        private static readonly Logger staticLogger = LogManager.GetLogger(typeof(Scene).Name);

        /// <summary>
        /// A common <see cref="JsonSerializer"/> used to save / load the <see cref="Scene"/>.
        /// </summary>
        private static readonly JsonSerializer serializer = JsonSerializer.CreateDefault(
            new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                TypeNameHandling = TypeNameHandling.Auto
            }
        );

        /// <summary>
        /// Occurs before the <see cref="Scene"/> is saved.
        /// </summary>
        public event EventHandler<ParameterEventArgs> Saving;

        /// <summary>
        /// Occurs after the <see cref="Scene"/> is saved.
        /// </summary>
        public event EventHandler<ParameterEventArgs> Saved;

        /// <summary>
        /// Notifies about changes in the <see cref="P:SuppressDraw"/>-property.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<bool>> SuppressDrawChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _SuppressDraw;

        /// <summary>
        /// Indicates whether to suppress the drawing of the <see cref="Scene"/>.
        /// </summary>
        /// <remarks>Used for dedicated servers where drawing is not required.</remarks>
        [DataMember]
        public bool SuppressDraw
        {
            get
            {
                return _SuppressDraw;
            }
            set
            {
                bool previous = this.SuppressDraw;
                this.SetProperty(ref _SuppressDraw, value);
                this.Raise(this.SuppressDrawChanged, value, previous);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Scene"/>.
        /// </summary>
        public Scene()
        {
            Log.Info(() => "Initializing a new scene.");
        }

        /// <summary>
        /// Initializes a new <see cref="Scene"/> and sets the <see cref="P:Name"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="Scene"/>.</param>
        public Scene(string name)
            : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new <see cref="Scene"/> from an initial set of <see cref="GameObject"/>s.
        /// </summary>
        /// <param name="gameObjects">A set of <see cref="GameObject"/>s to start with.</param>
        public Scene(params GameObject[] gameObjects)
            : this(null, gameObjects)
        {
            Contract.Requires<ArgumentNullException>(gameObjects != null);
        }

        /// <summary>
        /// Initializes a new <see cref="Scene"/> from an initial set of <see cref="GameObject"/>s and a <paramref name="name"/>.
        /// </summary>
        /// <param name="gameObjects">A set of <see cref="GameObject"/>s to start with.</param>
        /// <param name="name">The name of the <see cref="Scene"/>.</param>
        public Scene(string name, params GameObject[] gameObjects)
            : this(name)
        {
            Contract.Requires<ArgumentNullException>(gameObjects != null);

            this.AddRange(gameObjects);
        }

        /// <summary>
        /// Adds a <see cref="GameObject"/> to the <see cref="Scene"/>.
        /// </summary>
        /// <param name="item">The <see cref="GameObject"/> to add.</param>
        public override void Add(GameObject item)
        {
            if (item != null)
            {
                item.Scene = this;
                base.Add(item);
            }
        }

        /// <summary>
        /// Adds a range of <see cref="GameObject"/>s to the <see cref="Scene"/>.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public override void AddRange(IEnumerable<GameObject> items)
        {
            items = items.FilterNull();
            foreach (GameObject gameObject in items)
            {
                gameObject.Scene = this;
            }
            base.AddRange(items);
        }

        /// <summary>
        /// Clears out the <see cref="Scene"/>.
        /// </summary>
        public override void Clear()
        {
            foreach (GameObject gameObject in this.FilterNull())
            {
                gameObject.Scene = null;
            }
            base.Clear();
        }

        /// <summary>
        /// Inserts a <see cref="GameObject"/> at the specified position into the <see cref="Scene"/>.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The <see cref="GameObject"/> to insert.</param>
        public override void Insert(int index, GameObject item)
        {
            Contract.Assume(item != null);
            item.Scene = this;
            base.Insert(index, item);
        }

        /// <summary>
        /// Inserts a range of items into the <see cref="Scene"/>.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="items">The <see cref="GameObject"/>s to insert.</param>
        public override void InsertRange(int index, IEnumerable<GameObject> items)
        {
            items = items.FilterNull();
            foreach (GameObject gameObject in items)
            {
                gameObject.Scene = this;
            }
            base.InsertRange(index, items);
        }

        /// <summary>
        /// Removes the specified <see cref="GameObject"/> from the <see cref="Scene"/>.
        /// </summary>
        /// <param name="item">The <see cref="GameObject"/> to remove.</param>
        /// <returns></returns>
        public override bool Remove(GameObject item)
        {
            if (base.Remove(item) && item != null)
            {
                item.Scene = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the <see cref="GameObject"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        public override void RemoveAt(int index)
        {
            GameObject oldItem = this[index];
            if (oldItem != null)
            {
                oldItem.Scene = null;
            }
            base.RemoveAt(index);
        }

        /// <summary>
        /// Asynchronously saves the <see cref="Scene"/> with optimal compression to the specified resource string.
        /// </summary>
        /// <param name="resourceString">The resource string to save to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous saving process.</returns>
        public async Task SaveAsync(ResourceString resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            using (Stream s = await this.IocC.Resolve<IContentManager>().GetStreamAsync(resourceString))
            {
                await this.SaveAsync(s);
            }
        }

        /// <summary>
        /// Asynchronously saves the <see cref="Scene"/> with optimal compression to the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> to save to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous saving process.</returns>
        public Task SaveAsync(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanWrite);

            return this.SaveAsync(s, CompressionLevel.Optimal);
        }

        /// <summary>
        /// Asynchronously saves the <see cref="Scene"/> to the specified <see cref="Stream"/> and while using the specified 
        /// <see cref="CompressionLevel"/>.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> to save to.</param>
        /// <param name="level">A <see cref="CompressionLevel"/> indicating how strong to compress the <see cref="Scene"/>-file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous saving process.</returns>
        public async Task SaveAsync(Stream s, CompressionLevel level)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanWrite);

            Log.Info(() => "Saving scene in a compressed format by redirecting scene data to a compression stream (level '{0}')...".FormatWith(level));

            using (DeflateStream deflateStream = new DeflateStream(s, level, true))
            {
                await this.SaveRawAsync(deflateStream);
            }
        }

        /// <summary>
        /// Asynchronously saves the <see cref="Scene"/> to the specified resource string without compression.
        /// </summary>
        /// <param name="resourceString">The resource string to save to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous saving process.</returns>
        public async Task SaveRawAsync(ResourceString resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            using (Stream s = await this.IocC.Resolve<IContentManager>().GetStreamAsync(resourceString))
            {
                await this.SaveRawAsync(s);
            }
        }

        /// <summary>
        /// Asynchronously saves the <see cref="Scene"/> to the specified <see cref="Stream"/> without compression.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> to save to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous saving process.</returns>
        public Task SaveRawAsync(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanWrite);

            return Task.Run(() =>
            {
                Log.Info(() => "Saving scene to a stream.");

                using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Saving, this.Saved))
                using (StreamWriter sw = new StreamWriter(s, Encoding.UTF8, 1024, true))
                using (JsonTextWriter jtw = new JsonTextWriter(sw) {  CloseOutput = false })
                {
                    serializer.Serialize(jtw, this);
                    //new NetDataContractSerializer().WriteObject(s, this); // Check whether we can switch to Json.NET here
                }

                Log.Info(() => "Scene saved.");
            });
        }

        /// <summary>
        /// Implementation of <see cref="M:Enable"/>.
        /// </summary>
        protected override void OnEnable()
        {
            Parallel.ForEach(this.Items, item => item.Enable());
        }

        /// <summary>
        /// Implementation of <see cref="M:Disable"/>.
        /// </summary>
        protected override void OnDisable()
        {
            Parallel.ForEach(this.Items, item => item.Disable());
        }

        /// <summary>
        /// Overriden implementation of <see cref="M:Draw"/> with the possibility to suppress the drawing process.
        /// </summary>
        protected override void OnDraw()
        {
            if (!this.SuppressDraw)
            {
                base.OnDraw();
            }
        }

        /// <summary>
        /// Implementation of <see cref="M:Load"/>.
        /// </summary>
        protected override void OnLoad()
        {
            Parallel.ForEach(this.Items, item => item.Load());
        }

        /// <summary>
        /// Implementation of <see cref="M:Reset"/>.
        /// </summary>
        protected override void OnReset()
        {
            Parallel.ForEach(this.Items, item => item.Reset());
        }

        /// <summary>
        /// Implementation of <see cref="M:Update"/>.
        /// </summary>
        protected override bool OnUpdate(GameTime gameTime, int pass)
        {
            bool result = true;
            Parallel.ForEach(this.Items, item => result &= item.Update(gameTime, pass));
            return result;
        }

        /// <summary>
        /// Being called after deserialization from data contract serializers.
        /// </summary>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            this.InitializeGameObjects();
        }

        /// <summary>
        /// Initializes the <see cref="GameObject"/>s setting the reference to the <see cref="Scene"/>.
        /// </summary>
        private void InitializeGameObjects()
        {
            foreach (GameObject gameObject in this.FilterNull())
            {
                gameObject.Scene = this;
            }
        }

        /// <summary>
        /// Asynchronously loads a compressed <see cref="Scene"/> from the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> to load from.</param>
        /// <returns>The loaded <see cref="Scene"/>.</returns>
        public static async Task<Scene> Load(Stream s)
        {
            using (DeflateStream deflateStream = new DeflateStream(s, CompressionMode.Decompress, true))
            {
                return await LoadRaw(deflateStream);
            }
        }

        /// <summary>
        /// Asynchronously loads an uncompressed <see cref="Scene"/> from the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> to load from.</param>
        /// <returns>The loaded <see cref="Scene"/>.</returns>
        public static Task<Scene> LoadRaw(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanRead);

            Task<Scene> sceneLoadingTask = Task.Run(() =>
            {
                staticLogger.Info("Loading a {0} from a stream.".FormatWith(typeof(Scene).Name));

                using (StreamReader sr = new StreamReader(s, Encoding.UTF8, true, 1024, true))
                using (JsonTextReader jtr = new JsonTextReader(sr) { CloseInput = false })
                {
                    return serializer.Deserialize<Scene>(jtr);
                }
            });
            sceneLoadingTask.ContinueWith(
                t =>
                {
                    staticLogger.Info(() => "Scene '{0}' loaded.".FormatWith((t.Result != null) ? t.Result.Name : "N/A"));
                    return t.Result;
                }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously
            );
            sceneLoadingTask.ContinueWith(
                t => staticLogger.Warn("Scene loading failed.", t.Exception), 
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
            );
            return sceneLoadingTask;
        }
    }
}
