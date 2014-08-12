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

        private bool _IsCompiled;

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

        private ImmutableDictionary<string, EffectUniform> _Uniforms;

        public ImmutableDictionary<string, EffectUniform> Uniforms
        {
            get
            {
                return _Uniforms;
            }
            private set
            {
                this.SetProperty(ref _Uniforms, value);
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
            get
            {
                return this.Uniforms.Values.First(uniform => uniform.Location == location);
            }
        }

        //public EffectStage(string source, ShaderType type, IEnumerable<EffectUniform> uniforms)
        //{
        //    Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(source));

        //    this.Source = source;
        //    this.Type = type;
        //    this.Uniforms = (uniforms != null) ?
        //        uniforms.ToImmutableDictionary(uniform => uniform.UniformName) :
        //        ImmutableDictionary<string, EffectUniform>.Empty;
        //}

        public void Compile()
        {
            if (!this.IsCompiled) // Double check to avoid lock acquiring, if possible
            {
                lock (this.compileLock)
                {
                    if (!this.IsCompiled)
                    {
                        this.Handle = GL.CreateShaderProgram(this.Type, 1, this.Source.YieldArray());
                        this.IsCompiled = true;

                        int result;
                        if (!this.CheckStatus(GetProgramParameterName.LinkStatus, out result))
                        {
                            throw new InvalidOperationException(
                                "Compiling the {0}'s underlying OpenGL Shader Program failed with code {1}.".FormatWith(typeof(EffectStage).Name, result)
                            );
                        }
                    }
                }
            }
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
    }
}
