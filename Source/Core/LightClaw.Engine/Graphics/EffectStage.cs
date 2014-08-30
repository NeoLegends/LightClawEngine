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

namespace LightClaw.Engine.Graphics
{
    public class EffectStage : DisposableEntity, IInitializable
    {
        private readonly object initializationLock = new object();

        private readonly bool ownsShaderProgram;

        private ImmutableDictionary<string, EffectAttribute> _Attributes = ImmutableDictionary<string, EffectAttribute>.Empty;

        public ImmutableDictionary<string, EffectAttribute> Attributes
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableDictionary<string, EffectAttribute>>() != null);

                return _Attributes;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.Values.All(attribute => attribute != null));

                this.SetProperty(ref _Attributes, value);
            }
        }

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

        private EffectPass _Pass;

        public EffectPass Pass
        {
            get
            {
                Contract.Ensures(Contract.Result<EffectPass>() != null);

                return _Pass;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Pass, value);
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

        private ImmutableDictionary<string, EffectUniform> _Uniforms = ImmutableDictionary<string, EffectUniform>.Empty;

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
                Contract.Requires<ArgumentException>(value.Values.All(uniform => uniform != null));

                this._Samplers = null;
                this._Values = null;
                this.SetProperty(ref _Uniforms, value);
            }
        }

        private ImmutableDictionary<string, SamplerEffectUniform> _Samplers;

        public ImmutableDictionary<string, SamplerEffectUniform> Samplers
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableDictionary<string, SamplerEffectUniform>>() != null);

                return _Samplers ?? (_Samplers = this.GetUniformsOfType<SamplerEffectUniform>(this.Uniforms));
            }
        }

        private ImmutableDictionary<string, ValueEffectUniform> _Values;

        public ImmutableDictionary<string, ValueEffectUniform> Values
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableDictionary<string, ValueEffectUniform>>() != null);

                return _Values ?? (_Values = this.GetUniformsOfType<ValueEffectUniform>(this.Uniforms));
            }
        }

        private VertexArrayObject _VertexData;

        public VertexArrayObject VertexData
        {
            get
            {
                return _VertexData;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _VertexData, value);
            }
        }

        public EffectUniform this[string uniformName]
        {
            get
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(uniformName));

                this.Initialize();
                return this.Uniforms[uniformName];
            }
        }

        public EffectUniform this[int location]
        {
            get
            {
                Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

                this.Initialize();
                return this.Uniforms.Values.EnsureNonNull().FilterNull().First(uniform => uniform.Location == location);
            }
        }

        public EffectStage(EffectPass pass, ShaderProgram program, bool ownsProgram = false)
        {
            Contract.Requires<ArgumentNullException>(pass != null);
            Contract.Requires<ArgumentNullException>(program != null);

            this.ownsShaderProgram = ownsProgram;
            this.Pass = pass;
            this.ShaderProgram = program;
        }

        public void Initialize()
        {
            if (!this.IsInitialized) // Double check to avoid lock acquiring, if possible
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        // Attributes
                        if (this.ShaderProgram.Type == ShaderType.VertexShader)
                        {
                            int activeAttributeCount = 0;
                            GL.GetProgram(this.ShaderProgram, GetProgramParameterName.ActiveAttributes, out activeAttributeCount);

                            this.Attributes = Enumerable.Range(0, activeAttributeCount).Select(i =>
                            {
                                int nameLength;
                                int size;
                                ActiveAttribType attributeType;
                                StringBuilder sbName = new StringBuilder(32);
                                GL.GetActiveAttrib(this.ShaderProgram, i, int.MaxValue, out nameLength, out size, out attributeType, sbName);
                                string attributeName = sbName.ToString();

                                return new KeyValuePair<string, EffectAttribute>(
                                    attributeName,
                                    new EffectAttribute(this, attributeName, size)
                                );
                            }).ToImmutableDictionary();

                            throw new NotImplementedException("Vertex array objects still need to be generated after the attributes have been filled out.");
                        }

                        {   // Uniforms
                            int activeUniformCount = 0;
                            GL.GetProgram(this.ShaderProgram, GetProgramParameterName.ActiveUniforms, out activeUniformCount);

                            this.Uniforms = Enumerable.Range(0, activeUniformCount).Select(i =>
                            {
                                int nameLength;
                                ActiveUniformType uniformType;
                                string uniformName = GL.GetActiveUniform(this.ShaderProgram, i, out nameLength, out uniformType);

                                return new KeyValuePair<string, EffectUniform>(
                                    uniformName,
                                    uniformType.IsSamplerUniform() ?
                                        (EffectUniform)new SamplerEffectUniform(this, uniformName, this.Pass.TextureUnitManager.GetTextureUnit()) :
                                        (EffectUniform)new ValueEffectUniform(this, uniformName)
                                );
                            }).ToImmutableDictionary();
                        }

                        this.IsInitialized = true;
                    }
                }
            }
        }

        public bool TryGetUniform(string name, out EffectUniform uniform)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
            Contract.Ensures(!Contract.Result<bool>() || Contract.ValueAtReturn(out uniform) != null);

            this.Initialize();
            return this.Uniforms.TryGetValue(name, out uniform);
        }

        public bool TryGetUniform(int location, out EffectUniform uniform)
        {
            Contract.Requires<ArgumentOutOfRangeException>(location >= 0);
            Contract.Ensures(!Contract.Result<bool>() || Contract.ValueAtReturn(out uniform) != null);

            this.Initialize();
            return (uniform = this.Uniforms.Values.EnsureNonNull().FilterNull().FirstOrDefault(u => u.Location == location)) != null;
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                foreach (EffectUniform uniform in this.Uniforms.Values.FilterNull())
                {
                    uniform.Dispose();
                }
                if (this.ownsShaderProgram)
                {
                    this.ShaderProgram.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        private ImmutableDictionary<string, T> GetUniformsOfType<T>(ImmutableDictionary<string, EffectUniform> uniforms)
            where T : EffectUniform
        {
            return (from kvp in this.Uniforms
                    let samplerUniform = kvp.Value as T
                    where samplerUniform != null
                    select new KeyValuePair<string, T>(kvp.Key, samplerUniform)).ToImmutableDictionary();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Pass != null);
            Contract.Invariant(this._ShaderProgram != null);
            Contract.Invariant(this._Uniforms != null);
        }

        public static implicit operator ShaderProgram(EffectStage stage)
        {
            return (stage != null) ? stage.ShaderProgram : null;
        }
    }
}
