using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [ProtoContract]
    public class ShaderProgram : GLObject, INameable
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public Shader[] Shaders { get; private set; }

        private ShaderProgram() { }

        public ShaderProgram(Shader[] shaders)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);

            this.Shaders = shaders.ToArray();
        }

        public void Bind()
        {
            GL.UseProgram(this);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        [ProtoAfterDeserialization]
        private void Initialize()
        {
            this.Id = GL.CreateProgram();
            this.Shaders.ForEach(shader => GL.AttachShader(this, shader));
            GL.LinkProgram(this);
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
                GL.DeleteProgram(this);
            }
            catch (AccessViolationException)
            {
                throw; // Log and swallow in the future
            }
            base.Dispose(disposing);
        }
    }
}
