using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public struct TextureGLBinding : IDisposable, IBindable
    {
        private readonly IList<Texture> textures;

        public TextureGLBinding(IList<Texture> texturesToBind, bool bindImmediately = true)
        {
            Contract.Requires<ArgumentNullException>(texturesToBind != null);

            this.textures = texturesToBind;
            if (bindImmediately)
            {
                this.Bind();
            }
        }

        public void Bind()
        {
            if (this.textures != null)
            {
                for (TextureUnit texUnit = TextureUnit.Texture0; (int)texUnit < this.textures.Count; texUnit++)
                {
                    GL.ActiveTexture(texUnit);
                    this.textures[texUnit - TextureUnit.Texture0].Bind();
                }
            }
        }

        public void Unbind()
        {
            if (this.textures != null)
            {
                for (TextureUnit texUnit = TextureUnit.Texture0; (int)texUnit < this.textures.Count; texUnit++)
                {
                    GL.ActiveTexture(texUnit);
                    this.textures[texUnit - TextureUnit.Texture0].Unbind();
                }
            }
        }

        void IDisposable.Dispose()
        {
            this.Unbind();
        }
    }
}
