using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace LightClaw.Engine.Core
{
    public class Game : GameWindow, IGame
    {
        private IGameCodeInterface gameCode;

        private readonly List<Scene> scenes = new List<Scene>();

        private string startScene;

        [CLSCompliant(false)]
        public IocContainer IocC { get; private set; }

        public string Name { get; set; }

        public double TimeSinceLastUpdate { get; private set; }

        public double TotalGameTime { get; private set; }

        public ReadOnlyCollection<Scene> Scenes
        {
            get
            {
                return this.scenes.AsReadOnly();
            }
        }

        IEnumerable<Scene> IGame.Scenes
        {
            get
            {
                return this.Scenes;
            }
        }

        public Game(IGameCodeInterface gameCodeInterface, string startScene)
            : base(
                VideoSettings.Default.Resolution.Width,
                VideoSettings.Default.Resolution.Height,
                new GraphicsMode(),
                GeneralSettings.Default.WindowTitle
            )
        {
            Contract.Requires<ArgumentNullException>(gameCodeInterface != null);
            Contract.Requires<ArgumentNullException>(startScene != null);

            this.gameCode = gameCodeInterface;
            this.startScene = startScene;

            this.IocC.Resolve<IContentManager>().LoadAsync<Icon>(GeneralSettings.Default.Icon).ContinueWith(t => this.Icon = t.Result);
            this.IocC = LightClawEngine.DefaultIocContainer;
            this.Name = GeneralSettings.Default.GameName;
            this.VSync = VideoSettings.Default.VSync;
        }

        public new void Run()
        {
            base.Run(60.0);
        }

        void IGame.Run()
        {
            base.Run(60.0);
        }

        public async Task LoadScene(int index, string resourceString)
        {
            Scene s = await Scene.LoadFrom(resourceString);
            s.Load();
            lock (this.scenes)
            {
                this.scenes.Insert(index, s);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            this.LoadScene(0, this.startScene).Wait();

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            foreach (Scene s in this.scenes)
            {
                s.Draw();
            }

            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            this.TimeSinceLastUpdate = e.Time;
            this.TotalGameTime += e.Time;

            Scene[] scenes = null;
            lock (this.scenes)
            {
                scenes = this.scenes.ToArray();
            }
            foreach (Scene s in scenes)
            {
                s.Update(new GameTime(this.TimeSinceLastUpdate, this.TotalGameTime));
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            this.Dispose();

            base.OnClosed(e);
        }
    }
}
