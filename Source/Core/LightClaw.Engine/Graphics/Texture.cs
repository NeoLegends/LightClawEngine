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
        public int MipLevels { get; protected set; }

        public PixelFormat PixelFormat { get; protected set; }

        public PixelInternalFormat PixelInternalFormat { get; protected set; }

        public PixelType PixelType { get; protected set; }

        public TextureTarget Target { get; protected set; }

        public int Width { get; protected set; }

        protected Texture() : base(GL.GenTexture()) { }

        protected Texture(TextureTarget target)
            : this()
        {
            this.Target = target;
        }

        protected Texture(
                    TextureTarget target, 
                    int mipLevels, 
                    PixelFormat pixelFormat, 
                    PixelInternalFormat pixelInternalFormat, 
                    PixelType pixelType, 
                    int width
                )
            : this(target)
        {
            Contract.Requires<ArgumentOutOfRangeException>(mipLevels >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);

            this.MipLevels = mipLevels;
            this.PixelFormat = pixelFormat;
            this.PixelInternalFormat = pixelInternalFormat;
            this.PixelType = pixelType;
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
    }
}
