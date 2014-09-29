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
    /// <summary>
    /// Represents an OpenGL shader program.
    /// </summary>
    /// <seealso href="http://www.opengl.org/wiki/Program_Object"/>
    public class ShaderProgram : GLObject, IBindable, IInitializable
    {
        /// <summary>
        /// The object used to lock access to <see cref="M:Initialize"/>.
        /// </summary>
        private readonly object initializationLock = new object();

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _IsInitialized;

        /// <summary>
        /// Indicates whether the instance has already been initialized.
        /// </summary>
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

        /// <summary>
        /// Backing field.
        /// </summary>
        private ImmutableArray<Shader> _Shaders;

        /// <summary>
        /// The <see cref="Shader"/>s the <see cref="ShaderProgram"/> consists of.
        /// </summary>
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

        /// <summary>
        /// Backing field.
        /// </summary>
        private ImmutableArray<Uniform> _Uniforms;

        /// <summary>
        /// The <see cref="Uniform"/>s of the <see cref="ShaderProgram"/>.
        /// </summary>
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

        /// <summary>
        /// Initializes a new <see cref="ShaderProgram"/> from a set of <see cref="Shader"/>s.
        /// </summary>
        /// <param name="shaders">The <see cref="Shader"/>s to initialize from.</param>
        public ShaderProgram(params Shader[] shaders)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentException>(shaders.Any());
            Contract.Requires<ArgumentException>(shaders.All(shader => shader != null));

            this.Shaders = shaders.ToImmutableArray();
        }

        /// <summary>
        /// Binds the <see cref="ShaderProgram"/> to the graphics context.
        /// </summary>
        public void Bind()
        {
            this.Initialize();
            GL.UseProgram(this);
        }

        /// <summary>
        /// Initializes the <see cref="ShaderProgram"/> compiling and linking the <see cref="Shader"/>s.
        /// </summary>
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

        /// <summary>
        /// Unbinds the <see cref="ShaderProgram"/> from the graphics context.
        /// </summary>
        public void Unbind()
        {
            GL.UseProgram(0);
        }

        /// <summary>
        /// Disposes the <see cref="ShaderProgram"/> freeing all OpenGL resources.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose of managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                lock (this.initializationLock)
                {
                    if (this.IsInitialized)
                    {
                        GL.DeleteProgram(this);
                    }
                }
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Shaders != null);
            Contract.Invariant(this._Uniforms != null);
        }
    }
}
