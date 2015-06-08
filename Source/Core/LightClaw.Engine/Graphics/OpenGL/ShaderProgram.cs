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
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents an OpenGL shader program.
    /// </summary>
    /// <seealso href="http://www.opengl.org/wiki/Program_Object"/>
    [DebuggerDisplay("Attribute Count: {Attributes.Count}, Uniform Count: {Uniforms.Count}")]
    public class ShaderProgram : GLObject, IBindable
    {
        private ImmutableDictionary<string, int> _Attributes;

        /// <summary>
        /// A list of all vertex attributes.
        /// </summary>
        public ImmutableDictionary<string, int> Attributes
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableDictionary<string, int>>() != null);

                return _Attributes;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Attributes, value);
            }
        }

        private ImmutableDictionary<string, Uniform> _Uniforms;

        /// <summary>
        /// The <see cref="Uniform"/>s of the <see cref="ShaderProgram"/>.
        /// </summary>
        public ImmutableDictionary<string, Uniform> Uniforms
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableDictionary<string, Uniform>>() != null);

                return _Uniforms;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(Enumerable.All(value, kvp => kvp.Value != null));

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

            this.VerifyAccess();

            this.Handle = GL.CreateProgram();
            try
            {
                foreach (Shader s in shaders)
                {
                    s.AttachTo(this);
                }

                GL.ProgramParameter(this, ProgramParameterName.ProgramBinaryRetrievableHint, 1);
                GL.ProgramParameter(this, ProgramParameterName.ProgramSeparable, 1);
                GL.LinkProgram(this);

                int result;
                GL.GetProgram(this, GetProgramParameterName.LinkStatus, out result);
                if (result != GLBool.True)
                {
                    string infoLog = this.GetInfoLog();
                    string message = "Linking the {0} failed. Info log: '{1}'.".FormatWith(typeof(ShaderProgram).Name, infoLog);
                    Log.Error(message);
                    throw new LinkingFailedException(message, infoLog, result);
                }

                int size;
                ActiveAttribType aat;
                this.Attributes = Enumerable.Range(0, this.GetInterface(ProgramInterface.ProgramInput, ProgramInterfaceParameter.ActiveResources))
                                            .Select(i => new KeyValuePair<string, int>(GL.GetActiveAttrib(this, i, out size, out aat), i))
                                            .ToImmutableDictionary();
                this.Uniforms = Enumerable.Range(0, this.GetInterface(ProgramInterface.Uniform, ProgramInterfaceParameter.ActiveResources))
                                          .Select(uniformIndex => new Uniform(this, uniformIndex))
                                          .ToImmutableDictionary(uniform => uniform.Name);
            }
            finally
            {
                foreach (Shader s in shaders)
                {
                    s.DetachFrom(this);
                }
            }
        }

        /// <summary>
        /// Binds the <see cref="ShaderProgram"/> to the graphics context.
        /// </summary>
        public Binding Bind()
        {
            this.VerifyAccess();
            GL.UseProgram(this);
            return new Binding(this);
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
            this.VerifyAccess();
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
            this.VerifyAccess();

            int result;
            GL.GetProgramInterface(this, programInterface, interfaceParameter, out result);
            return result;
        }

        /// <summary>
        /// Unbinds the <see cref="ShaderProgram"/> from the graphics context.
        /// </summary>
        public void Unbind()
        {
            this.VerifyAccess();
            GL.UseProgram(0);
        }

        /// <summary>
        /// Validates the <see cref="ShaderProgram"/>.
        /// </summary>
        /// <remarks>
        /// Warning, this is a lengthy operation, execute only during development.
        /// </remarks>
        public void Validate()
        {
            this.VerifyAccess();
            GL.ValidateProgram(this);

            int result;
            GL.GetProgram(this, GetProgramParameterName.ValidateStatus, out result);
            if (result != GLBool.True)
            {
                string infoLog = this.GetInfoLog();
                string message = "Validating the {0} failed. Info log: '{1}'.".FormatWith(typeof(ShaderProgram).Name, infoLog);
                Log.Error(message);
                throw new ValidationFailedException(message, infoLog, result);
            }
        }

        /// <summary>
        /// Disposes the <see cref="ShaderProgram"/> freeing all OpenGL resources.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose of managed resources as well.</param>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        protected override void Dispose(bool disposing)
        {
            this.Dispatcher.ImmediateOr(this.DeleteShaderProgram, disposing, DispatcherPriority.Background);
        }

        [System.Security.SecurityCritical]
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private void DeleteShaderProgram(bool disposing)
        {
            try
            {
                GL.DeleteProgram(this);
            }
            catch (Exception ex)
            {
                Log.Warn("An {0} was thrown while disposing of a {1}. This might or might not be an unwanted condition.".FormatWith(ex.GetType().Name, typeof(ShaderProgram).Name), ex);
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Attributes != null);
            Contract.Invariant(this._Uniforms != null);
        }
    }
}
