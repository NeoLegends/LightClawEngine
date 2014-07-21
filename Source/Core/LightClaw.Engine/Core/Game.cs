using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Core
{
    internal class Game : Entity, IGame
    {
        private Assembly _GameCodeAssembly;

        public Assembly GameCodeAssembly
        {
            get
            {
                return _GameCodeAssembly;
            }
            set
            {
                this.SetProperty(ref _GameCodeAssembly, value);
            }
        }

        private IRenderManager _RenderManager;

        public IRenderManager RenderManager
        {
            get
            {
                return _RenderManager;
            }
            private set
            {
                this.SetProperty(ref _RenderManager, value);
            }
        }

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                this.SetProperty(ref _Name, value);
            }
        }

        private ISceneManager _SceneManager;

        public ISceneManager SceneManager
        {
            get
            {
                return _SceneManager;
            }
            set
            {
                this.SetProperty(ref _SceneManager, value);
            }
        }

        public Game(Assembly gameCodeAssembly, string startScene)
        {
            Contract.Requires<ArgumentNullException>(gameCodeAssembly != null);
            Contract.Requires<ArgumentNullException>(startScene != null);

            this.GameCodeAssembly = gameCodeAssembly;

            this.Name = GeneralSettings.Default.GameName;
            this.RenderManager = new RenderManager();
            this.SceneManager = new SceneManager(startScene);

            this.IocC.Register<IRenderManager>(d => this.RenderManager);
            this.IocC.Register<ISceneManager>(d => this.SceneManager);
            //this.IocC.Resolve<IContentManager>()
            //         .LoadAsync<Icon>(GeneralSettings.Default.Icon)
            //         .ContinueWith(t => this.gameWindow.Icon = t.Result, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        ~Game()
        {
            this.Dispose(false);
        }

        public void Run()
        {
            this.RenderManager.Run(60.0);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.RenderManager.Dispose();
            this.SceneManager.Dispose();
        }

        protected virtual void OnClosed()
        {
            this.Dispose();
        }

        protected virtual void OnLoad()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            this.SceneManager.Load();
        }

        protected virtual void OnRenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            if (!this.RenderManager.SuppressDraw)
            {
                this.SceneManager.Draw();
            }
        }

        protected virtual void OnResize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }

        protected virtual void OnUpdateFrame(double elapsedSinceLastUpdate)
        {
            //this.CurrentGameTime = new GameTime(
            //    this.CurrentGameTime.ElapsedSinceLastUpdate + elapsedSinceLastUpdate,
            //    this.CurrentGameTime.TotalGameTime + elapsedSinceLastUpdate
            //);

            //this.SceneManager.Update(this.CurrentGameTime);
        }
    }
}
