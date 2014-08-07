using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    [ContractClass(typeof(ShaderProgramContracts))]
    public abstract class Shader : GLObject, IBindable
    {
        [IgnoreDataMember]
        public bool IsLinked { get; private set; }

        [IgnoreDataMember]
        public abstract int SamplerCount { get; protected set; }

        [IgnoreDataMember]
        public int ShaderCount { get; private set; }

        [IgnoreDataMember]
        public ShaderStage[] Stages { get; private set; }

        public int TotalUniformLocationCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return this.SamplerCount + this.UniformLocationCount;
            }
        }

        [IgnoreDataMember]
        public abstract int UniformLocationCount { get; protected set; }

        public Shader(IEnumerable<ShaderStage> shaders)
            : base(GL.CreateProgram())
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentException>(!shaders.Duplicates(shader => shader.Type));

            logger.Info(() => "Initializing a new shader with {0} stages.".FormatWith(shaders.Count()));

            this.Stages = shaders.ToArray();
            this.ShaderCount = this.Stages.Length;
        }

        public void Bind()
        {
            this.Link();
            GL.UseProgram(this);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        public int GetUniformLocation(string uniformName)
        {
            Contract.Requires<ArgumentNullException>(uniformName != null);

            return GL.GetUniformLocation(this, uniformName);
        }

        public void Link()
        {
            if (!this.IsLinked)
            {
                logger.Debug(() => "Linking shader on thread {0}.".FormatWith(System.Threading.Thread.CurrentThread.ManagedThreadId));

                foreach (ShaderStage shader in this.Stages.FilterNull())
                {
                    shader.Compile();
                    GL.AttachShader(this, shader);
                }
                GL.LinkProgram(this);

                int linkStatus;
                if (!this.CheckStatus(GetProgramParameterName.LinkStatus, out linkStatus))
                {
                    string message = "Linking the shader failed. Error Code: {0}".FormatWith(linkStatus);
                    logger.Error(message);
                    logger.Error(GL.GetShaderInfoLog(this));

                    throw new InvalidOperationException(message);
                }
                this.IsLinked = true;

                logger.Debug(() => "Shader linked.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.Unbind();
            foreach (ShaderStage shader in this.Stages.FilterNull())
            {
                try
                {
                    GL.DetachShader(this, shader);
                }
                catch { }
            }
            try
            {
                GL.DeleteProgram(this);
            }
            catch (Exception ex)
            {
                logger.Warn(() => 
                    "An exception of type '{0}' was thrown while deleting the shader. Swallowing...".FormatWith(ex.GetType().AssemblyQualifiedName),
                    ex
                );
            }
            base.Dispose(disposing);
        }

        protected bool CheckStatus(GetProgramParameterName parameter)
        {
            int result = 0;
            return CheckStatus(parameter, out result);
        }

        protected bool CheckStatus(GetProgramParameterName parameter, out int result)
        {
            GL.GetProgram(this, parameter, out result);
            return (result == 1);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Stages != null);
        }
    }

    [ContractClassFor(typeof(Shader))]
    abstract class ShaderProgramContracts : Shader
    {
        public ShaderProgramContracts() 
            : base(Enumerable.Empty<ShaderStage>()) 
        {
            Contract.Requires(false);
        }

        public override int SamplerCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return 0;
            }
            protected set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);
            }
        }

        public override int UniformLocationCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return 0;
            }
            protected set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);
            }
        }
    }
}
