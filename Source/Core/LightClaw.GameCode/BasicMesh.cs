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
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.GameCode
{
    [DataContract]
    public class BasicMesh : Component
    {
        private static readonly Vector3[] cubeData = new[]
        {
            //new Vector3(-0.5f,  0.5f, 0.0f),
            //new Vector3(-0.5f, -0.5f, 0.0f),
            //new Vector3( 0.5f, -0.5f, 0.0f),
            //new Vector3( 0.5f,  0.5f, 0.0f)
            new Vector3(-0.25f,  0.25f,  0.25f),
            new Vector3( 0.25f,  0.25f,  0.25f),
            new Vector3( 0.25f,  0.25f, -0.25f),
            new Vector3(-0.25f,  0.25f, -0.25f),
            
            new Vector3(-0.25f, -0.25f,  0.25f),
            new Vector3( 0.25f, -0.25f,  0.25f),
            new Vector3( 0.25f, -0.25f, -0.25f),
            new Vector3(-0.25f, -0.25f, -0.25f)
        };

        private static readonly Vector3[] colorData = new[]
        {
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.5f, 0.0f, 0.5f),
            
            new Vector3(0.5f, 0.0f, 0.5f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
        };

        private static readonly ushort[] indices = new ushort[]
        {
            //0, 1, 2,
            //0, 2, 3
            // Top
            0, 1, 2,
            0, 2, 3,

            // Front
            0, 5, 1,
            0, 4, 5,

            // Right
            1, 6, 2,
            1, 6, 5,

            // Left
            0, 3, 7,
            0, 5, 4,

            // Back
            2, 7, 3,
            2, 6, 7
        };

        private static readonly Matrix4 viewProjectionMatrix = 
            Matrix4.CreatePerspectiveFieldOfView(MathF.DegreesToRadians(50), 16 / 9, 0.01f, 100f) *
            Matrix4.LookAt(new Vector3(6, 3, 6), Vector3.Zero, new Vector3(0, 1, 0));

        private int getErrorCount = 0;

        private IBuffer colorBuffer;

        private IBuffer indexBuffer;

        private Matrix4 modelMatrix;

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
            IBuffer cBuffer = this.colorBuffer;
            if (cBuffer != null)
            {
                cBuffer.Dispose();
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

        protected override async void OnLoad()
        {
            IContentManager mgr = this.IocC.Resolve<IContentManager>();
            Task<string> fragmentShaderSourceTask = mgr.LoadAsync<string>("Shaders/Basic.frag");
            Task<string> vertexShaderSourceTask = mgr.LoadAsync<string>("Shaders/Basic.vert");

            this.indexBuffer = BufferObject.Create(indices, BufferTarget.ElementArrayBuffer);
            this.colorBuffer = BufferObject.Create(colorData, BufferTarget.ArrayBuffer);
            this.vertexBuffer = BufferObject.Create(cubeData, BufferTarget.ArrayBuffer);

            await Task.WhenAll(fragmentShaderSourceTask, vertexShaderSourceTask);

            ThreadF.ThrowIfNotCurrentThread(this.IocC.Resolve<Dispatcher>().Thread);

            Shader[] shaders = new Shader[] 
            { 
                new Shader(vertexShaderSourceTask.Result, ShaderType.VertexShader),
                new Shader(fragmentShaderSourceTask.Result, ShaderType.FragmentShader)
            };
            this.program = new ShaderProgram(shaders);

            ThreadF.ThrowIfNotCurrentThread(this.IocC.Resolve<Dispatcher>().Thread);

            BufferDescription vDesc = new BufferDescription(
                this.vertexBuffer,
                new VertexAttributePointer(this.program.Attributes["inVertexPosition"], 3, VertexAttribPointerType.Float, false, 0, 0)
            );
            BufferDescription cDesc = new BufferDescription(
                this.colorBuffer,
                new VertexAttributePointer(this.program.Attributes["inVertexColor"], 3, VertexAttribPointerType.Float, false, 0, 0)
            );
            this.vao = new VertexArrayObject(this.indexBuffer, vDesc, cDesc);
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
                    Matrix4 mvp = this.modelMatrix; // viewProjectionMatrix * this.modelMatrix;

                    program.Uniforms["MVP"].Set(mvp);
                    if (getErrorCount < 3)
                    {
                        Log.Warn(() => "Current OpenGL-Error after setting uniform: {0}".FormatWith(GL.GetError()));
                    }
                    vao.DrawIndexed();
                    if (getErrorCount < 3)
                    {
                        Log.Warn(() => "Current OpenGL-Error after rendering: {0}".FormatWith(GL.GetError()));
                        getErrorCount++;
                    }
                }
            }
        }

        protected override bool OnUpdate(GameTime gameTime, int pass)
        {
            if (pass == 0)
            {
                float degreeValue = (float)((gameTime.TotalGameTime.TotalSeconds * 90.0) % 360.0);
                float radiansValue = MathF.DegreesToRadians(degreeValue);
                this.modelMatrix = Matrix4.CreateRotationY(radiansValue);
            }
            return true;
        }
    }
}
