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
    public class Texture2D : GLObject, IBindable
    {
        public PixelFormat PixelFormat { get; private set; }

        public PixelInternalFormat PixelInternalFormat { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public PixelType PixelType { get; private set; }

        public Texture2D() : base(GL.GenTexture()) { }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, this);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Update<T>(T[] data)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Length > 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)data.Length));

            int sideLength = (int)Math.Sqrt(data.Length);
            this.Update(data, sideLength, sideLength, PixelFormat.Rgb, PixelInternalFormat.Rgb, PixelType.Byte);
        }

        public void Update<T>(T[] data, int width, int height, PixelFormat format, PixelInternalFormat internalFormat, PixelType type)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));

            using (GLBinding textureBinding = new GLBinding(this))
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, type, data);
            }
        }
    }
}
