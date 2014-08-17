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
    public class Texture1DArray : Texture2DBase
    {
        public Texture1DArray() { }

        public Texture1DArray(PixelInternalFormat pixelInternalFormat, int width, int height)
            : base(TextureTarget2d.Texture1DArray, pixelInternalFormat, width, height)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
        }

        public Texture1DArray(PixelInternalFormat pixelInternalFormat, int width, int height, int levels)
            : base(TextureTarget2d.Texture1DArray, pixelInternalFormat, width, height, levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);
        }

        public void Set<T>(T[] data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(yOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        throw new InvalidOperationException("{0} has to be initialized before setting the data.".FormatWith(typeof(Texture1DArray).Name));
                    }
                }
            }

            using (GLBinding texture2dBinding = new GLBinding(this))
            {
                GL.TexSubImage2D(this.Target, level, 0, 0, width, height, pixelFormat, pixelType, data);
            }
        }
    }
}
