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
        private const string vertexShaderSource = 
@"#version 400

in vec3 inVertexPosition;

out vec4 vertexColor

void main(void)
{
	gl_Position = inVertexPosition;
	vertexColor = vec4(1, 0, 0, 0);
}";

        private const string fragmentShaderSource =
@"#version 400

in vec4 vertexColor;

out vec3 finalColor;

void main(void)
{
	finalColor = vertexColor;
}";

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
                new[] 
                { 
                    -1.0f, -1.0f, 0.0f,
                    1.0f, -1.0f, 0.0f,
                    0.0f,  1.0f, 0.0f,
                }, 
                BufferTarget.ArrayBuffer, 
                BufferUsageHint.StaticDraw
            );
            this.indexBuffer = BufferObject.Create(new int[] { 1, 2, 3 }, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw);

            BufferDescription desc = new BufferDescription(
                this.vertexBuffer,
                new VertexAttributePointer(VertexAttributeLocation.Position, 3, VertexAttribPointerType.Float, false, 0, 0)
            );
            this.vao = new VertexArrayObject(this.indexBuffer, desc);
            Shader[] shaders = new Shader[] 
            { 
                new Shader(vertexShaderSource, ShaderType.VertexShader, new VertexAttributeDescription("inVertexPosition", VertexAttributeLocation.Position)),
                new Shader(fragmentShaderSource, ShaderType.FragmentShader)
            };
            this.program = new ShaderProgram(shaders);

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
                    vao.DrawIndexed();
                }
            }
            base.OnDraw();
        }
    }
}
