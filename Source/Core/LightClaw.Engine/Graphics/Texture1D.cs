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
    public class Texture1D : Texture
    {
        public Texture1D(SizedInternalFormat sizedInternalFormat, int width)
            : this(sizedInternalFormat, width, Math.Max((int)Math.Log(width, 2), 1)) // Generate as many levels as needed, but at least one
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
        }

        public Texture1D(SizedInternalFormat sizedInternalFormat, int width, int levels)
            : base((TextureTarget)TextureTarget1d.Texture1D, sizedInternalFormat, width, levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));

            GL.TexStorage1D(TextureTarget1d.Texture1D, levels, sizedInternalFormat, width);
        }

        public void Update<T>(T[] data, PixelType pixelType, PixelFormat pixelFormat, int width, int xOffset, int level)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            GL.TexSubImage1D(TextureTarget.Texture1D, level, xOffset, width, pixelFormat, pixelType, data);
        }
    }
}
