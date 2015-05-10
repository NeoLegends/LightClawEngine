using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.IO;

namespace LightClaw.Engine.Graphics
{
    [ContentReader(typeof(EffectPassReader))]
    public sealed class EffectPass : DisposableEntity, IBindable
    {
        private readonly bool ownsProgram;

        private ImmutableDictionary<string, DataEffectUniform> _DataUniforms;

        public ImmutableDictionary<string, DataEffectUniform> DataUniforms
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableDictionary<string, DataEffectUniform>>() != null);

                return _DataUniforms ?? (_DataUniforms = this.Uniforms.Where(kvp => kvp.Value is DataEffectUniform)
                                                                      .ToImmutableDictionary(kvp => kvp.Key, kvp => (DataEffectUniform)kvp.Value));
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

        private ImmutableDictionary<string, SamplerEffectUniform> _SamplerUniforms;

        public ImmutableDictionary<string, SamplerEffectUniform> SamplerUniforms
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableDictionary<string, SamplerEffectUniform>>() != null);

                return _SamplerUniforms ?? (_SamplerUniforms = this.Uniforms.Where(kvp => kvp.Value is SamplerEffectUniform)
                                                                            .ToImmutableDictionary(kvp => kvp.Key, kvp => (SamplerEffectUniform)kvp.Value));
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

        private ImmutableDictionary<string, EffectUniform> _Uniforms;

        public ImmutableDictionary<string, EffectUniform> Uniforms
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableDictionary<string, EffectUniform>>() != null);

                return _Uniforms;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Uniforms, value);
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
            this.Uniforms = this.ShaderProgram.Uniforms.Select((Func<KeyValuePair<string, Uniform>, EffectUniform>)(kvp =>
            {
                if (kvp.Value.Type.IsSampler())
                {
                    return new SamplerEffectUniform(this, kvp.Value, 0);
                }
                else if (kvp.Value.Type.IsPrimitiveValue() || kvp.Value.Type.IsVector() || kvp.Value.Type.IsMatrix())
                {
                    return new DataEffectUniform(this, kvp.Value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            })).ToImmutableDictionary(eu => eu.Name);
        }

        public void Bind()
        {
            this.ShaderProgram.Bind();
            foreach (EffectUniform uniform in this.Uniforms.Values)
            {
                uniform.Bind();
            }
        }

        public void Unbind()
        {
            foreach (EffectUniform uniform in this.Uniforms.Values)
            {
                uniform.Unbind();
            }
            this.ShaderProgram.Unbind();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (ownsProgram)
                {
                    this.ShaderProgram.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._ShaderProgram != null);
            Contract.Invariant(this._Uniforms != null);
        }

        public static implicit operator ShaderProgram(EffectPass pass)
        {
            return (pass != null) ? pass.ShaderProgram : null;
        }
    }
}
