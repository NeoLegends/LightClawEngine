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

out vec3 passVertexColor;

void main(void)
{
	gl_Position = vec4(inVertexPosition, 1.0);
	passVertexColor = vec3(1.0, 0.0, 0.0);
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

        private int indexVboHandle;

        private int vaoHandle;

        private int[] indexVboData = new[]
        {
            1, 2, 3
        };

        private Vector3[] vertexVboData = new[]
        {
            new Vector3(-1.0f, -1.0f, 0.0f), 
            new Vector3(0.0f, 1.0f, 0.0f), 
            new Vector3(1.0f, -1.0f, 0.0f)
        };

        public Program()
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

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(this.vaoHandle);

            GL.DrawArrays(PrimitiveType.Triangles, 0, this.indexVboData.Length);

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

            this.fragShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(this.fragShaderHandle, fShaderSource);
            GL.CompileShader(this.fragShaderHandle);

            this.shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(this.shaderProgramHandle, this.vertShaderHandle);
            GL.AttachShader(this.shaderProgramHandle, this.fragShaderHandle);
            GL.BindAttribLocation(this.shaderProgramHandle, 0, "inVertexPosition");
            GL.LinkProgram(this.shaderProgramHandle);

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

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexVboHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexVboHandle);

            Console.WriteLine();

            GL.BindVertexArray(0);
        }

        static void Main(string[] args)
        {
            new Program();
        }
    }
}
