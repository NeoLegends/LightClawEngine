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
    public class EffectStage : GLObject
    {
        private readonly object compileLock = new object();

        private bool _IsCompiled = false;

        public bool IsCompiled
        {
            get
            {
                return _IsCompiled;
            }
            private set
            {
                this.SetProperty(ref _IsCompiled, value);
            }
        }

        private string _Source;

        public string Source
        {
            get
            {
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

        private UniformBufferPool _UboPool = UniformBufferPool.Default;

        public UniformBufferPool UboPool
        {
            get
            {
                return _UboPool;
            }
            set
            {
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

                return this.Uniforms[uniformName];
            }
        }

        public EffectUniform this[int location]
        {
            [ContractVerification(false)]
            get
            {
                Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

                IEnumerable<EffectUniform> values = this.Uniforms.Values;
                if (values == null)
                {
                    throw new NullReferenceException("The collection containing the values of the dictionary containing the uniforms was null.");
                }
                return values.FilterNull().First(uniform => uniform.Location == location);
            }
        }

        public EffectStage() { }

        public EffectStage(string source, ShaderType type)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));

            this.Compile(source, type);
        }

        public void Compile(string source, ShaderType type)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));

            if (!this.IsCompiled) // Double check to avoid lock acquiring, if possible
            {
                lock (this.compileLock)
                {
                    if (!this.IsCompiled)
                    {
                        this.Source = source;
                        this.Type = type;
                        this.Handle = GL.CreateShaderProgram(this.Type, 1, this.Source.YieldArray());

                        int result;
                        if (!this.CheckStatus(GetProgramParameterName.LinkStatus, out result))
                        {
                            throw new CompilationFailedException(
                                "Compiling the {0}'s underlying OpenGL Shader Program failed with code {1}.".FormatWith(typeof(EffectStage).Name, result),
                                GL.GetProgramInfoLog(this),
                                result
                            );
                        }
                        throw new NotImplementedException("Shader compilation seems to work fine (no exception yet), but uniform variable handling is not finished yet.");

#pragma warning disable 0162 // Unreachable code, remove when NotImplementedException is gone

                        int uniformCount = 0;
                        GL.GetProgram(this, GetProgramParameterName.ActiveUniforms, out uniformCount);
                        for (int i = 0; i < uniformCount; i++)
                        {
                            int nameLength;
                            ActiveUniformType uniformType;
                            string uniformName = GL.GetActiveUniform(this, i, out nameLength, out uniformType);
                        }

#pragma warning restore 0162

                        this.IsCompiled = true;
                        return;
                    }
                }
            }

            throw new NotSupportedException("Compiling an {0} two times is not supported (as the source-parameter might be different). Create a new one instead.".FormatWith(typeof(EffectStage).Name));
        }

        public bool TryGetUniform(string name, out EffectUniform uniform)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
            Contract.Ensures(!Contract.Result<bool>() || Contract.ValueAtReturn(out uniform) != null);

            return this.Uniforms.TryGetValue(name, out uniform);
        }

        public bool TryGetUniform(int location, out EffectUniform uniform)
        {
            Contract.Requires<ArgumentOutOfRangeException>(location >= 0);
            Contract.Ensures(!Contract.Result<bool>() || Contract.ValueAtReturn(out uniform) != null);

            IEnumerable<EffectUniform> values = this.Uniforms.Values;
            if (values == null)
            {
                throw new NullReferenceException("The collection containing the values of the dictionary containing the uniforms was null.");
            }
            return (uniform = values.FilterNull().FirstOrDefault(u => u.Location == location)) != null;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.DeleteProgram(this.Handle);
            }
            catch (Exception ex)
            {
                logger.Warn("An error occured while disposing an {0}' underlying OpenGL Shader Program.".FormatWith(typeof(EffectStage).Name), ex);
            }
            base.Dispose(disposing);
        }

        private bool CheckStatus(GetProgramParameterName pName)
        {
            return (QueryValue(pName) == 1);
        }

        private bool CheckStatus(GetProgramParameterName pName, out int result)
        {
            result = this.QueryValue(pName);
            return (result == 1);
        }

        private int QueryValue(GetProgramParameterName pName)
        {
            int result;
            GL.GetProgram(this, pName, out result);
            return result;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Uniforms != null);
        }
    }
}
