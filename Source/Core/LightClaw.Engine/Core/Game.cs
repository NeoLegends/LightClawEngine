using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Graphics;
using Munq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Core
{
    public class Game : IGame, INameable
    {
        private IGameCodeInterface gameCode;

        private GameWindow gameWindow = new GameWindow(
            VideoSettings.Default.Resolution.Width,
            VideoSettings.Default.Resolution.Height,
            new GraphicsMode(),
            GeneralSettings.Default.GameName
        ) { VSync = VideoSettings.Default.VSync };

        private string startScene;

        [CLSCompliant(false)]
        public IocContainer IocC { get; private set; }

        public string Name { get; set; }

        public Scene MainScene { get; private set; }

        public Scene TransitionScene { get; private set; }

        public Scene NewScene { get; private set; }

        private Game() 
        {
            this.IocC = LightClawEngine.DefaultIocContainer;
        }

        public Game(IGameCodeInterface gameCodeInterface, string startScene)
            : this()
        {
            this.gameCode = gameCodeInterface;
            this.startScene = startScene;
        }

        public void Run()
        {
            this.gameWindow.Resize += (s, e) => GL.Viewport(0, 0, this.gameWindow.Width, this.gameWindow.Height);
            this.gameWindow.UpdateFrame += (s, e) => this.OnUpdate();
            this.gameWindow.RenderFrame += (s, e) => this.OnDraw();

            this.gameWindow.Run(60.0);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void OnDraw()
        {

        }
        
        private void OnLoad()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
        }

        private void OnResize()
        {

        }

        private void OnUpdate()
        {

        }
    }
}
