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

        private int _TextureUnit = -1;

        public int TextureUnit
        {
            get
            {
                return _TextureUnit;
            }
            set
            {
                this.SetProperty(ref _TextureUnit, value);
            }
        }

        public SamplerEffectUniform() { }

        public SamplerEffectUniform(int textureUnit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            this.TextureUnit = textureUnit;
        }

        public void Set(Texture texture, Sampler sampler)
        {
            Contract.Requires<ArgumentNullException>(texture != null);
            Contract.Requires<ArgumentNullException>(sampler != null);

            int unit = this.TextureUnit;
            if (unit < 0)
            {
                throw new InvalidOperationException("The TextureUnit to bind to is invalid. It needs to be greater than zero and is {0}.".FormatWith(unit));
            }
            this.Set(unit, texture, sampler);
        }

        public void Set(int textureUnit, Texture texture, Sampler sampler)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);
            Contract.Requires<ArgumentNullException>(texture != null);
            Contract.Requires<ArgumentNullException>(sampler != null);

            lock (this.setLock)
            {
                this.TextureUnit = textureUnit;
                GL.ProgramUniform1(this.Stage, this.Location, textureUnit);
                texture.Bind(textureUnit);
                sampler.Bind(textureUnit);
            }
        }
    }
}
