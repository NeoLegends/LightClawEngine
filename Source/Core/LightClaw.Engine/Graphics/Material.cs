using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class Material : Entity, IBindable
    {
        public ShaderProgram Shader { get; private set; }

        public int TextureCount
        {
            get
            {
                return this.Textures.Count;
            }
        }

        public ImmutableList<Texture2D> Textures { get; private set; }

        public Material(ShaderProgram program, IEnumerable<Texture2D> textures)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(textures != null);
            Contract.Requires<ArgumentException>(textures.Count() < Constants.MaxCombinedTextureImageUnits);

            this.Shader = program;
            this.Textures = textures.ToImmutableList();
        }

        public void Bind()
        {
            for (TextureUnit texUnit = TextureUnit.Texture0; (int)texUnit < this.TextureCount; texUnit++)
            {
                Texture2D currentTexture = this.Textures[texUnit - TextureUnit.Texture0];
                GL.ActiveTexture(texUnit);
                GL.BindTexture(currentTexture.Target, currentTexture);
            }
        }

        public void Unbind()
        {
            throw new NotImplementedException();
        }
    }
}
