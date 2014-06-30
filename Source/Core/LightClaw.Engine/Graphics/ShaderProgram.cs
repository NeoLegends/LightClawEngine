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
    public class ShaderProgram : GLObject, INameable
    {
        [ProtoMember(3)]
        public AttributeBinding[] AttributeBindings { get; private set; }

        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public Shader[] Shaders { get; private set; }

        private ShaderProgram() : this(Enumerable.Empty<Shader>(), Enumerable.Empty<AttributeBinding>()) { }

        public ShaderProgram(IEnumerable<Shader> shaders, IEnumerable<AttributeBinding> attributeBindings)
            : this(null, shaders, attributeBindings)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentNullException>(attributeBindings != null);
        }

        public ShaderProgram(String name, IEnumerable<Shader> shaders, IEnumerable<AttributeBinding> attributeBindings)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentNullException>(attributeBindings != null);

            this.AttributeBindings = attributeBindings.ToArray();
            this.Name = name;
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
            foreach (Shader shader in this.Shaders)
            {
                GL.AttachShader(this, shader);
            }
            foreach (AttributeBinding binding in this.AttributeBindings)
            {
                GL.BindAttribLocation(this, binding.Index, binding.Name);
            }
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

                foreach (int shader in attachedShaders)
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

        [ProtoContract]
        public struct AttributeBinding
        {
            [ProtoMember(1)]
            public int Index { get; private set; }

            [ProtoMember(2)]
            public string Name { get; private set; }

            public AttributeBinding(int index, string name)
                : this()
            {
                Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
                Contract.Requires<ArgumentNullException>(name != null);

                this.Index = index;
                this.Name = name;
            }
        }
    }
}
