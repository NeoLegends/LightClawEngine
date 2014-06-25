using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class ShaderProgram : GLObject, INameable
    {
        public string Name { get; set; }

        public ShaderProgram(Shader[] shaders)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);

            this.Id = GL.CreateProgram();
            shaders.ForEach(shader => GL.AttachShader(this, shader));
            GL.LinkProgram(this);
        }

        public void Bind()
        {
            GL.UseProgram(this);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                int[] attachedShaders = null;
                int count = 0;
                GL.GetAttachedShaders(this, int.MaxValue, out count, attachedShaders);
                Contract.Assume(attachedShaders != null);
                attachedShaders.ForEach(attachedShader => GL.DetachShader(this, attachedShader));
            }
            catch (AccessViolationException)
            {
                throw; // Log and swallow in the future
            }
            base.Dispose(disposing);
        }
    }
}
