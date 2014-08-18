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
    public class Texture1D : Texture
    {
        public Texture1D(TextureDescription description) : base(description) { }

        public void Set<T>(T[] data, PixelType pixelType, PixelFormat pixelFormat, int width, int xOffset, int level)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            this.Initialize();
            GL.TexSubImage1D(TextureTarget.Texture1D, level, xOffset, width, pixelFormat, pixelType, data);
        }

        protected override void OnInitialize()
        {
            using (GLBinding textureBinding = new GLBinding(this))
            {
                GL.TexStorage1D(TextureTarget1d.Texture1D, this.Levels, (SizedInternalFormat)this.PixelInternalFormat, this.Width);
            }
        }
    }
}
