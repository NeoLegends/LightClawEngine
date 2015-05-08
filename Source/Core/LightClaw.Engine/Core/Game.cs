using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.IO;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a game.
    /// </summary>
    public class Game : DisposableEntity, IGame
    {
        // TODO: Abstract rendering from Game class, perhaps in some sort of component-like GameSystem-system. New
        //       renderer should also allow for multiple render targets, respectively cameras rendering to textures.
        //       (which then can be rendered afterwards into the scene)

        /// <summary>
        /// The version of OpenGL to use for rendering.
        /// </summary>
        private static readonly Version OpenGLVersion = new Version(4, 4);

        /// <summary>
        /// Used for avoiding creating a new object every tick.
        /// </summary>
        private readonly Action cachedOnTickAction;

        /// <summary>
        /// The maximum time for a single frame.
        /// </summary>
        private readonly TimeSpan frameDuration = TimeSpan.FromMilliseconds(1000.0 / (double)VideoSettings.Default.FPSLimit);

        /// <summary>
        /// The system tick count before the update has occured.
        /// </summary>
        private long preUpdateTime = 0L;

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

        private Dispatcher _Dispatcher = Dispatcher.Current;

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> associated with the <see cref="Game"/>.
        /// </summary>
        public Dispatcher Dispatcher
        {
            get
            {
                return _Dispatcher;
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
            OpenGLVersion.Major, OpenGLVersion.Minor,
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
        /// Backing field.
        /// </summary>
        private bool _SuppressDraw;

        /// <summary>
        /// Indicates whether the drawing shall not be performed.
        /// </summary>
        public bool SuppressDraw
        {
            get
            {
                return (_SuppressDraw || !this.GameWindow.Visible);
            }
            set
            {
                this.SetProperty(ref _SuppressDraw, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Game"/> from the specified <paramref name="startScene"/>.
        /// </summary>
        /// <param name="startScene">The <see cref="Scene"/> that will be run at startup.</param>
        public Game(ResourceString startScene)
            : base(GeneralSettings.Default.GameName)
        {
            Contract.Requires<ArgumentNullException>(startScene != null);

            if (!GLObject.IsOpenGLVersionSupported(OpenGLVersion))
            {
                throw new NotSupportedException(
                    "The required OpenGL-Version ({0}) is not supported by the current OS / hardware (max vesion {1}). LightClaw cannot run.".FormatWith(OpenGLVersion, GLObject.MaxOpenGLVersion)
                );
            }

            this.cachedOnTickAction = new Action(this.OnTick);

            this.GameWindow.Closed += (s, e) => this.OnClosed();
            this.GameWindow.Move += (s, e) => this.OnMove(this.GameWindow.Location);
            this.GameWindow.Resize += (s, e) => this.OnResize(this.GameWindow.Width, this.GameWindow.Height);
            this.GameWindow.Unload += (s, e) => this.OnUnload();
            
            Log.Debug(() => "Creating {0} from start scene '{1}.'".FormatWith(typeof(SceneManager).Name, startScene));
            this.SceneManager = new SceneManager(startScene);
            
            this.IocC.RegisterInstance(this.SceneManager);
            this.IocC.RegisterInstance(this.Dispatcher);

            this.LoadIcon();

            Log.Debug(() => "Game successfully created.");
        }

        /// <summary>
        /// Runs the game.
        /// </summary>
        public void Run()
        {
            bool limitFps = VideoSettings.Default.LimitFPS;
            double maxFrameRate = VideoSettings.Default.FPSLimit;

            Log.Info(
                () => limitFps ?
                    "Entering game loop. FPS will be limited to {0}.".FormatWith(maxFrameRate) :
                    "Entering game loop with unlimited frame rate."
            );

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            SynchronizationContext.SetSynchronizationContext(new LightClawSynchronizationContext());
            this.SceneManager.Load();
            this.SceneManager.Enable();

            this.GameWindow.Visible = true;

            this.Dispatcher.InvokeSlim(this.OnTick);
            this.Dispatcher.Run();
        }

        /// <summary>
        /// Disposes the <see cref="Game"/> and optionally releases managed resources as well.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                this.Dispatcher.Stop();
                this.SceneManager.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.Closed"/>.
        /// </summary>
        protected virtual void OnClosed()
        {
            Log.Info(() => "Closing game window.");

            this.Dispose();
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.Move"/>.
        /// </summary>
        protected virtual void OnMove(System.Drawing.Point location)
        {
            this.OnRender();
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.RenderFrame"/>.
        /// </summary>
        protected virtual void OnRender()
        {
            if (!this.SuppressDraw)
            {
                GL.Viewport(0, 0, this.GameWindow.Width, this.GameWindow.Height);
                GL.ClearColor(Color.CornflowerBlue);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                this.SceneManager.Draw();
                this.GameWindow.SwapBuffers();
            }
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.Resize"/>.
        /// </summary>
        /// <param name="width">The <see cref="IGameWindow"/>s new width.</param>
        /// <param name="height">The <see cref="IGameWindow"/>s new height.</param>
        protected virtual void OnResize(int width, int height)
        {
            this.OnRender();
        }

        /// <summary>
        /// Callback for <see cref="E:IGameWindow.UpdateFrame"/>.
        /// </summary>
        /// <param name="elapsedSinceLastUpdate">The time that passed since the last call to this callback.</param>
        protected virtual void OnUpdate(TimeSpan elapsedSinceLastUpdate)
        {
            GameTime currentGameTime = this.CurrentGameTime = this.CurrentGameTime + elapsedSinceLastUpdate;

            int pass = 0;
            while (!this.SceneManager.Update(currentGameTime, pass++)) { }
        }

        /// <summary>
        /// Unloads the game's contents.
        /// </summary>
        protected virtual void OnUnload()
        {
            this.Dispose();
        }

        /// <summary>
        /// Loads the icon.
        /// </summary>
        private async void LoadIcon()
        {
            try
            {
                this.GameWindow.Icon = await this.IocC.Resolve<IContentManager>()
                                                      .LoadAsync<System.Drawing.Icon>(GeneralSettings.Default.IconPath);
            }
            catch (Exception ex)
            {
                Log.Warn("An error of type {0} occured while loading the game's icon.".FormatWith(ex.GetType().FullName), ex);
            }
        }

        private void OnTick()
        {
            this.GameWindow.ProcessEvents();
            
            long previousPreUpdateTime = Interlocked.Exchange(ref this.preUpdateTime, Stopwatch.GetTimestamp());
            TimeSpan elapsedSinceLastUpdate = TimeSpan.FromTicks((previousPreUpdateTime != 0L) ? this.preUpdateTime - previousPreUpdateTime : 0L);

            this.OnUpdate(elapsedSinceLastUpdate);
            this.OnRender();

            TimeSpan timeSinceUpdate = TimeSpan.FromTicks(Stopwatch.GetTimestamp() - this.preUpdateTime);
            TimeSpan timeToSleep = frameDuration - timeSinceUpdate;
            if (timeToSleep > TimeSpan.Zero)
            {
                Thread.Sleep(timeToSleep);
            }

            this.Dispatcher.InvokeSlim(this.cachedOnTickAction); // Small perf hack
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
