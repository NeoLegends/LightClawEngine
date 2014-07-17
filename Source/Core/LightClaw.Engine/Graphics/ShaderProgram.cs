using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [ProtoContract]
    public class ShaderProgram : GLObject, IBindable, INameable
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public AttributeLocationBinding[] AttributeBindings { get; private set; }

        [ProtoMember(3)]
        public Shader[] Shaders { get; private set; }

        private ShaderProgram() : this(Enumerable.Empty<Shader>(), Enumerable.Empty<AttributeLocationBinding>()) { }

        public ShaderProgram(IEnumerable<Shader> shaders, IEnumerable<AttributeLocationBinding> attributeBindings)
            : this(null, shaders, attributeBindings)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentNullException>(attributeBindings != null);
            Contract.Requires<ArgumentException>(shaders.All(shader => shader != null));
        }

        public ShaderProgram(String name, IEnumerable<Shader> shaders, IEnumerable<AttributeLocationBinding> attributeBindings)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentNullException>(attributeBindings != null);
            Contract.Requires<ArgumentException>(shaders.All(shader => shader != null));

            this.Name = name;
            this.AttributeBindings = attributeBindings.ToArray();
            this.Shaders = shaders.ToArray();

            this.Initialize();
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
            foreach (Shader shader in this.Shaders)
            {
                GL.AttachShader(this, shader);
            }
            foreach (AttributeLocationBinding binding in this.AttributeBindings)
            {
                GL.BindAttribLocation(this, binding.Index, binding.Name);
            }
            GL.LinkProgram(this);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                this.Unbind();
                foreach (Shader shader in this.Shaders)
                {
                    GL.DetachShader(this, shader);
                }
                GL.DeleteProgram(this);
            }
            catch (AccessViolationException)
            {
                throw; // Log and swallow in the future
            }
            base.Dispose(disposing);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.AttributeBindings != null);
            Contract.Invariant(this.Shaders != null);
        }
    }
}
