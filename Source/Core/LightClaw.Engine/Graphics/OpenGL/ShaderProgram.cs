using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class ShaderProgram : GLObject, IBindable, IInitializable
    {
        private readonly object initializationLock = new object();

        private bool _IsInitialized;

        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            private set
            {
                this.SetProperty(ref _IsInitialized, value);
            }
        }

        private ImmutableArray<Shader> _Shaders;

        public ImmutableArray<Shader> Shaders
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableArray<Shader>>() != null);

                return _Shaders;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.All(shader => shader != null));

                this.SetProperty(ref _Shaders, value);
            }
        }

        private ImmutableArray<Uniform> _Uniforms;

        public ImmutableArray<Uniform> Uniforms
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableArray<Uniform>>() != null);

                return _Uniforms;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.All(uniform => uniform != null));

                this.SetProperty(ref _Uniforms, value);
            }
        }

        public ShaderProgram(params Shader[] shaders)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentException>(shaders.Any());
            Contract.Requires<ArgumentException>(shaders.All(shader => shader != null));

            this.Shaders = shaders.ToImmutableArray();
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        Logger.Info("Initializing shader program.");

                        this.Handle = GL.CreateProgram();
                        try
                        {
                            foreach (Shader s in this.Shaders)
                            {
                                s.AttachTo(this);
                            }

                            foreach (VertexAttributeDescription desc in this.Shaders.SelectMany(shader => shader.VertexAttributeDescriptions))
                            {
                                GL.BindAttribLocation(this, desc.Location, desc.Name);
                            }
                            GL.ProgramParameter(this, ProgramParameterName.ProgramBinaryRetrievableHint, 1);
                            GL.ProgramParameter(this, ProgramParameterName.ProgramSeparable, 1);
                            GL.LinkProgram(this);

                            int result;
                            GL.GetProgram(this, GetProgramParameterName.LinkStatus, out result);
                            if (result == 0)
                            {
                                string message = "Linking the {0} failed. Info log: '{1}'.".FormatWith(typeof(ShaderProgram).Name, GL.GetProgramInfoLog(this));
                                Logger.Warn(message);
                                throw new InvalidOperationException(message);
                            }

                            int uniformCount = 0;
                            GL.GetProgramInterface(this, ProgramInterface.Uniform, ProgramInterfaceParameter.ActiveResources, out uniformCount);
                            this.Uniforms = Enumerable.Range(0, uniformCount).Select(uniformIndex => new Uniform(this, uniformIndex)).ToImmutableArray();
                        }
                        finally
                        {
                            foreach (Shader s in this.Shaders)
                            {
                                s.DetachFrom(this);
                            }
                        }

                        this.IsInitialized = true;
                    }
                }
            }
        }

        public void Bind()
        {
            this.Initialize();
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
                GL.DeleteProgram(this);
            }
            catch (Exception ex)
            {
                Logger.Warn(() => "An exception of type '{0}' was thrown while disposing the {1}'s underlying OpenGL Shader Program.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(ShaderProgram).Name), ex);
            }
            base.Dispose(disposing);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Shaders != null);
            Contract.Invariant(this._Uniforms != null);
        }
    }
}
