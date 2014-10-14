using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.GameCode
{
    [DataContract]
    public class BasicMesh : Component
    {
        //private Matrix modelViewProjectionMatrix = 
        //    Matrix.PerspectiveFovRH(MathF.DegreesToRadians(45), 16 / 9, 0.1f, 300f) *
        //    Matrix.LookAtRH(new Vector3(3, 3, 3), Vector3.Zero, Vector3.Up) * 
        //    Matrix.Identity;

        private int getErrorCount = 0;

        private IBuffer indexBuffer;

        private VertexArrayObject vao;

        private ShaderProgram program;

        private IBuffer vertexBuffer;

        public BasicMesh() { }

        protected override void Dispose(bool disposing)
        {
            VertexArrayObject vao = this.vao;
            if (vao != null)
            {
                vao.Dispose();
            }
            IBuffer iBuffer = this.indexBuffer;
            if (iBuffer != null)
            {
                iBuffer.Dispose();
            }
            IBuffer vBuffer = this.vertexBuffer;
            if (vBuffer != null)
            {
                vBuffer.Dispose();
            }
            ShaderProgram program = this.program;
            if (program != null)
            {
                program.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnLoad()
        {
            this.vertexBuffer = BufferObject.Create(
                new[] { 
                    -1.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                    1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
                    1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
                    -1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 1.0f,
                },
                BufferTarget.ArrayBuffer, 
                BufferUsageHint.StaticDraw
            );
            this.indexBuffer = BufferObject.Create(
                new ushort[] { 2, 1, 0, 3, 2, 0 }, 
                BufferTarget.ElementArrayBuffer, 
                BufferUsageHint.StaticDraw
            );

            BufferDescription desc = new BufferDescription(
                this.vertexBuffer,
                new VertexAttributePointer(VertexAttributeLocation.Position, 3, VertexAttribPointerType.Float, false, 24, 0),
                new VertexAttributePointer(VertexAttributeLocation.Color, 3, VertexAttribPointerType.Float, false, 24, 12)
            );
            this.vao = new VertexArrayObject(this.indexBuffer, desc);
            IContentManager mgr = this.IocC.Resolve<IContentManager>();
            Task<string> fragmentShaderSourceTask = mgr.LoadAsync<string>("Shaders/Basic.frag");
            Task<string> vertexShaderSourceTask = mgr.LoadAsync<string>("Shaders/Basic.vert");

            Task.WhenAll(fragmentShaderSourceTask, vertexShaderSourceTask).ContinueWith(t =>
            {
                Shader[] shaders = new Shader[] 
                { 
                    new Shader(
                        t.Result[1], 
                        ShaderType.VertexShader, 
                        new VertexAttributeDescription("inVertexPosition", VertexAttributeLocation.Position), 
                        new VertexAttributeDescription("inVertexColor", VertexAttributeLocation.Color)
                    ),
                    new Shader(t.Result[0], ShaderType.FragmentShader)
                };
                this.program = new ShaderProgram(shaders);
            });

            Task.Run(() => this.IocC.Resolve<IGame>().UpdateDispatcher.Invoke(() => Logger.Info("Hello from the message pumped through the dispatcher!")));

            base.OnLoad();
        }

        protected override void OnDraw()
        {
            VertexArrayObject vao = this.vao;
            ShaderProgram program = this.program;
            if (vao != null && program != null)
            {
                using (Binding programBinding = new Binding(program))
                using (Binding vaoBinding = new Binding(vao))
                {
                    //program.Uniforms["mat_MVP"].Set(modelViewProjectionMatrix);
                    //if (getErrorCount < 3)
                    //{
                    //    Logger.Warn(() => "Current OpenGL-Error after setting uniform: {0}".FormatWith(GL.GetError()));
                    //}
                    vao.DrawIndexed();
                    if (getErrorCount < 3)
                    {
                        Logger.Warn(() => "Current OpenGL-Error after rendering: {0}".FormatWith(GL.GetError()));
                        getErrorCount++;
                    }
                }
            }
            base.OnDraw();
        }
    }
}
