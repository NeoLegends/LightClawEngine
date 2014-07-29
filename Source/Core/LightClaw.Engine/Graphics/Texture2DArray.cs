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
    public class Texture2DArray : Texture3DBase
    {
        public Texture2DArray(SizedInternalFormat sizedInternalFormat, int width, int height, int depth)
            : base(TextureTarget3d.Texture2DArray, sizedInternalFormat, width, height, depth)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)depth));
        }

        public Texture2DArray(SizedInternalFormat sizedInternalFormat, int width, int height, int depth, int levels)
            : base(TextureTarget3d.Texture2DArray, sizedInternalFormat, width, height, depth, levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)depth));
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);
        }

        public void Update<T>(T[] data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(yOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(zOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            using (GLBinding texture2dBinding = new GLBinding(this))
            {
                GL.TexSubImage3D(this.Target, level, xOffset, yOffset, zOffset, width, height, depth, pixelFormat, pixelType, data);
            }
        }
    }
}
