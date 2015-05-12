using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Threading;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a one-dimensional <see cref="Texture"/>.
    /// </summary>
    public class Texture1D : Texture
    {
        /// <summary>
        /// Initializes a new <see cref="Texture1D"/> from a <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The <see cref="TextureDescription"/> describing the texture layout.</param>
        public Texture1D(TextureDescription description)
            : base(description)
        {
            Contract.Requires<ArgumentException>(description.Target.IsTexture1DTarget());

            this.VerifyAccess();
            using (Binding textureBinding = new Binding(this))
            {
                GL.TexStorage1D(TextureTarget1d.Texture1D, this.MipmapLevels, (SizedInternalFormat)this.PixelInternalFormat, this.Width);
            }
        }

        /// <summary>
        /// Sets the texture data.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the data to set as texture data.</typeparam>
        /// <param name="data">The texture data.</param>
        /// <param name="pixelType">The type of a single color component inside of the <paramref name="data"/>.</param>
        /// <param name="pixelFormat">The format of a single pixel inside of the <paramref name="data"/>.</param>
        /// <param name="width">The width of the block to be set.</param>
        /// <param name="xOffset">The offset in X direction of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        public void Set<T>(T[] data, PixelType pixelType, PixelFormat pixelFormat, int width, int xOffset, int level)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                this.Set(
                    dataHandle.AddrOfPinnedObject(),
                    pixelType, pixelFormat,
                    width,
                    xOffset,
                    level
                );
            }
            finally
            {
                dataHandle.Free();
            }
        }

        /// <summary>
        /// Sets the texture data.
        /// </summary>
        /// <param name="data">The texture data.</param>
        /// <param name="pixelType">The type of a single color component inside of the <paramref name="data"/>.</param>
        /// <param name="pixelFormat">The format of a single pixel inside of the <paramref name="data"/>.</param>
        /// <param name="width">The width of the block to be set.</param>
        /// <param name="xOffset">The horizontal offset of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        public void Set(IntPtr data, PixelType pixelType, PixelFormat pixelFormat, int width, int xOffset, int level)
        {
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            this.VerifyAccess();
            using (Binding textureBinding = new Binding(this))
            {
                GL.TexSubImage1D(this.Target, level, xOffset, width, pixelFormat, pixelType, data);
            }
        }
    }
}
