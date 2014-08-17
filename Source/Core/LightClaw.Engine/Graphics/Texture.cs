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
    public abstract class Texture : GLObject, IBindable
    {
        protected readonly object initializationLock = new object();

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

        private int _Levels = 1;

        public int Levels
        {
            get
            {
                return _Levels;
            }
            protected set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value > 0);

                this.SetProperty(ref _Levels, value);
            }
        }

        private PixelInternalFormat _InternalFormat = PixelInternalFormat.Rgba8i;

        public PixelInternalFormat PixelInternalFormat
        {
            get
            {
                return _InternalFormat;
            }
            protected set
            {
                this.SetProperty(ref _InternalFormat, value);
            }
        }

        private int _TextureUnit;

        public int TextureUnit
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return Math.Max(_TextureUnit, 0);
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _TextureUnit, value);
            }
        }

        private TextureTarget _Target = TextureTarget.Texture2D;

        public TextureTarget Target
        {
            get
            {
                return _Target;
            }
            protected set
            {
                this.SetProperty(ref _Target, value);
            }
        }

        private int _Width;

        public int Width
        {
            get
            {
                return _Width;
            }
            protected set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value > 0);

                this.SetProperty(ref _Width, value);
            }
        }

        static Texture()
        {
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

        protected Texture() { }

        protected Texture(TextureTarget target, PixelInternalFormat pixelInternalFormat, int width, int levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);

            this.Levels = levels;
            this.Target = target;
            this.PixelInternalFormat = pixelInternalFormat;
            this.Width = width;
        }

        public void Bind()
        {
            this.Bind(this.TextureUnit);
        }

        public void Bind(int textureUnit)
        {
            GL.ActiveTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(this.Target, this);
        }

        public void Unbind()
        {
            this.Unbind(this.TextureUnit);
        }

        public void Unbind(int textureUnit)
        {
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
                logger.Warn(() => "An exception of type '{0}' was thrown while disposing the {0}'s underlying OpenGL Texture.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(Texture).Name));
            }

            base.Dispose(disposing);
        }
    }
}
