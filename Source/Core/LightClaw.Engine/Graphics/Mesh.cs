using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public class Mesh : Component
    {
        private static ILog logger = LogManager.GetLogger(typeof(Mesh));

        private MeshData meshData;

        private string _ResourceString;

        [DataMember]
        public string ResourceString
        {
            get
            {
                return _ResourceString;
            }
            private set
            {
                this.SetProperty(ref _ResourceString, value);
            }
        }

        private Shader _Shader;

        public Shader Shader
        {
            get
            {
                return _Shader;
            }
            private set
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
            private set
            {
                this.SetProperty(ref _Textures, value);
            }
        }

        private VertexArrayObject _Vao;

        public VertexArrayObject Vao
        {
            get
            {
                return _Vao;
            }
            private set
            {
                this.SetProperty(ref _Vao, value);
            }
        }

        private Mesh() { }

        public Mesh(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            logger.Info("Initializing a new mesh from resource string '{0}'.".FormatWith(resourceString));

            this.Name = resourceString;
            this.ResourceString = resourceString;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnDraw()
        {
            Shader shader = this.Shader;
            ImmutableList<Texture> textures = this.Textures;
            VertexArrayObject vao = this.Vao;
            if (shader != null && textures != null && vao != null)
            {
                using (GLBinding shaderBinding = new GLBinding(shader))
                using (TextureGLBinding textureBinding = new TextureGLBinding(textures))
                using (GLBinding vaoBinding = new GLBinding(vao))
                {
                    GL.DrawElements(BeginMode.Triangles, vao.IndexBuffer.Count, DrawElementsType.UnsignedShort, 0);
                }
            }

            base.OnDraw();
        }

        protected override void OnLoad()
        {
            logger.Debug("Loading mesh '{0}'.".FormatWith(this.Name ?? this.ResourceString));
            Task<MeshData> meshDataTask = (this.meshData != null) ? Task.FromResult(this.meshData) : this.IocC.Resolve<IContentManager>()
                                                                                                              .LoadAsync<MeshData>(this.ResourceString);

            meshDataTask.ContinueWith(t =>
            {
                logger.Debug("Mesh '{0}' loaded successfully.".FormatWith(this.Name ?? this.ResourceString));
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
            meshDataTask.ContinueWith(t =>
            {
                logger.Warn("Mesh '{0}' could not be loaded, it will not be rendered.".FormatWith(this.Name ?? this.ResourceString), t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);

            base.OnLoad();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            Shader s = this.Shader;
            if (s != null)
            {
                s.Update(gameTime);
            }
            base.OnUpdate(gameTime);
        }

        protected override void OnLateUpdate()
        {
            Shader s = this.Shader;
            if (s != null)
            {
                s.LateUpdate();
            }
            base.OnLateUpdate();
        }

        protected struct TextureGLBinding : IDisposable, IBindable
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
}
