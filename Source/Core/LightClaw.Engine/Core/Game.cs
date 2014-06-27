using System;
using System.Collections.Generic;
using System.IO;
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

        public static void Main(string[] args)
        {
            Random r = new Random();
            Vertex[] vertices = new Vertex[100];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vertex(
                    new Vector2(
                        (float)r.NextDouble(), 
                        (float)r.NextDouble()
                    ),
                    new Vector3(
                        (float)(r.NextDouble() * 100),
                        (float)(r.NextDouble() * 100),
                        (float)(r.NextDouble() * 100)
                    ),
                    new Vector3(
                        (float)(r.NextDouble() * 100),
                        (float)(r.NextDouble() * 100),
                        (float)(r.NextDouble() * 100)
                    )
                );
            }

            using (FileStream fs = File.OpenWrite("E:\\Users\\Moritz\\Desktop\\Vertices.pbuf"))
            {
                ProtoBuf.Serializer.Serialize(fs, vertices);
            }

            //GameWindow game = new GameWindow(1280, 720, new GraphicsMode(), "Test Window");

            //using (Buffer<Vertex> vbo = new Buffer<Vertex>(
            //        new[] { 
            //            new Vertex(Vector2.Zero, Vector3.Zero, Vector3.One),
            //            new Vertex(Vector2.Zero, new Vector3(1, 1, 0), Vector3.One),
            //            new Vertex(Vector2.Zero, new Vector3(0.0f, 1.0f, 0.0f), Vector3.One)
            //        }
            //    ))
            //{
            //    game.Load += (s, e) =>
            //    {
            //        game.VSync = VSyncMode.On;
            //        GL.ClearColor(Color4.CornflowerBlue);
            //    };
            //    game.Resize += (s, e) => GL.Viewport(0, 0, game.Width, game.Height);
            //    game.UpdateFrame += (s, e) =>
            //    {
            //        if (game.Keyboard[OpenTK.Input.Key.Escape] || game.Keyboard[OpenTK.Input.Key.Q])
            //        {
            //            game.Exit();
            //        }
            //    };
            //    game.RenderFrame += (s, e) =>
            //    {
            //        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            //        vbo.Bind();
            //        GL.DrawArrays(PrimitiveType.Triangles, 0, vbo.Count);
            //        game.SwapBuffers();
            //    };

            //    game.Run(60d);
            //}
        }
    }
}
