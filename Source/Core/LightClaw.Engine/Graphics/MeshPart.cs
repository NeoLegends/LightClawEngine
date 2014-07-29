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
    public abstract class MeshPart : Entity, IDrawable, ILateUpdateable, IUpdateable
    {
        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        private Mesh _Component;

        public Mesh Component
        {
            get
            {
                return _Component;
            }
            internal set
            {
                this.SetProperty(ref _Component, value);
            }
        }

        private Shader _Shader;

        public Shader Shader
        {
            get
            {
                return _Shader;
            }
            protected set
            {
                this.SetProperty(ref _Shader, value);
            }
        }

        private ImmutableList<Texture> _Textures;

        public ImmutableList<Texture> Textures
        {
            get
            {
                return _Textures;
            }
            protected set
            {
                this.SetProperty(ref _Textures, value);
            }
        }

        private VertexArrayObject  _Vao;

        public VertexArrayObject  Vao
        {
            get
            {
                return _Vao;
            }
            protected set
            {
                this.SetProperty(ref _Vao, value);
            }
        }

        public MeshPart(VertexArrayObject vao)
        {
            Contract.Requires<ArgumentNullException>(vao != null);

            this.Vao = vao;
        }

        public abstract void Draw();

        public abstract void Update(GameTime gameTime);

        public abstract void LateUpdate();

        // Drawing Code
        //this.Raise(this.Drawing);
        //using (TextureGLBinding textureBinding = new TextureGLBinding(this.Textures))
        //using (GLBinding shaderBinding = new GLBinding(this.Shader))
        //using (GLBinding vaoBinding = new GLBinding(this.Vao))
        //{
        //    GL.DrawElements(BeginMode.Triangles, this.Vao.IndexCount, DrawElementsType.UnsignedShort, 0);
        //}
        //this.Raise(this.Drawn);

        protected struct TextureGLBinding : IDisposable, IBindable
        {
            private readonly ImmutableList<Texture> textures;

            public TextureGLBinding(ImmutableList<Texture> texturesToBind, bool bindImmediately = true)
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
}
