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
    class Program : OpenTK.GameWindow
    {
        private const string vShaderSource = @"
#version 400

in vec3 inVertexPosition;
in vec3 inVertexColor;

out vec3 passVertexColor;

void main(void)
{
	gl_Position = vec4(inVertexPosition, 1.0);
	passVertexColor = inVertexColor;
}";

        private const string fShaderSource = @"
#version 400

in vec3 passVertexColor;

out vec4 finalColor;

void main(void)
{
	finalColor = vec4(passVertexColor, 1.0);
}";

        private int vertShaderHandle;

        private int fragShaderHandle;

        private int shaderProgramHandle;

        private int vertexVboHandle;

        private int colorVboHandle;

        private int indexVboHandle;

        private int vaoHandle;

        private int[] indexVboData = new[]
        {
            1, 2, 3
        };

        private Vector3[] vertexVboData = new[]
        {
            new Vector3(-1.0f, -1.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, -1.0f, 0.0f),
        };

        private Vector3[] colorVboData = new[]
        {
            Vector3.Random, Vector3.Random, Vector3.Random
        };

        public Program()
            : base(1280, 720, new GraphicsMode(), "Lülülü OpenTK Window")
        {
            this.Run(30);
        }

        protected override void OnClosed(EventArgs e)
        {
            GL.DetachShader(this.shaderProgramHandle, this.vertShaderHandle);
            GL.DetachShader(this.shaderProgramHandle, this.fragShaderHandle);
            GL.DeleteProgram(this.shaderProgramHandle);

            GL.DeleteShader(this.vertShaderHandle);
            GL.DeleteShader(this.fragShaderHandle);

            base.OnClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.VSync = OpenTK.VSyncMode.On;
            GL.ClearColor(0.0f, 1.0f, 0.0f, 1.0f);

            this.CreateShaders();
            this.CreateVBOs();
            this.CreateVAOs();

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(this.vaoHandle);

            GL.DrawArrays(PrimitiveType.Triangles, 0, this.indexVboData.Length);
            //GL.DrawElements(BeginMode.TriangleStrip, this.indexVboData.Length, DrawElementsType.UnsignedInt, 0);

            this.SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
        {
            if (this.Keyboard[OpenTK.Input.Key.Q])
            {
                this.Exit();
            }
        }

        private void CreateShaders()
        {
            this.vertShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(this.vertShaderHandle, vShaderSource);
            GL.CompileShader(this.vertShaderHandle);

            int compileStatus;
            GL.GetShader(this.vertShaderHandle, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus == 0)
            {
                throw new InvalidOperationException("Compiling vertex shader failed.");
            }

            this.fragShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(this.fragShaderHandle, fShaderSource);
            GL.CompileShader(this.fragShaderHandle);

            GL.GetShader(this.fragShaderHandle, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus == 0)
            {
                throw new InvalidOperationException("Compiling fragment shader failed.");
            }

            this.shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(this.shaderProgramHandle, this.vertShaderHandle);
            GL.AttachShader(this.shaderProgramHandle, this.fragShaderHandle);
            GL.LinkProgram(this.shaderProgramHandle);

            int linkStatus;
            GL.GetProgram(this.shaderProgramHandle, GetProgramParameterName.LinkStatus, out linkStatus);
            if (linkStatus == 0)
            {
                throw new InvalidOperationException("Linking failed.");
            }

            GL.UseProgram(this.shaderProgramHandle);
        }

        private void CreateVBOs()
        {
            this.vertexVboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexVboHandle);
            GL.BufferData(
                BufferTarget.ArrayBuffer, 
                (IntPtr)(Marshal.SizeOf(typeof(Vector3)) * this.vertexVboData.Length), 
                this.vertexVboData, 
                BufferUsageHint.StaticDraw
            );
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            this.colorVboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.colorVboHandle);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(Marshal.SizeOf(typeof(Vector3)) * this.colorVboData.Length),
                this.colorVboData,
                BufferUsageHint.StaticDraw
            );
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            this.indexVboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexVboHandle);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                (IntPtr)(sizeof(int) * this.indexVboData.Length),
                this.indexVboData,
                BufferUsageHint.StaticDraw
            );
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private void CreateVAOs()
        {
            this.vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vaoHandle);

            int vertexPositionAttribLocation = GL.GetAttribLocation(this.shaderProgramHandle, "inVertexPosition");
            int vertexColorAttribLocation = GL.GetAttribLocation(this.shaderProgramHandle, "inVertexColor");

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexVboHandle);
            GL.EnableVertexAttribArray(vertexPositionAttribLocation);
            GL.VertexAttribPointer(vertexPositionAttribLocation, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.colorVboHandle);
            GL.EnableVertexAttribArray(vertexColorAttribLocation);
            GL.VertexAttribPointer(vertexColorAttribLocation, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexVboHandle);

            GL.BindVertexArray(0);
        }

        static void Main(string[] args)
        {
            new Program();
        }
    }
}
