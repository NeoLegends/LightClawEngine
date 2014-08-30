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

        private TextureUnit _TextureUnit = 0;

        public TextureUnit TextureUnit
        {
            get
            {
                Contract.Ensures(Contract.Result<TextureUnit>() >= 0);

                return _TextureUnit;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _TextureUnit, value);
            }
        }

        public SamplerEffectUniform(EffectStage stage, string name, TextureUnit textureUnit)
            : base(stage, name)
        {
            Contract.Requires<ArgumentNullException>(stage != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            this.TextureUnit = textureUnit;
        }

        public void Bind()
        {
            Texture texture = this.Texture;
            if (texture != null)
            {
                GL.ProgramUniform1(this.Stage.ShaderProgram, this.Location, this.TextureUnit);
                texture.Bind();

                Sampler sampler = this.Sampler;
                if (sampler != null)
                {
                    sampler.Bind();
                }
            }
            else
            {
                Logger.Warn(() => "Texture to bind to the sampler in the shader was null and thus will not be bound. This is presumably unwanted behaviour!");
            }
        }

        public void Unbind()
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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._TextureUnit >= 0);
        }
    }
}
