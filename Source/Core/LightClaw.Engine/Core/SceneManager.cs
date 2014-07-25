using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace LightClaw.Engine.Core
{
    internal class SceneManager : Manager, ISceneManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SceneManager));

        private readonly SortedDictionary<int, Scene> scenes = new SortedDictionary<int, Scene>();

        private readonly List<Scene> workingCopy = new List<Scene>();

        private string _StartScene;

        public string StartScene
        {
            get
            {
                return _StartScene;
            }
            private set
            {
                this.SetProperty(ref _StartScene, value);
            }
        }

        public SceneManager(string startScene)
        {
            Contract.Requires<ArgumentNullException>(startScene != null);

            logger.Info("Initializing scene manager.");
            this.StartScene = startScene;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<Scene> GetEnumerator()
        {
            Scene[] scenes;
            lock (this.scenes)
            {
                scenes = this.scenes.Values.ToArray();
            }
            return (IEnumerator<Scene>)scenes.GetEnumerator();
        }

        public void Move(int index, int newIndex)
        {
            lock (this.scenes)
            {
                Scene scene;
                if (this.scenes.TryGetValue(index, out scene))
                {
                    this.scenes.Remove(index);
                    this.scenes.Add(newIndex, scene);
                }
            }
        }

        public async Task<bool> Load(int index, string resourceString)
        {
            Scene s = await Scene.LoadFrom(resourceString);
            s.Load();
            return this.Load(index, s);
        }

        public bool Load(int index, Scene s)
        {
            lock (this.scenes)
            {
                try
                {
                    this.scenes.Add(index, s);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
        }

        public bool Unload(int index)
        {
            Scene s = null;
            bool result;
            lock (this.scenes)
            {
                result = this.scenes.TryGetValue(index, out s) ? this.scenes.Remove(index) : false;
            }
            if (s != null)
            {
                s.Dispose();
            }
            return result;
        }

        protected override void OnEnable() 
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Enable();
            }
            this.workingCopy.Clear();
        }

        protected override void OnDisable()
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Disable();
            }
            this.workingCopy.Clear();
        }

        protected override void OnDraw()
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Draw();
            }
            this.workingCopy.Clear();
        }

        protected override void OnLoad()
        {
            logger.Info("Loading scene manager.");

            this.Load(0, this.StartScene).Wait();
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Load();
            }
            this.workingCopy.Clear();

            logger.Info("Scene manager loaded.");
        }

        protected override void OnReset()
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Reset();
            }
            this.workingCopy.Clear();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Update(gameTime);
            }
            this.workingCopy.Clear();
        }
    }
}
