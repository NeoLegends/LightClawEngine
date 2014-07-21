using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    internal class SceneManager : Manager, ISceneManager
    {
        private readonly SortedDictionary<int, Scene> scenes = new SortedDictionary<int, Scene>();

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

            this.StartScene = startScene;
        }

        public async Task LoadScene(int index, string resourceString)
        {
            Scene s = await Scene.LoadFrom(resourceString);
            s.Load();
            this.LoadScene(index, s);
        }

        public void LoadScene(int index, Scene s)
        {
            lock (this.scenes)
            {
                this.scenes.Add(index, s);
            }
        }

        public void UnloadScene(int index)
        {
            Scene s;
            lock (this.scenes)
            {
                s = this.scenes[index];
                this.scenes.Remove(index);
            }
            s.Dispose();
        }

        protected override void OnEnable() 
        {
            foreach (Scene s in this.scenes.Values)
            {
                s.Enable();
            }
        }

        protected override void OnDisable()
        {
            foreach (Scene s in this.scenes.Values)
            {
                s.Disable();
            }
        }

        protected override void OnDraw()
        {
            foreach (Scene s in this.scenes.Values)
            {
                s.Draw();
            }
        }

        protected override void OnLoad()
        {
            this.LoadScene(0, this.StartScene).Wait();
            foreach (Scene s in this.scenes.Values)
            {
                s.Load();
            }
        }

        protected override void OnReset()
        {
            foreach (Scene s in this.scenes.Values)
            {
                s.Reset();
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            foreach (Scene s in this.scenes.Values)
            {
                s.Update(gameTime);
            }
        }
    }
}
