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

        protected Texture2DBase(TextureTarget2d target, SizedInternalFormat sizedInternalFormat, int width, int height)
            : this(target, sizedInternalFormat, width, height, Math.Max((int)Math.Min(Math.Log(width, 2), Math.Log(height, 2)) + 1, 1)) // Generate as many levels as needed, but at least one
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
        }

        protected Texture2DBase(TextureTarget2d target, SizedInternalFormat sizedInternalFormat, int width, int height, int levels)
            : base((TextureTarget)target, sizedInternalFormat, width, levels)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentOutOfRangeException>(levels > 0);

            this.Height = height;

            using (GLBinding textureBinding = new GLBinding(this))
            {
                GL.TexStorage2D(target, levels, sizedInternalFormat, width, height);
            }
        }
    }
}
