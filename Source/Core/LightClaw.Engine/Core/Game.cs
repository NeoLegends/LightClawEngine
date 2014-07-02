using System;
using System.Collections.Concurrent;
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
    public class Game
    {
        private GameWindow gameWindow;

        public event EventHandler<ValueChangedEventArgs<Scene>> SceneLoading;

        public event EventHandler<ValueChangedEventArgs<Scene>> SceneLoaded;

        public Scene Scene { get; private set; }

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
            this.gameWindow.UpdateFrame += (s, e) => this.OnUpdate();
            this.gameWindow.RenderFrame += (s, e) => this.OnDraw();

            this.gameWindow.Run(60d);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void OnDraw()
        {

        }

        private void OnResize()
        {

        }

        private void OnUpdate()
        {

        }

        private void RaiseSceneLoaded(Scene newScene, Scene oldScene)
        {
            EventHandler<ValueChangedEventArgs<Scene>> handler = this.SceneLoaded;
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<Scene>(newScene, oldScene));
            }
        }

        private void RaiseSceneLoading(Scene newScene, Scene oldScene)
        {
            EventHandler<ValueChangedEventArgs<Scene>> handler = this.SceneLoading;
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<Scene>(newScene, oldScene));
            }
        }
    }
}
