using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    [ContractClass(typeof(ShaderProgramContracts))]
    public abstract class Shader : GLObject, IBindable
    {
        private static ILog logger = LogManager.GetLogger(typeof(Shader));

        public abstract int SamplerCount { get; protected set; }

        public int ShaderCount { get; private set; }

        public ImmutableList<ShaderStage> Shaders { get; private set; }

        public int TotalUniformLocationCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return this.SamplerCount + this.UniformLocationCount;
            }
        }

        public abstract int UniformLocationCount { get; protected set; }

        internal Shader() : base(GL.CreateProgram()) { }

        public Shader(IEnumerable<ShaderStage> shaders)
            : this()
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentException>(!shaders.Duplicates(shader => shader.Type));

            this.Shaders = shaders.ToImmutableList();
            this.ShaderCount = this.Shaders.Count;

            logger.Info("Initializing a new shader with {0} shaders.".FormatWith(this.Shaders.Count));

            foreach (ShaderStage shader in this.Shaders)
            {
                GL.AttachShader(this, shader);
            }
            GL.LinkProgram(this);
            this.CheckCompileStatus();

            logger.Debug("ShaderProgram initialized.");
        }

        public void Bind()
        {
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

        protected abstract IEnumerable<int> GetSamplerUniformLocations();

        protected override void Dispose(bool disposing)
        {
            this.Unbind();
            foreach (ShaderStage shader in this.Shaders)
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
                logger.Warn(
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

        private void CheckCompileStatus()
        {
            int result;
            if (!this.CheckStatus(GetProgramParameterName.LinkStatus, out result))
            {
                string message = "Linking the shader failed. Error Code: {0}".FormatWith(result);
                logger.Error(message);
                logger.Error(GL.GetShaderInfoLog(this));

                throw new InvalidOperationException(message);
            }
        }
    }

    [ContractClassFor(typeof(Shader))]
    abstract class ShaderProgramContracts : Shader
    {
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

        protected override IEnumerable<int> GetSamplerUniformLocations()
        {
            Contract.Ensures(Contract.Result<IEnumerable<int>>().Count() == this.SamplerCount);

            return null;
        }
    }
}
