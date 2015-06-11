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
    /// Represents a three-dimensional texture.
    /// </summary>
    public class Texture3D : Texture3DBase
    {
        /// <summary>
        /// Initializes a new <see cref="Texture3D"/> from a <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The <see cref="TextureDescription"/> describing the texture layout.</param>
        public Texture3D(TextureDescription description)
            : base(description)
        {
            Contract.Requires<ArgumentNullException>(description != null);
            Contract.Requires<ArgumentException>(description.Target.IsTexture3DTarget());

            this.VerifyAccess();
        }

        /// <summary>
        /// Sets the texture data.
        /// </summary>
        /// <param name="data">The texture data.</param>
        /// <param name="pixelType">The type of a single color component inside of the <paramref name="data"/>.</param>
        /// <param name="pixelFormat">The format of a single pixel inside of the <paramref name="data"/>.</param>
        /// <param name="width">The width of the block of texture data to be set.</param>
        /// <param name="height">The height of the block of texture data to be set.</param>
        /// <param name="depth">The depth of the block of texture data to be set.</param>
        /// <param name="xOffset">The offset in X direction of the data to be set.</param>
        /// <param name="yOffset">The offset in Y direction of the data to be set.</param>
        /// <param name="zOffset">The offset in Z direction of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
        {
            this.VerifyAccess();

            using (Binding texture2dBinding = this.Bind(0))
            {
                GL.TexSubImage3D(this.Target, level, xOffset, yOffset, zOffset, width, height, depth, pixelFormat, pixelType, data);
            }
        }

        /// <summary>
        /// Creates a new <see cref="Texture3D"/> from the specified <see cref="TextureDescription"/> and sets the texture data.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the data to set as texture data.</typeparam>
        /// <param name="desc">The <see cref="TextureDescription"/> describing the texture layout.</param>
        /// <param name="data">The texture data.</param>
        /// <param name="pixelType">The type of a single color component inside of the <paramref name="data"/>.</param>
        /// <param name="pixelFormat">The format of a single pixel inside of the <paramref name="data"/>.</param>
        /// <param name="width">The width of the block of texture data to be set.</param>
        /// <param name="height">The height of the block of texture data to be set.</param>
        /// <param name="depth">The depth of the block of texture data to be set.</param>
        /// <param name="xOffset">The offset in X direction of the data to be set.</param>
        /// <param name="yOffset">The offset in Y direction of the data to be set.</param>
        /// <param name="zOffset">The offset in Z direction of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        /// <returns>The newly created texture.</returns>
        public static Texture3D Create<T>(TextureDescription desc, T[] data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
        {
            Contract.Requires<ArgumentNullException>(desc != null);
            Contract.Requires<ArgumentException>(desc.Target.IsTexture3DTarget());
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(yOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(zOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            GCHandle arrayHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                return Create(
                    desc,
                    arrayHandle.AddrOfPinnedObject(),
                    pixelFormat, pixelType,
                    width, height, depth,
                    xOffset, yOffset, zOffset,
                    level
                );
            }
            finally
            {
                arrayHandle.Free();
            }
        }

        /// <summary>
        /// Creates a new <see cref="Texture3D"/> from the specified <see cref="TextureDescription"/> and sets the texture data.
        /// </summary>
        /// <param name="desc">The <see cref="TextureDescription"/> describing the texture layout.</param>
        /// <param name="data">The texture data.</param>
        /// <param name="pixelType">The type of a single color component inside of the <paramref name="data"/>.</param>
        /// <param name="pixelFormat">The format of a single pixel inside of the <paramref name="data"/>.</param>
        /// <param name="width">The width of the block of texture data to be set.</param>
        /// <param name="height">The height of the block of texture data to be set.</param>
        /// <param name="depth">The depth of the block of texture data to be set.</param>
        /// <param name="xOffset">The offset in X direction of the data to be set.</param>
        /// <param name="yOffset">The offset in Y direction of the data to be set.</param>
        /// <param name="zOffset">The offset in Z direction of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        /// <returns>The newly created texture.</returns>
        public static Texture3D Create(TextureDescription desc, IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
        {
            Contract.Requires<ArgumentNullException>(desc != null);
            Contract.Requires<ArgumentException>(desc.Target.IsTexture3DTarget());
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(yOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            Texture3D tex3d = new Texture3D(desc);
            tex3d.Set(data, pixelFormat, pixelType, width, height, depth, xOffset, yOffset, zOffset, level);
            return tex3d;
        }
    }
}
