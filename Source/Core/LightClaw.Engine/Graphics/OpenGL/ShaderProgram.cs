using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents an OpenGL shader program.
    /// </summary>
    /// <seealso href="http://www.opengl.org/wiki/Program_Object"/>
    [DebuggerDisplay("Name = {Name}, Handle = {Handle}, Uniform Count = {Uniforms.Count}")]
    public class ShaderProgram : GLObject, IBindable
    {
        /// <summary>
        /// The object used to lock access to <see cref="M:Initialize"/>.
        /// </summary>
        private readonly object initializationLock = new object();

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
        private ImmutableDictionary<string, Uniform> _Uniforms;

        /// <summary>
        /// The <see cref="Uniform"/>s of the <see cref="ShaderProgram"/>.
        /// </summary>
        public ImmutableDictionary<string, Uniform> Uniforms
        {
            get
            {
                Contract.Ensures(!this.IsInitialized || Contract.Result<ImmutableDictionary<string, Uniform>>() != null);

                return _Uniforms;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.All(kvp => kvp.Value != null));

                this.SetProperty(ref _Uniforms, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="ShaderProgram"/> from a set of <see cref="Shader"/>s.
        /// </summary>
        /// <param name="shaders">The <see cref="Shader"/>s to initialize from.</param>
        public ShaderProgram(params Shader[] shaders)
            : this(null, shaders)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentException>(shaders.Any());
            Contract.Requires<ArgumentException>(shaders.All(shader => shader != null));
        }

        /// <summary>
        /// Initializes a new <see cref="ShaderProgram"/> from a set of <see cref="Shader"/>s.
        /// </summary>
        /// <param name="name">The <see cref="ShaderProgram"/>s name.</param>
        /// <param name="shaders">The <see cref="Shader"/>s to initialize from.</param>
        public ShaderProgram(string name, params Shader[] shaders)
            : base(name)
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
        /// Gets the OpenGL info log. See remarks.
        /// </summary>
        /// <remarks>
        /// This method, as all <c>GL.Get***</c>-methods, stalls the rendering pipeline and forces all previously submitted
        /// commands to be executed synchronously. Use with caution.
        /// </remarks>
        /// <returns>The progam info log.</returns>
        public string GetInfoLog()
        {
            return GL.GetProgramInfoLog(this);
        }

        /// <summary>
        /// Queries the interface of the program. See remarks.
        /// </summary>
        /// <remarks>
        /// This method, as all <c>GL.Get***</c>-methods, stalls the rendering pipeline and forces all previously submitted
        /// commands to be executed synchronously. Use with caution.
        /// </remarks>
        /// <param name="programInterface">The interface to query.</param>
        /// <param name="interfaceParameter">The name of the interface parameter to query.</param>
        /// <returns>The value.</returns>
        public int GetInterface(ProgramInterface programInterface, ProgramInterfaceParameter interfaceParameter)
        {
            int result;
            GL.GetProgramInterface(this, programInterface, interfaceParameter, out result);
            return result;
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
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        protected override void Dispose(bool disposing)
        {
            lock (this.initializationLock)
            {
                if (this.IsInitialized)
                {
                    try
                    {
                        GL.DeleteProgram(this);
                    }
                    catch (AccessViolationException ex)
                    {
                        Log.Warn("An {0} was thrown while disposing of a {1}. This might or might not be an unwanted condition.".FormatWith(ex.GetType().Name, typeof(ShaderProgram).Name), ex);
                    }
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialization callback.
        /// </summary>
        protected override void OnInitialize()
        {
            Log.Debug(() => "Initializing {0}.".FormatWith(typeof(ShaderProgram).Name));

            this.Handle = GL.CreateProgram();
            try
            {
                foreach (Shader s in this.Shaders)
                {
                    s.AttachTo(this);
                }

                GL.ProgramParameter(this, ProgramParameterName.ProgramBinaryRetrievableHint, 1);
                GL.ProgramParameter(this, ProgramParameterName.ProgramSeparable, 1);
                foreach (VertexAttributeDescription desc in this.Shaders.SelectMany(shader => shader.VertexAttributeDescriptions))
                {
                    desc.ApplyIn(this);
                }
                GL.LinkProgram(this);

                int result;
                GL.GetProgram(this, GetProgramParameterName.LinkStatus, out result);
                if (result == GLBool.False)
                {
                    string infoLog = this.GetInfoLog();
                    string message = "Linking the {0} failed. Info log: '{1}'.".FormatWith(typeof(ShaderProgram).Name, infoLog);
                    Log.Error(message);
                    throw new LinkingFailedException(message, infoLog, result);
                }

                int uniformCount = 0;
                GL.GetProgramInterface(this, ProgramInterface.Uniform, ProgramInterfaceParameter.ActiveResources, out uniformCount);
                this.Uniforms = Enumerable.Range(0, uniformCount)
                                          .Select(uniformIndex => new Uniform(this, uniformIndex))
                                          .ToImmutableDictionary(uniform => uniform.Name);
            }
            finally
            {
                foreach (Shader s in this.Shaders)
                {
                    s.DetachFrom(this);
                }
            }
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Shaders != null);
            Contract.Invariant(!this.IsInitialized || this._Uniforms != null);
        }
    }
}
