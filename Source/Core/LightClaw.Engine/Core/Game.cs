using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a game.
    /// </summary>
    public class Game : Entity, IGame
    {
        /// <summary>
        /// Backing field.
        /// </summary>
        private GameTime _CurrentGameTime;

        /// <summary>
        /// The current <see cref="GameTime"/>.
        /// </summary>
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

        /// <summary>
        /// Backing field.
        /// </summary>
        private IGameWindow _GameWindow = new OpenTK.GameWindow(
            MathF.ClampToInt32(VideoSettings.Default.Width),
            MathF.ClampToInt32(VideoSettings.Default.Height),
            new OpenTK.Graphics.GraphicsMode(),
            GeneralSettings.Default.WindowTitle,
            OpenTK.GameWindowFlags.Default, 
            OpenTK.DisplayDevice.Default,
            4, 4,
#if DEBUG
            OpenTK.Graphics.GraphicsContextFlags.Debug
#else
            OpenTK.Graphics.GraphicsContextFlags.Default
#endif
        )
        {
            WindowState = VideoSettings.Default.WindowState,
            VSync = VideoSettings.Default.VSync
        };

        /// <summary>
        /// The <see cref="IGameWindow"/> the game is presented in.
        /// </summary>
        public IGameWindow GameWindow
        {
            get
            {
                Contract.Ensures(Contract.Result<IGameWindow>() != null);

                return _GameWindow;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _GameWindow, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private ISceneManager _SceneManager;

        /// <summary>
        /// The <see cref="ISceneManager"/> managing the currently loaded <see cref="Scene"/>s.
        /// </summary>
        public ISceneManager SceneManager
        {
            get
            {
                Contract.Ensures(Contract.Result<ISceneManager>() != null);

                return _SceneManager;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _SceneManager, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Game"/>.
        /// </summary>
        private Game()
        {
            Logger.Info(() => "Initializing a new game instance.");

            this.Name = GeneralSettings.Default.GameName;

            this.GameWindow.Closed += (s, e) => this.OnClosed();
            this.GameWindow.Load += (s, e) => this.OnLoad();
            this.GameWindow.RenderFrame += (s, e) => this.OnRender();
            this.GameWindow.Resize += (s, e) => this.OnResize(this.GameWindow.Width, this.GameWindow.Height);
            this.GameWindow.UpdateFrame += (s, e) => this.OnUpdate(e.Time);

            this.IocC.Resolve<IContentManager>()
                     .LoadAsync<System.Drawing.Icon>(GeneralSettings.Default.IconPath)
                     .ContinueWith(t => this.GameWindow.Icon = t.Result, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
        }
        
        /// <summary>
        /// Initializes a new <see cref="Game"/> from the specified <paramref name="startScene"/>.
        /// </summary>
        /// <param name="startScene">The resource string of the <see cref="Scene"/> to be loaded at startup.</param>
        public Game(string startScene)
            : this()
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(startScene));

            this.SceneManager = new SceneManager(startScene);
            this.IocC.RegisterInstance<ISceneManager>(this.SceneManager);

            Logger.Info(() => "Game successfully created.");
        }

        /// <summary>
        /// Initializes a new <see cref="Game"/> from the specified <paramref name="startScene"/>.
        /// </summary>
        /// <param name="startScene">The <see cref="Scene"/> that will be run at startup.</param>
        public Game(Scene startScene)
            : this()
        {
            Contract.Requires<ArgumentNullException>(startScene != null);

            this.SceneManager = new SceneManager(startScene);
            this.IocC.RegisterInstance<ISceneManager>(this.SceneManager);

            Logger.Info(() => "Game successfully created.");
        }

        /// <summary>
        /// Finalizes the instance before the object is reclaimed by garbage collection.
        /// </summary>
        ~Game()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Runs the game.
        /// </summary>
        public void Run()
        {
            bool limitFps = VideoSettings.Default.LimitFPS;
            double maxFrameRate = (double)VideoSettings.Default.FPSLimit;

            Logger.Info(
                () => limitFps ? 
                    "Entering game loop. FPS will be limited to {0}.".FormatWith(maxFrameRate) :
                    "Entering game loop with unlimited frame rate."
            );

            this.GameWindow.Run(limitFps ? maxFrameRate : 0.0);
        }

        /// <summary>
        /// Disposes the <see cref="Game"/> freeing all managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes the <see cref="Game"/> and optionally releases managed resources as well.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources as well.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.GameWindow.Dispose();
            this.SceneManager.Dispose();
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.Closed"/>.
        /// </summary>
        protected virtual void OnClosed()
        {
            Logger.Info(() => "Closing game window.");

            this.Dispose();
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.Load"/>.
        /// </summary>
        protected virtual void OnLoad()
        {
            Logger.Info(() => "OnLoad callback called. Loading SceneManager and enabling depth testing.");

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            Logger.Info(() => "Depth testing enabled.");

            GL.ClearColor(Color.CornflowerBlue);

            this.SceneManager.Load();
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.RenderFrame"/>.
        /// </summary>
        protected virtual void OnRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            this.SceneManager.Draw();
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.Resize"/>.
        /// </summary>
        /// <param name="width">The <see cref="IGameWindow"/>s new width.</param>
        /// <param name="height">The <see cref="IGameWindow"/>s new height.</param>
        protected virtual void OnResize(int width, int height)
        {
            Logger.Info(() => "Resizing window to {0}x{1}.".FormatWith(width, height));

            GL.Viewport(0, 0, width, height);
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.UpdateFrame"/>.
        /// </summary>
        /// <param name="elapsedSinceLastUpdate">The time that passed since the last call to this callback.</param>
        protected virtual void OnUpdate(double elapsedSinceLastUpdate)
        {
            elapsedSinceLastUpdate = Math.Max(elapsedSinceLastUpdate, 0.0);
            GameTime currentGameTime = new GameTime(
                this.CurrentGameTime.ElapsedSinceLastUpdate + elapsedSinceLastUpdate,
                this.CurrentGameTime.TotalGameTime + elapsedSinceLastUpdate
            );

            this.CurrentGameTime = currentGameTime;
            this.SceneManager.Update(currentGameTime);
            this.SceneManager.LateUpdate();
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._GameWindow != null);
            Contract.Invariant(this._SceneManager != null);
        }
    }
}
