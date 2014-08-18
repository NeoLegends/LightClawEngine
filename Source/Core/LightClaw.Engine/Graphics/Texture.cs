using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public abstract class Texture : GLObject, IBindable, IInitializable
    {
        private readonly object initializationLock = new object();

        private bool _IsInitialized;

        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            protected set
            {
                this.SetProperty(ref _IsInitialized, value);
            }
        }

        private TextureDescription _Description;

        public TextureDescription Description
        {
            get
            {
                return _Description;
            }
            protected set
            {
                this.SetProperty(ref _Description, value);
            }
        }

        public int Levels
        {
            get
            {
                return this.Description.TextureLevels;
            }
        }

        public int MultisamplingLevels
        {
            get
            {
                return this.Description.MultisamplingLevels;
            }
        }

        public PixelInternalFormat PixelInternalFormat
        {
            get
            {
                return this.Description.PixelInternalFormat;
            }
        }

        private int _TextureUnit;

        public int TextureUnit
        {
            get
            {
                return _TextureUnit;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _TextureUnit, value);
            }
        }

        public TextureTarget Target
        {
            get
            {
                return this.Description.Target;
            }
        }

        public int Width
        {
            get
            {
                return this.Description.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.Description.Height;
            }
        }

        public int Depth
        {
            get
            {
                return this.Description.Depth;
            }
        }

        static Texture()
        {
            // Assume GLContext is present
            GL.Enable(EnableCap.Texture1D);
            // Not sure whether bug still exists, but on http://www.opengl.org/wiki/Common_Mistakes#Automatic_mipmap_generation it states
            // that ATI cards had problems with automatic mipmap generation if Texture2D wasn't enabled. But since I own an NVIDIA card,
            // I can't test that.
            GL.Enable(EnableCap.Texture2D);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture1D);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture1DArray);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture3D);
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMapArray);
        }

        protected Texture(TextureDescription description)
        {
            this.Description = description;
        }

        public void Bind()
        {
            int unit = this.TextureUnit;
            if (unit < 0)
            {
                throw new InvalidOperationException("The texture unit to bind to ({0}) was smaller than zero.".FormatWith(unit));
            }
            this.Bind(this.TextureUnit);
        }

        public void Bind(int textureUnit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            GL.ActiveTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(this.Target, this);
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.OnInitialize();
                        this.IsInitialized = true;
                    }
                }
            }
        }

        public void Unbind()
        {
            this.Unbind(this.TextureUnit);
        }

        public void Unbind(int textureUnit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            GL.ActiveTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(this.Target, 0);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                this.Unbind();
                GL.DeleteTexture(this);
            }
            catch (Exception ex)
            {
                Logger.Warn(() => "An exception of type '{0}' was thrown while disposing the {0}'s underlying OpenGL Texture.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(Texture).Name));
            }

            base.Dispose(disposing);
        }

        protected abstract void OnInitialize();
    }
}
