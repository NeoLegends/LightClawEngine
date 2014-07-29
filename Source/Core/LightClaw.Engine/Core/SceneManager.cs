using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;

namespace LightClaw.Engine.Core
{
    internal class SceneManager : Manager, ISceneManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SceneManager));

        private readonly SortedDictionary<int, Scene> scenes = new SortedDictionary<int, Scene>(new ReverseComparer<int>());

        private readonly List<Scene> workingCopy = new List<Scene>();

        public Scene this[int index]
        {
            get
            {
                lock (this.scenes)
                {
                    return this.scenes[index];
                }
            }
        }

        public SceneManager(string startScene)
        {
            Contract.Requires<ArgumentNullException>(startScene != null);

            logger.Info("Initializing scene manager from resource string '{0}'.".FormatWith(startScene));
            this.Load(0, startScene).Wait();
        }

        public SceneManager(Scene startScene)
        {
            Contract.Requires<ArgumentNullException>(startScene != null);

            logger.Info("Initiailzing scene manager from scene '{0}'".FormatWith(startScene.Name ?? "N/A"));
            this.Load(0, startScene);
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
            logger.Debug("Moving a scene from {0} to position {1}.".FormatWith(index, newIndex));

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
            return this.Load(index, await IocC.Resolve<IContentManager>().LoadAsync<Scene>(resourceString));
        }

        public bool Load(int index, Scene s)
        {
            logger.Info("Loading a new scene into position {0}.".FormatWith(index));

            if (this.IsLoaded && !s.IsLoaded)
            {
                s.Load();
            }
            lock (this.scenes)
            {
                for (int i = index; i < int.MaxValue; i++)
                {
                    try
                    {
                        logger.Debug("Trying to insert scene '{0}' into position {1}.".FormatWith(s.Name ?? "N/a", i));
                        this.scenes.Add(i, s);
                        logger.Debug("Scene inserted into position {0} successfully.".FormatWith(i));
                        return true;
                    }
                    catch (ArgumentException)
                    {
                        logger.Info("Position taken, incrementing...");
                    }
                }

                logger.Warn("Scene insertion failed, all slots were taken."); // Shouldn't happen irl
                return false;
            }
        }

        public bool Unload(int index)
        {
            logger.Debug("Unloading scene from position {0}.".FormatWith(index));

            Scene s = null;
            bool result;
            lock (this.scenes)
            {
                result = this.scenes.TryGetValue(index, out s) ? this.scenes.Remove(index) : false;
            }
            if (s != null)
            {
                logger.Debug("Disposing scene...");
                s.Dispose();
            }

            logger.Debug(result ? "Scene unloaded from position {0}.".FormatWith(index) : "Scene could not be removed.");
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

        protected override void OnLateUpdate()
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.LateUpdate();
            }
            this.workingCopy.Clear();
        }
    }
}
