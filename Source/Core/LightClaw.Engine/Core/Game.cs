using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;

namespace LightClaw.Engine.Core
{
    internal class Game : Entity, IGame
    {
        private GameTime _CurrentGameTime;

        public GameTime CurrentGameTime
        {
            get
            {
                return _CurrentGameTime;
            }
            private set
            {
                this.SetProperty(ref _CurrentGameTime, value);
            }
        }

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

        private IGameWindow _GameWindow = new OpenTK.GameWindow(
            VideoSettings.Default.Width,
            VideoSettings.Default.Height,
            new OpenTK.Graphics.GraphicsMode(),
            GeneralSettings.Default.WindowTitle
        )
        {
            WindowState = VideoSettings.Default.WindowState,
            VSync = VideoSettings.Default.VSync
        };

        public IGameWindow GameWindow
        {
            get
            {
                return _GameWindow;
            }
            private set
            {
                this.SetProperty(ref _GameWindow, value);
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

        private bool _SuppressDraw;

        public bool SuppressDraw
        {
            get
            {
                return _SuppressDraw;
            }
            set
            {
                this.SetProperty(ref _SuppressDraw, value);
            }
        }

        public Game(Assembly gameCodeAssembly, string startScene)
        {
            Contract.Requires<ArgumentNullException>(gameCodeAssembly != null);
            Contract.Requires<ArgumentNullException>(startScene != null);

            this.GameCodeAssembly = gameCodeAssembly;

            this.Name = GeneralSettings.Default.GameName;
            this.SceneManager = new SceneManager(startScene);

            this.GameWindow.Closed += (s, e) => this.OnClosed();
            this.GameWindow.Load += (s, e) => this.OnLoad();
            this.GameWindow.RenderFrame += (s, e) => this.OnRender();
            this.GameWindow.Resize += (s, e) => this.OnResize(this.GameWindow.Width, this.GameWindow.Height);
            this.GameWindow.UpdateFrame += (s, e) => this.OnUpdate(e.Time);
            
            this.IocC.Register<ISceneManager>(d => this.SceneManager);
            this.IocC.Resolve<IContentManager>()
                     .LoadAsync<System.Drawing.Icon>(GeneralSettings.Default.Icon)
                     .ContinueWith(t => this.GameWindow.Icon = t.Result, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        ~Game()
        {
            this.Dispose(false);
        }

        public void Run()
        {
            this.GameWindow.Run(60.0);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.GameWindow.Dispose();
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

        protected virtual void OnRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            if (!this.SuppressDraw)
            {
                this.SceneManager.Draw();
            }
        }

        protected virtual void OnResize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }

        protected virtual void OnUpdate(double elapsedSinceLastUpdate)
        {
            this.CurrentGameTime = new GameTime(
                this.CurrentGameTime.ElapsedSinceLastUpdate + elapsedSinceLastUpdate,
                this.CurrentGameTime.TotalGameTime + elapsedSinceLastUpdate
            );

            this.SceneManager.Update(this.CurrentGameTime);
        }
    }
}
