using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    [ContentReader(typeof(EffectPassReader))]
    public sealed class EffectPass : DisposableEntity, IBindable, IInitializable
    {
        private readonly object initializationLock = new object();

        private readonly bool ownsProgram;

        private bool _IsInitialized = false;

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

        private string _PassName;

        public string PassName
        {
            get
            {
                return _PassName;
            }
            private set
            {
                this.SetProperty(ref _PassName, value);
            }
        }

        private ShaderProgram _ShaderProgram;

        public ShaderProgram ShaderProgram
        {
            get
            {
                Contract.Ensures(Contract.Result<ShaderProgram>() != null);

                return _ShaderProgram;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _ShaderProgram, value);
            }
        }

        public EffectPass(ShaderProgram program)
            : this(program, false)
        {
            Contract.Requires<ArgumentNullException>(program != null);
        }

        public EffectPass(ShaderProgram program, bool ownsProgram) 
        {
            Contract.Requires<ArgumentNullException>(program != null);

            this.ownsProgram = ownsProgram;
            this.ShaderProgram = program;
        }

        public void Bind()
        {
            this.Initialize();
            this.ShaderProgram.Bind();
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.ShaderProgram.Uniforms.Select((Func<Uniform, EffectUniform>)(uniform =>
                        {
                            throw new NotImplementedException();
                            if (uniform.Type.IsSampler())
                            {
                                return new SamplerEffectUniform(this, uniform, 0);
                            }
                            else if (uniform.Type.IsVector() || uniform.Type.IsMatrix())
                            {
                                return new DataEffectUniform(this, uniform, 0);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }));
                        
                        this.IsInitialized = true;
                    }
                }
            }
        }

        public void Unbind()
        {
            this.ShaderProgram.Unbind();
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (ownsProgram)
                {
                    this.ShaderProgram.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._ShaderProgram != null);
        }

        public static implicit operator ShaderProgram(EffectPass pass)
        {
            return (pass != null) ? pass.ShaderProgram : null;
        }
    }
}
