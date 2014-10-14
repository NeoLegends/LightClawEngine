using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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
        private Dispatcher _GraphicsDispatcher = Dispatcher.Current;

        /// <summary>
        /// The <see cref="Dispatcher"/> used to submit work to the rendering thread.
        /// </summary>
        public Dispatcher UpdateDispatcher
        {
            get
            {
                Contract.Ensures(Contract.Result<Dispatcher>() != null);

                return _GraphicsDispatcher;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _GraphicsDispatcher, value);
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

            if (!GLObject.IsOpenGLVersionSupported(new Version(4, 3)))
            {
                throw new NotSupportedException(
                    "The required OpenGL-Version (4.3) is not supported by the current OS / hardware (max vesion {0}). LightClaw cannot run.".FormatWith(GLObject.MaxOpenGLVersion)
                );
            }

            this.GameWindow.Closed += (s, e) => this.OnClosed();
            this.GameWindow.Load += (s, e) => this.OnLoad();
            this.GameWindow.Move += (s, e) => this.OnMove(this.GameWindow.Location);
            this.GameWindow.RenderFrame += (s, e) => this.OnRender();
            this.GameWindow.Resize += (s, e) => this.OnResize(this.GameWindow.Width, this.GameWindow.Height);
            this.GameWindow.UpdateFrame += (s, e) => this.OnUpdate(e.Time);
            this.GameWindow.Unload += (s, e) => this.OnUnload();

            Logger.Debug(s => "Creating {0} from start scene '{1}.'".FormatWith(typeof(SceneManager).Name, s), startScene);
            this.SceneManager = new SceneManager(startScene);
            this.IocC.RegisterInstance<ISceneManager>(this.SceneManager);

            this.LoadIcon();

            Logger.Debug(() => "Game successfully created.");
        }

        /// <summary>
        /// Runs the game.
        /// </summary>
        public void Run()
        {
            bool limitFps = VideoSettings.Default.LimitFPS;
            double maxFrameRate = VideoSettings.Default.FPSLimit;

            Logger.Info(
                (fpsLimited, frameRate) => fpsLimited ?
                    "Entering game loop. FPS will be limited to {0}.".FormatWith(frameRate) :
                    "Entering game loop with unlimited frame rate.",
                limitFps, maxFrameRate
            );

            this.GameWindow.Run(limitFps ? maxFrameRate : 0.0);
        }

        /// <summary>
        /// Disposes the <see cref="Game"/> and optionally releases managed resources as well.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            this.UpdateDispatcher.Dispose();
            this.SceneManager.Dispose();
            //this.GameWindow.Dispose();

            base.Dispose(disposing);
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
            this.SceneManager.Load();
            this.SceneManager.Enable();

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Viewport(0, 0, this.GameWindow.Width, this.GameWindow.Height);
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
        protected virtual void OnUpdate(double elapsedSinceLastUpdate)
        {
            elapsedSinceLastUpdate = Math.Max(elapsedSinceLastUpdate, 0.0);
            GameTime currentGameTime = this.CurrentGameTime = this.CurrentGameTime + elapsedSinceLastUpdate;

            Contract.Assume(this.SceneManager != null);
            this.SceneManager.Update(currentGameTime);
            this.UpdateDispatcher.Pop();
            this.SceneManager.LateUpdate();
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
        private void LoadIcon()
        {
            Task<System.Drawing.Icon> iconLoadTask = this.IocC.Resolve<IContentManager>()
                                                              .LoadAsync<System.Drawing.Icon>(GeneralSettings.Default.IconPath);
            iconLoadTask.ContinueWith(
                t =>
                {
                    this.GameWindow.Icon = t.Result;
                    Logger.Debug(iconPath => "Icon '{0}' loaded successfully.".FormatWith(iconPath), GeneralSettings.Default.IconPath);
                },
                TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously
            );
            iconLoadTask.ContinueWith(
                t => Logger.Warn(ex => "Icon loading failed. An exception of type '{0}' occured.".FormatWith(ex.GetType().AssemblyQualifiedName), t.Exception, t.Exception),
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
            );
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._GameWindow != null);
            Contract.Invariant(this._GraphicsDispatcher != null);
            Contract.Invariant(this._SceneManager != null);
        }
    }
}
