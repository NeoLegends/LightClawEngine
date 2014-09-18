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
    public class Shader : GLObject, IInitializable
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

        private string _Source;

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

        private ShaderType _Type;

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

        private ImmutableArray<VertexAttributeDescription> _VertexAttributeDescriptions;

        public ImmutableArray<VertexAttributeDescription> VertexAttributeDescriptions
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableArray<VertexAttributeDescription>>() != null);

                return _VertexAttributeDescriptions;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _VertexAttributeDescriptions, value);
            }
        }

        public Shader(string source, ShaderType type)
            : this(source, type, VertexAttributeDescription.Empty)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(ShaderType), type));
        }

        public Shader(string source, ShaderType type, IEnumerable<VertexAttributeDescription> vad)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(ShaderType), type));
            Contract.Requires<ArgumentNullException>(vad != null);

            this.Source = source;
            this.Type = type;
            this.VertexAttributeDescriptions = vad.ToImmutableArray();
        }

        public void AttachTo(ShaderProgram program)
        {
            Contract.Requires<ArgumentNullException>(program != null);

            this.Initialize();
            GL.AttachShader(program, this);
        }

        public void DetachFrom(ShaderProgram program)
        {
            Contract.Requires<ArgumentNullException>(program != null);

            GL.DetachShader(program, this);
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Handle = GL.CreateShader(this.Type);
                        GL.ShaderSource(this, this.Source);
                        GL.CompileShader(this);

                        int result;
                        GL.GetShader(this, ShaderParameter.CompileStatus, out result);
                        if (result == 0)
                        {
                            string message = "{0} could not be compiled. Info log: '{1}'.".FormatWith(typeof(Shader).Name, GL.GetShaderInfoLog(this));
                            Logger.Warn(message);
                            throw new InvalidOperationException(message);
                        }

                        this.IsInitialized = true;
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                GL.DeleteShader(this);

                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(this._Source));
            Contract.Invariant(this._VertexAttributeDescriptions != null);
        }
    }
}
