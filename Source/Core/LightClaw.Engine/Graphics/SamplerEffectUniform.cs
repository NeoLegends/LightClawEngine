using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Extensions;

using LCTextureUnit = LightClaw.Engine.Graphics.OpenGL.TextureUnit;

namespace LightClaw.Engine.Graphics
{
    public class SamplerEffectUniform : EffectUniform, IBindable
    {
        private Sampler _Sampler;

        public Sampler Sampler
        {
            get
            {
                return _Sampler;
            }
            set
            {
                this.SetProperty(ref _Sampler, value);
            }
        }

        private Texture _Texture;

        public Texture Texture
        {
            get
            {
                return _Texture;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Texture, value);
            }
        }

        private LCTextureUnit _TextureUnit = 0;

        public LCTextureUnit TextureUnit
        {
            get
            {
                return _TextureUnit;
            }
            private set
            {
                this.SetProperty(ref _TextureUnit, value);
            }
        }

        public SamplerEffectUniform(EffectPass pass, Uniform uniform, LCTextureUnit textureUnit)
            : base(pass, uniform)
        {
            Contract.Requires<ArgumentNullException>(pass != null);
            Contract.Requires<ArgumentNullException>(uniform != null);

            this.TextureUnit = textureUnit;
        }

        public override void Bind()
        {
            Texture texture = this.Texture;
            if (texture != null)
            {
                this.Uniform.Set(this.TextureUnit);
                texture.Bind();

                Sampler sampler = this.Sampler;
                if (sampler != null)
                {
                    if (sampler.TextureUnit == texture.TextureUnit)
                    {
                        sampler.Bind();
                    }
                    else
                    {
                        Logger.Warn(() => "The texture unit of the sampler to bind ({0}) didn't match the texture unit of the texture ({1}). The sampler will not be bound.".FormatWith(sampler.TextureUnit, texture.TextureUnit));
                    }
                }
            }
            else
            {
                Logger.Warn(() => "Texture to bind to the sampler in the shader was null and thus will not be bound. This is presumably unwanted behaviour!");
            }
        }

        public override void Unbind()
        {
            Texture texture = this.Texture;
            if (texture != null)
            {
                texture.Unbind();
            }
            Sampler sampler = this.Sampler;
            if (sampler != null)
            {
                sampler.Unbind();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                this.TextureUnit.Dispose();

                base.Dispose(disposing);
            }
        }

        protected override void OnInitialize()
        {
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._TextureUnit >= 0);
        }
    }
}
