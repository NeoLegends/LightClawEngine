using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public abstract class Texture : GLObject, IBindable
    {
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

        private SizedInternalFormat _SizedInternalFormat = SizedInternalFormat.Rgba8i;

        public SizedInternalFormat SizedInternalFormat
        {
            get
            {
                return _SizedInternalFormat;
            }
            protected set
            {
                this.SetProperty(ref _SizedInternalFormat, value);
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

        protected Texture() : base(GL.GenTexture()) { }

        protected Texture(TextureTarget target, SizedInternalFormat sizedInternalFormat, int width, int levels)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);

            this.Levels = levels;
            this.Target = target;
            this.SizedInternalFormat = sizedInternalFormat;
            this.Width = width;
        }

        public void Bind()
        {
            GL.BindTexture(this.Target, this);
        }

        public void Unbind()
        {
            GL.BindTexture(this.Target, 0);
        }

        protected override void Dispose(bool disposing)
        {
            this.Unbind();
            GL.DeleteTexture(this);

            base.Dispose(disposing);
        }
    }
}
