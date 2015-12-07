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
using LightClaw.Engine.IO;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a shader, a linkable part of a <see cref="ShaderProgram"/>.
    /// </summary>
    /// <remarks>
    /// A shader is only one stage / one part of a shader program. Shaders themselves cannot be used to draw an object,
    /// at least a vertex and a fragment shader need to be linked to a <see cref="ShaderProgram"/> that can be bound to
    /// the graphics pipeline to draw.
    /// </remarks>
    /// <seealso href="http://www.opengl.org/wiki/Shader"/>
    /// <seealso cref="ShaderProgram"/>
    [ContentReader(typeof(ShaderReader))]
    [DebuggerDisplay("Type: {Type}, Name: {Name}")]
    public class Shader : GLObject
    {
        /// <summary>
        /// Backing field.
        /// </summary>
        private ShaderType _Type;

        /// <summary>
        /// The type of the shader.
        /// </summary>
        public ShaderType Type
        {
            get
            {
                return _Type;
            }
            private set
            {
                this.SetProperty(ref _Type, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Shader"/>.
        /// </summary>
        /// <param name="source">The shader source code.</param>
        /// <param name="type">The shader type.</param>
        public Shader(string source, ShaderType type)
            : this(null, source, type)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(ShaderType), type));
        }

        /// <summary>
        /// Initializes a new <see cref="Shader"/>.
        /// </summary>
        /// <param name="name">The <see cref="Shader"/>s name.</param>
        /// <param name="source">The shader source code.</param>
        /// <param name="type">The shader type.</param>
        public Shader(string name, string source, ShaderType type)
            : base(name)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(ShaderType), type));

            this.VerifyAccess();

            this.Type = type;

            this.Handle = GL.CreateShader(this.Type);
            GL.ShaderSource(this, source);
            GL.CompileShader(this);

            int result;
            GL.GetShader(this, ShaderParameter.CompileStatus, out result);
            if (result == GLBool.False)
            {
                string infoLog = GL.GetShaderInfoLog(this);
                string message = "{0} could not be compiled. Info log: '{1}'.".FormatWith(typeof(Shader).Name, infoLog);
                Log.Error(message);
                throw new CompilationFailedException(message, infoLog, result);
            }
        }

        /// <summary>
        /// Attaches the <see cref="Shader"/> to the specified <see cref="ShaderProgram"/>.
        /// </summary>
        /// <remarks>Triggers initialization.</remarks>
        /// <param name="program">The <see cref="ShaderProgram"/> to attach the shader to.</param>
        public void AttachTo(ShaderProgram program)
        {
            Contract.Requires<ArgumentNullException>(program != null);

            this.VerifyAccess();
            GL.AttachShader(program, this);
        }

        /// <summary>
        /// Detaches the <see cref="Shader"/> from the specified <see cref="ShaderProgram"/>.
        /// </summary>
        /// <remarks>Triggers initialization.</remarks>
        /// <param name="program">The <see cref="ShaderProgram"/> to detach from.</param>
        public void DetachFrom(ShaderProgram program)
        {
            Contract.Requires<ArgumentNullException>(program != null);

            this.VerifyAccess();
            GL.DetachShader(program, this);
        }

        /// <summary>
        /// Disposes the <see cref="Shader"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources as well.</param>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        protected override void Dispose(bool disposing)
        {
            this.Dispatcher.ImmediateOr(this.DeleteShader, disposing, DispatcherPriority.Background);
        }

        [System.Security.SecurityCritical]
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private void DeleteShader(bool disposing)
        {
            try
            {
                GL.DeleteShader(this);
            }
            catch (Exception ex)
            {
                Log.Warn(
                    ex,
                    "A {0} was thrown while disposing of a {1}. In most cases, this should be nothing to worry about. Check the error message to make sure there really is nothing to worry about, though.", 
                    ex.GetType().Name, typeof(Shader).Name
                );
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
