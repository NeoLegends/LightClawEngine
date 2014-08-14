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
        public Texture1D() { }

        public Texture1D(SizedInternalFormat sizedInternalFormat, int width)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));

            this.Initialize(sizedInternalFormat, width);
        }

        public Texture1D(SizedInternalFormat sizedInternalFormat, int width, int levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));

            this.Initialize(sizedInternalFormat, width, levels);
        }

        public void Initialize(SizedInternalFormat sizedInternalFormat, int width)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));

            this.Initialize(sizedInternalFormat, width, Math.Max((int)Math.Log(width, 2), 1));
        }

        public void Initialize(SizedInternalFormat sizedInternalFormat, int width, int levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));

            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Levels = levels;
                        this.SizedInternalFormat = sizedInternalFormat;
                        this.Target = (TextureTarget)TextureTarget1d.Texture1D;
                        this.Width = width;

                        using (GLBinding textureBinding = new GLBinding(this))
                        {
                            GL.TexStorage1D(TextureTarget1d.Texture1D, levels, sizedInternalFormat, width);
                        }

                        this.IsInitialized = true;
                        return;
                    }
                }
            }

            throw new NotSupportedException("Initializing a {0} twice is not supported. Parameters might be different.".FormatWith(typeof(Texture1D).Name));
        }

        public void Set<T>(T[] data, PixelType pixelType, PixelFormat pixelFormat, int width, int xOffset, int level)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        throw new InvalidOperationException("{0} has to be initialized before setting the data.".FormatWith(typeof(Texture1D).Name));
                    }
                }
            }

            GL.TexSubImage1D(TextureTarget.Texture1D, level, xOffset, width, pixelFormat, pixelType, data);
        }
    }
}
