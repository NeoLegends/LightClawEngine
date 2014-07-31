using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    [ContractClass(typeof(ShaderProgramContracts))]
    public abstract class Shader : GLObject, IBindable, ILateUpdateable, IUpdateable
    {
        private static ILog logger = LogManager.GetLogger(typeof(Shader));

        private object updateLock = new object();

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        private Component _Component;

        public Component Component
        {
            get
            {
                return _Component;
            }
            internal set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Component, value);
            }
        }

        public bool IsLinked { get; private set; }

        public abstract int SamplerCount { get; protected set; }

        public int ShaderCount { get; private set; }

        public ImmutableList<ShaderStage> Stages { get; private set; }

        public int TotalUniformLocationCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return this.SamplerCount + this.UniformLocationCount;
            }
        }

        public abstract int UniformLocationCount { get; protected set; }

        private Shader() : base(GL.CreateProgram()) { }

        public Shader(IEnumerable<ShaderStage> shaders, Component component)
            : this()
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentNullException>(component != null);
            Contract.Requires<ArgumentException>(!shaders.Duplicates(shader => shader.Type));

            logger.Info("Initializing a new shader with {0} stages.".FormatWith(shaders.Count()));

            this.Component = component;
            this.Stages = shaders.ToImmutableList();
            this.ShaderCount = this.Stages.Count;
        }

        public void Bind()
        {
            if (this.IsLinked)
            {
                GL.UseProgram(this);
            }
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
                logger.Debug("Linking shader on thread {0}.".FormatWith(System.Threading.Thread.CurrentThread.ManagedThreadId));

                foreach (ShaderStage shader in this.Stages)
                {
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

                logger.Debug("Shader linked.");
            }
        }

        public void Update(GameTime gameTime)
        {
            lock (this.updateLock)
            {
                this.Raise(this.Updating);
                this.OnUpdate(gameTime);
                this.Raise(this.Updated);
            }
        }

        public void LateUpdate()
        {
            lock (this.updateLock)
            {
                this.Raise(this.LateUpdating);
                this.OnLateUpdate();
                this.Raise(this.LateUpdated);
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.Unbind();
            foreach (ShaderStage shader in this.Stages)
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

        protected abstract void OnUpdate(GameTime gameTime);

        protected abstract void OnLateUpdate();
    }

    [ContractClassFor(typeof(Shader))]
    abstract class ShaderProgramContracts : Shader
    {
        public ShaderProgramContracts() 
            : base(Enumerable.Empty<ShaderStage>(), null) 
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
