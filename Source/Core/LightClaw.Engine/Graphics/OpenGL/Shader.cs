using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    [DebuggerDisplay("Name = {Name}, Handle = {Handle}, Type = {Type}, Vertex Attribute Count = {VertexAttributeDescriptions.Length}")]
    public class Shader : GLObject, IInitializable
    {
        /// <summary>
        /// Used to restrict access to the initialization method.
        /// </summary>
        private readonly object initializationLock = new object();

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _IsInitialized;

        /// <summary>
        /// Indicates whether the <see cref="Shader"/> has already been initialized or not.
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
        private string _Source;

        /// <summary>
        /// The shader source code.
        /// </summary>
        public string Source
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return _Source;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(value));

                this.SetProperty(ref _Source, value);
            }
        }

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
        /// Backing field.
        /// </summary>
        private ImmutableArray<VertexAttributeDescription> _VertexAttributeDescriptions;

        /// <summary>
        /// The vertex attribute descriptions declaring how the vertex data in the buffer shall be laid out inside the
        /// shader source.
        /// </summary>
        public ImmutableArray<VertexAttributeDescription> VertexAttributeDescriptions
        {
            get
            {
                return _VertexAttributeDescriptions;
            }
            private set
            {
                this.SetProperty(ref _VertexAttributeDescriptions, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Shader"/> without any vertex attributes.
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
        /// Initializes a new <see cref="Shader"/> without any vertex attributes.
        /// </summary>
        /// <param name="name">The <see cref="Shader"/>s name.</param>
        /// <param name="source">The shader source code.</param>
        /// <param name="type">The shader type.</param>
        public Shader(string name, string source, ShaderType type)
            : this(name, source, type, VertexAttributeDescription.Empty)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(ShaderType), type));
        }

        /// <summary>
        /// Initializes a new <see cref="Shader"/>.
        /// </summary>
        /// <param name="source">The shader source code.</param>
        /// <param name="type">The shader type.</param>
        /// <param name="vad">
        /// Vertex attribute descriptions declaring how the vertex data in the buffer shall be laid out inside the
        /// shader source.
        /// </param>
        public Shader(string source, ShaderType type, params VertexAttributeDescription[] vad)
            : this(null, source, type, vad)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(ShaderType), type));
            Contract.Requires<ArgumentNullException>(vad != null);
        }

        /// <summary>
        /// Initializes a new <see cref="Shader"/>.
        /// </summary>
        /// <param name="name">The <see cref="Shader"/>s name.</param>
        /// <param name="source">The shader source code.</param>
        /// <param name="type">The shader type.</param>
        /// <param name="vad">
        /// Vertex attribute descriptions declaring how the vertex data in the buffer shall be laid out inside the
        /// shader source.
        /// </param>
        public Shader(string name, string source, ShaderType type, params VertexAttributeDescription[] vad)
            : base(name)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(ShaderType), type));
            Contract.Requires<ArgumentNullException>(vad != null);

            this.Source = source;
            this.Type = type;
            this.VertexAttributeDescriptions = vad.ToImmutableArray();
        }

        /// <summary>
        /// Attaches the <see cref="Shader"/> to the specified <see cref="ShaderProgram"/>.
        /// </summary>
        /// <remarks>Triggers initialization.</remarks>
        /// <param name="program">The <see cref="ShaderProgram"/> to attach the shader to.</param>
        public void AttachTo(ShaderProgram program)
        {
            Contract.Requires<ArgumentNullException>(program != null);

            this.Initialize();
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

            this.Initialize();
            GL.DetachShader(program, this);
        }

        /// <summary>
        /// Initializes the <see cref="Shader"/>.
        /// </summary>
        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        Logger.Debug(() => "Initializing {0}.".FormatWith(typeof(Shader).Name));

                        this.Handle = GL.CreateShader(this.Type);
                        GL.ShaderSource(this, this.Source);
                        GL.CompileShader(this);

                        int result;
                        GL.GetShader(this, ShaderParameter.CompileStatus, out result);
                        if (result == 0)
                        {
                            string infoLog = GL.GetShaderInfoLog(this);
                            string message = "{0} could not be compiled. Info log: '{1}'.".FormatWith(typeof(Shader).Name, infoLog);
                            Logger.Warn(message);
                            throw new CompilationFailedException(message, infoLog, result);
                        }

                        this.IsInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Disposes the <see cref="Shader"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources as well.</param>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        protected override void Dispose(bool disposing)
        {
            lock (this.initializationLock)
            {
                if (this.IsInitialized)
                {
                    try
                    {
                        GL.DeleteShader(this);
                    }
                    catch (AccessViolationException ex)
                    {
                        Logger.Warn(e => "An {0} was thrown while disposing of a {1}. This might or might not be an unwanted condition.".FormatWith(e.GetType().Name, typeof(Shader).Name), ex, ex);
                    }
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(this._Source));
        }
    }
}
