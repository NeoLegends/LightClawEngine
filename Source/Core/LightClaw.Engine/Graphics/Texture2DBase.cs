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
    public abstract class Texture2DBase : Texture
    {
        private int _Height;

        public int Height
        {
            get
            {
                return _Height;
            }
            protected set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value > 0);

                this.SetProperty(ref _Height, value);
            }
        }

        protected Texture2DBase() { }

        protected Texture2DBase(TextureTarget2d target, SizedInternalFormat sizedInternalFormat, int width, int height)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));

            this.Initialize(target, sizedInternalFormat, width, height);
        }

        protected Texture2DBase(TextureTarget2d target, SizedInternalFormat sizedInternalFormat, int width, int height, int levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);

            this.Initialize(target, sizedInternalFormat, width, height, levels);
        }

        public void Initialize(TextureTarget2d target, SizedInternalFormat sizedInternalFormat, int width, int height)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));

            this.Initialize(target, sizedInternalFormat, width, height, Math.Max((int)Math.Min(Math.Log(width, 2), Math.Log(height, 2)) + 1, 1));
        }

        public void Initialize(TextureTarget2d target, SizedInternalFormat sizedInternalFormat, int width, int height, int levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);

            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Height = height;
                        this.Levels = levels;
                        this.SizedInternalFormat = sizedInternalFormat;
                        this.Target = (TextureTarget)target;
                        this.Width = width;

                        using (GLBinding textureBinding = new GLBinding(this))
                        {
                            GL.TexStorage2D(target, levels, sizedInternalFormat, width, height);
                        }

                        this.IsInitialized = true;
                        return;
                    }
                }
            }

            throw new InvalidOperationException("{0} cannot be initialized twice.".FormatWith(this.GetType().Name));
        }
    }
}
