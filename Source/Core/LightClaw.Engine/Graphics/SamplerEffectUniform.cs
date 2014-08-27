using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class SamplerEffectUniform : EffectUniform
    {
        private readonly object setLock = new object();

        private int _TextureUnit = 0;

        public int TextureUnit
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _TextureUnit;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _TextureUnit, value);
            }
        }

        public SamplerEffectUniform(EffectStage stage, string name, int textureUnit)
            : base(stage, name)
        {
            Contract.Requires<ArgumentNullException>(stage != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            this.TextureUnit = textureUnit;
        }

        public void Set(Texture texture, Sampler sampler)
        {
            Contract.Requires<ArgumentNullException>(texture != null);

            this.Set(this.TextureUnit, texture, sampler);
        }

        public void Set(int textureUnit, Texture texture, Sampler sampler)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);
            Contract.Requires<ArgumentNullException>(texture != null);

            lock (this.setLock)
            {
                this.TextureUnit = textureUnit;
                GL.ProgramUniform1(this.Stage.ShaderProgram, this.Location, textureUnit);
                texture.Bind(textureUnit);
                if (sampler != null)
                {
                    sampler.Bind(textureUnit);
                }
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._TextureUnit >= 0);
        }
    }
}
