using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.IO;
using Munq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace LightClaw.Engine.Core
{
    internal class Game : Entity, IGame
    {
        private readonly GameWindow gameWindow = new GameWindow(
            VideoSettings.Default.Width,
            VideoSettings.Default.Height,
            new GraphicsMode(),
            GeneralSettings.Default.WindowTitle
        ) { VSync = VideoSettings.Default.VSync };

        private GameTime _CurrentGameTime;

        public GameTime CurrentGameTime
        {
            get
            {
                return _CurrentGameTime;
            }
            set
            {
                this.SetProperty(ref _CurrentGameTime, value);
            }
        }

        private IGameCodeInterface _GameCode;

        public IGameCodeInterface GameCode
        {
            get
            {
                return _GameCode;
            }
            set
            {
                this.SetProperty(ref _GameCode, value);
            }
        }

        public int Height
        {
            get
            {
                return this.gameWindow.Height;
            }
            set
            {
                this.gameWindow.Height = value;
                VideoSettings.Default.Height = value;
                VideoSettings.Default.Save();
                this.RaisePropertyChanged();
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

        public int Width
        {
            get
            {
                return this.gameWindow.Width;
            }
            set
            {
                this.gameWindow.Width = value;
                VideoSettings.Default.Width = value;
                VideoSettings.Default.Save();
                this.RaisePropertyChanged();
            }
        }

        public Game(IGameCodeInterface gameCodeInterface, string startScene)
        {
            Contract.Requires<ArgumentNullException>(gameCodeInterface != null);
            Contract.Requires<ArgumentNullException>(startScene != null);

            this.GameCode = gameCodeInterface;

            this.Name = GeneralSettings.Default.GameName;
            this.SceneManager = new SceneManager(startScene);

            this.gameWindow.Closed += (s, e) => this.OnClosed();
            this.gameWindow.Load += (s, e) => this.OnLoad();
            this.gameWindow.RenderFrame += (s, e) => this.OnRenderFrame();
            this.gameWindow.Resize += (s, e) => this.OnResize();
            this.gameWindow.UpdateFrame += (s, e) => this.OnUpdateFrame(e.Time);
            this.gameWindow.WindowStateChanged += (s, e) => this.OnWindowStateChanged(this.gameWindow.WindowState);

            this.IocC.Register<ISceneManager>(d => this.SceneManager);
            this.IocC.Resolve<IContentManager>()
                     .LoadAsync<Icon>(GeneralSettings.Default.Icon)
                     .ContinueWith(t => this.gameWindow.Icon = t.Result, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        ~Game()
        {
            this.Dispose(false);
        }

        public void Run()
        {
            this.gameWindow.Run(60.0);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.gameWindow.Dispose();
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

            if (!this.SuppressDraw)
            {
                this.SceneManager.Draw();
            }
        }

        protected virtual void OnResize()
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        protected virtual void OnUpdateFrame(double elapsedSinceLastUpdate)
        {
            this.CurrentGameTime = new GameTime(
                this.CurrentGameTime.ElapsedSinceLastUpdate + elapsedSinceLastUpdate,
                this.CurrentGameTime.TotalGameTime + elapsedSinceLastUpdate
            );

            this.SceneManager.Update(this.CurrentGameTime);
        }

        protected virtual void OnWindowStateChanged(WindowState windowState)
        {
            if (windowState == WindowState.Minimized)
            {
                this.SuppressDraw = true;
            }
        }
    }
}
