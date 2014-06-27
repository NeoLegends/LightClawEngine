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
    [ProtoContract]
    public class Shader : GLObject, INameable
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string ResourceString { get; private set; }

        [ProtoIgnore]
        public string Source { get; private set; }

        [ProtoMember(3)]
        public ShaderType Type { get; private set; }

        private Shader() { }

        public Shader(string resourceString, ShaderType type)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            this.ResourceString = resourceString;
            this.Type = type;

            this.Initialize();
        }

        [ProtoAfterDeserialization]
        private void Initialize()
        {
            throw new NotImplementedException(); // Load Source from resource string using resource system

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
