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
    public abstract class Texture3DBase : Texture
    {
        private int _Depth;

        public int Depth
        {
            get
            {
                return _Depth;
            }
            protected set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value > 0);

                this.SetProperty(ref _Depth, value);
            }
        }

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

        protected Texture3DBase(TextureTarget3d target, SizedInternalFormat sizedInternalFormat, int width, int height, int depth)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)depth));

            this.Initialize(target, sizedInternalFormat, width, height, depth);
        }

        protected Texture3DBase(TextureTarget3d target, SizedInternalFormat sizedInternalFormat, int width, int height, int depth, int levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)depth));
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);

            this.Initialize(target, sizedInternalFormat, width, height, depth, levels);
        }

        public void Initialize(TextureTarget3d target, SizedInternalFormat sizedInternalFormat, int width, int height, int depth)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)depth));

            this.Initialize(
                target,
                sizedInternalFormat,
                width,
                height,
                depth,
                Math.Max((int)Math.Min(Math.Min(Math.Log(width, 2), Math.Log(height, 2)), Math.Log(depth, 2)) + 1, 1)
            );
        }

        public void Initialize(TextureTarget3d target, SizedInternalFormat sizedInternalFormat, int width, int height, int depth, int levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)depth));
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);

            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Depth = depth;
                        this.Height = height;
                        this.Levels = levels;
                        this.SizedInternalFormat = sizedInternalFormat;
                        this.Target = (TextureTarget)target;
                        this.Width = width;

                        using (GLBinding textureBinding = new GLBinding(this))
                        {
                            GL.TexStorage3D(target, levels, sizedInternalFormat, width, height, depth);
                        }

                        this.IsInitialized = true;
                        return;
                    }
                }
            }

            throw new NotSupportedException("{0} cannot be initialized twice.".FormatWith(this.GetType().Name));
        }
    }
}
