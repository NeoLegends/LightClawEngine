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
            : this(target, sizedInternalFormat, width, height, depth, (int)Math.Min(Math.Min(Math.Log(width, 2), Math.Log(height, 2)), Math.Log(depth, 2)) + 1) // Generate as many mipmaps as needed to shrink width or height to 1px
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)depth));
        }

        protected Texture3DBase(TextureTarget3d target, SizedInternalFormat sizedInternalFormat, int width, int height, int depth, int levels)
            : base((TextureTarget)target, sizedInternalFormat, width, levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)depth));
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);

            this.Depth = depth;
            this.Height = height;

            using (GLBinding textureBinding = new GLBinding(this))
            {
                GL.TexStorage3D(target, levels, sizedInternalFormat, width, height, depth);
            }
        }
    }
}
