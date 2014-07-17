using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Coroutines;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            OpenTK.GameWindow gameWindow = new OpenTK.GameWindow(1280, 720, new GraphicsMode(), "OpenTK Window");

            LightClawEngine.DefaultIocContainer.Register<IContentManager>(
                d => new ContentManager(
                        new StringContentReader().Yield(),
                        new DiskContentResolver(AppDomain.CurrentDomain.BaseDirectory).Yield()
                    )
            );

            using (Buffer<Vertex> vertexBuffer = new Buffer<Vertex>(
                new[] { 
                    new Vertex() { Position = new Vector3(-1.0f, 1.0f, 0.0f), Color = new Vector3(1.0f, 0.0f, 0.0f) }, 
                    new Vertex() { Position = new Vector3(0, 1.0f, 0.0f), Color = new Vector3(0.0f, 0.0f, 1.0f) }, 
                    new Vertex() { Position = new Vector3(1.0f, 1.0f, 0.0f), Color = new Vector3(0.0f, 1.0f, 0.0f) } 
                }
            ))
            using (Buffer<int> indexBuffer = new Buffer<int>(new[] { 1, 2, 3}, BufferTarget.ElementArrayBuffer))
            using (VertexArrayObject arrayObject = new VertexArrayObject(
                new BufferConfiguration(
                    indexBuffer,
                    vertexBuffer, 
                    new[] { 
                        new VertexAttributePointer(0, 3, VertexAttribPointerType.Float, false, 0, 0),
                        new VertexAttributePointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0)
                    }
                ).Yield()
            ))
            using (Shader fragmentShader = new Shader("Basic.frag", ShaderType.FragmentShader))
            using (Shader vertexShader = new Shader("Basic.vert", ShaderType.VertexShader))
            using (ShaderProgram program = new ShaderProgram(
                new[] { fragmentShader, vertexShader },
                new[] { new AttributeLocationBinding(0, "inVertexPosition"), new AttributeLocationBinding(1, "inVertexColor") }
            ))
            {
                GL.ClearColor(1.0f, 0, 0, 0);
                gameWindow.RenderFrame += (s, e) =>
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    program.Bind();
                    arrayObject.Bind();
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                };

                gameWindow.Run(30);
            }
        }

        private struct Vertex
        {
            public Vector3 Position;

            public Vector3 Color;
        }
    }
}
