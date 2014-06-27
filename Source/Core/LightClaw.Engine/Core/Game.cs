using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Graphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Core
{
    public class Game : IGame
    {
        private GameWindow gameWindow;

        private Scene scene;

        public event EventHandler<SceneLoadingEventArgs> SceneLoading;

        public event EventHandler<SceneLoadedEventArgs> SceneLoaded;

        public void Run()
        {
            this.gameWindow = new GameWindow(
                VideoSettings.Default.Resolution.Width,
                VideoSettings.Default.Resolution.Height,
                new GraphicsMode(),
                GeneralSettings.Default.GameName
            )
            {
                VSync = VideoSettings.Default.VSync
            };

            this.gameWindow.Resize += (s, e) => GL.Viewport(0, 0, this.gameWindow.Width, this.gameWindow.Height);
            this.gameWindow.UpdateFrame += (s, e) => this.scene.Update();
            this.gameWindow.RenderFrame += (s, e) => this.scene.Draw();

            this.gameWindow.Run(60d);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
