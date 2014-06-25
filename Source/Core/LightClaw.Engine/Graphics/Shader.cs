using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    public class Shader : GLObject, INameable
    {
        public string Name { get; set; }

        public string Source { get; private set; }

        public ShaderType Type { get; private set; }

        public Shader(string source, ShaderType type)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(source));

            this.Source = source;
            this.Type = type;

            this.Id = GL.CreateShader(this.Type);
            GL.ShaderSource(this, this.Source);
            GL.CompileShader(this);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.DeleteShader(this);
            }
            catch (AccessViolationException)
            {
                throw;
            }
            base.Dispose(disposing);
        }
    }
}
