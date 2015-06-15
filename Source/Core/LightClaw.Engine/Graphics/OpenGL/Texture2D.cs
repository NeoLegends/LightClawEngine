using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.IO;
using LightClaw.Engine.Threading;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a two-dimensional texture.
    /// </summary>
    [ContentReader(typeof(TextureReader))]
    public class Texture2D : Texture2DBase
    {
        /// <summary>
        /// Initializes a new <see cref="Texture2D"/> from a <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The <see cref="TextureDescription"/> describing the texture layout.</param>
        public Texture2D(TextureDescription description)
            : base(description)
        {
            Contract.Requires<ArgumentNullException>(description != null);
            Contract.Requires<ArgumentException>(description.Target.IsTexture2DTarget());
        }

        /// <summary>
        /// Sets the texture data.
        /// </summary>
        /// <param name="data">The texture data.</param>
        /// <param name="pixelType">The type of a single color component inside of the <paramref name="data"/>.</param>
        /// <param name="pixelFormat">The format of a single pixel inside of the <paramref name="data"/>.</param>
        /// <param name="width">The width of the block of texture data to be set.</param>
        /// <param name="height">The height of the block of texture data to be set.</param>
        /// <param name="xOffset">The offset in X direction of the data to be set.</param>
        /// <param name="yOffset">The offset in Y direction of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
        {
            this.VerifyAccess();
            using (Binding texture2dBinding = new Binding(this))
            {
                GL.TexSubImage2D(this.Target, level, xOffset, yOffset, width, height, pixelFormat, pixelType, data);
            }
        }

        /// <summary>
        /// Creates a new <see cref="Texture2D"/> from the specified <see cref="TextureDescription"/> and sets the texture data.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the data to set as texture data.</typeparam>
        /// <param name="desc">The <see cref="TextureDescription"/> describing the texture layout.</param>
        /// <param name="data">The texture data.</param>
        /// <param name="pixelType">The type of a single color component inside of the <paramref name="data"/>.</param>
        /// <param name="pixelFormat">The format of a single pixel inside of the <paramref name="data"/>.</param>
        /// <param name="width">The width of the block of texture data to be set.</param>
        /// <param name="height">The height of the block of texture data to be set.</param>
        /// <param name="xOffset">The offset in X direction of the data to be set.</param>
        /// <param name="yOffset">The offset in Y direction of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        /// <returns>The newly created texture.</returns>
        public static Texture2D Create<T>(TextureDescription desc, T[] data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
        {
            Contract.Requires<ArgumentNullException>(desc != null);
            Contract.Requires<ArgumentException>(desc.Target.IsTexture2DTarget());
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(yOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                return Create(
                    desc,
                    dataHandle.AddrOfPinnedObject(),
                    pixelFormat, pixelType, width, height, xOffset, yOffset, level
                );
            }
            finally
            {
                dataHandle.Free();
            }
        }

        /// <summary>
        /// Creates a new <see cref="Texture2D"/> from the specified <see cref="TextureDescription"/> and sets the texture data.
        /// </summary>
        /// <param name="desc">The <see cref="TextureDescription"/> describing the texture layout.</param>
        /// <param name="data">The texture data.</param>
        /// <param name="pixelType">The type of a single color component inside of the <paramref name="data"/>.</param>
        /// <param name="pixelFormat">The format of a single pixel inside of the <paramref name="data"/>.</param>
        /// <param name="width">The width of the block of texture data to be set.</param>
        /// <param name="height">The height of the block of texture data to be set.</param>
        /// <param name="xOffset">The offset in X direction of the data to be set.</param>
        /// <param name="yOffset">The offset in Y direction of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        /// <returns>The newly created texture.</returns>
        public static Texture2D Create(TextureDescription desc, IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
        {
            Contract.Requires<ArgumentNullException>(desc != null);
            Contract.Requires<ArgumentException>(desc.Target.IsTexture2DTarget());
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(yOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            Texture2D tex2d = new Texture2D(desc);
            tex2d.Set(data, pixelFormat, pixelType, width, height, xOffset, yOffset, level);
            return tex2d;
        }
    }
}
