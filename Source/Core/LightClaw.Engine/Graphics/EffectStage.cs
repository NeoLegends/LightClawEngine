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
    public class EffectStage : Entity, IInitializable
    {
        private readonly object initializationLock = new object();

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

        private UniformBufferPool _UboPool;

        public UniformBufferPool UboPool
        {
            get
            {
                Contract.Ensures(Contract.Result<UniformBufferPool>() != null);

                return _UboPool;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _UboPool, value);
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

                this.SetProperty(ref _Uniforms, value);
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
            [ContractVerification(false)]
            get
            {
                Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

                this.Initialize();
                IEnumerable<EffectUniform> values = this.Uniforms.Values;
                if (values == null)
                {
                    throw new NullReferenceException("The collection containing the values of the dictionary containing the uniforms was null.");
                }
                return values.FilterNull().First(uniform => uniform.Location == location);
            }
        }

        public EffectStage(ShaderProgram program)
            : this(program, UniformBufferPool.Default)
        {
            Contract.Requires<ArgumentNullException>(program != null);
        }

        public EffectStage(ShaderProgram program, UniformBufferPool uboPool)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(uboPool != null);

            this.ShaderProgram = program;
            this.UboPool = uboPool;
        }

        public void Initialize()
        {
            if (!this.IsInitialized) // Double check to avoid lock acquiring, if possible
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        throw new NotImplementedException();

                        int uniformCount = 0;
                        GL.GetProgram(this.ShaderProgram, GetProgramParameterName.ActiveUniforms, out uniformCount);
                        for (int i = 0; i < uniformCount; i++)
                        {
                            int nameLength;
                            ActiveUniformType uniformType;
                            string uniformName = GL.GetActiveUniform(this.ShaderProgram, i, out nameLength, out uniformType);
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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._ShaderProgram != null);
            Contract.Invariant(this._UboPool != null);
            Contract.Invariant(this._Uniforms != null);
        }

        public static implicit operator ShaderProgram(EffectStage stage)
        {
            return (stage != null) ? stage.ShaderProgram : null;
        }
    }
}
