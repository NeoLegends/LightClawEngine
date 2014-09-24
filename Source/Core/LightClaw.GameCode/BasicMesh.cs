using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;
using LCBuffer = LightClaw.Engine.Graphics.OpenGL.Buffer;

namespace LightClaw.GameCode
{
    public class BasicMesh : Component
    {
        private readonly IBuffer indexBuffer = new LCBuffer(BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw);

        private VertexArrayObject vao;

        private ShaderProgram program;

        private readonly IBuffer vertexBuffer = new LCBuffer(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);

        public BasicMesh()
        {

        }

        protected override void OnLoad()
        {
            vertexBuffer.Set(
                new Vertex[] { 
                    new Vertex(new Vector3(-1.0f, -1.0f, 0.0f), Vector3.Zero, Vector2.Zero, Color.Red),
                    new Vertex(new Vector3(1.0f, -1.0f, 0.0f), Vector3.Zero, Vector2.Zero, Color.Blue),
                    new Vertex(new Vector3(0.0f,  1.0f, 0.0f), Vector3.Zero, Vector2.Zero, Color.Yellow)
                }
            );
            indexBuffer.Set(new int[] { 1, 2, 3 });

            VertexAttributePointer[] pointers = new VertexAttributePointer[]
            {
                new VertexAttributePointer(VertexAttributeLocation.Position, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0),
                new VertexAttributePointer(VertexAttributeLocation.Color, 4, VertexAttribPointerType.UnsignedByte, false, Vertex.SizeInBytes, 32),
            };
            BufferDescription desc = new BufferDescription(this.vertexBuffer, pointers);
            this.vao = new VertexArrayObject(desc.Yield(), this.indexBuffer);

            IContentManager contentMgr = this.IocC.Resolve<IContentManager>();
            Task<string> vertexShaderSourceTask = contentMgr.LoadAsync<string>("Shaders/Basic.vert");
            Task<string> fragmentShaderSourceTask = contentMgr.LoadAsync<string>("Shaders/Basic.frag");

            Task.WhenAll(vertexShaderSourceTask, fragmentShaderSourceTask).ContinueWith(t =>
            {
                this.program = new ShaderProgram(
                    new Shader[] { 
                        new Shader(t.Result[0], ShaderType.VertexShader, new[] { new VertexAttributeDescription("inVertexPosition", VertexAttributeLocation.Position), new VertexAttributeDescription("inVertexColor", VertexAttributeLocation.Color) }),
                        new Shader(t.Result[1], ShaderType.FragmentShader)
                    }
                );
            });

            base.OnLoad();
        }

        protected override void OnDraw()
        {
            VertexArrayObject vao = this.vao;
            ShaderProgram program = this.program;
            if (vao != null && program != null)
            {
                using (Binding programBinding = new Binding(program))
                {
                    vao.DrawIndexed();
                }
            }
            base.OnDraw();
        }
    }
}
