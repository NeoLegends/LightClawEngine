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
    /// The base class of all three-dimensional textures.
    /// </summary>
    [ContractClass(typeof(Texture3DBaseContracts))]
    public abstract class Texture3DBase : Texture
    {
        /// <summary>
        /// Initializes a new <see cref="Texture3DBase"/> from a <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The <see cref="TextureDescription"/> describing the texture layout.</param>
        protected Texture3DBase(TextureDescription description)
            : base(description)
        {
            this.VerifyAccess();

            using (Binding textureBinding = this.Bind())
            {
                switch (this.Target)
                {
                    case TextureTarget.Texture2DMultisampleArray:
                    case TextureTarget.ProxyTexture2DMultisampleArray:
                        GL.TexStorage3DMultisample(
                            (TextureTargetMultisample3d)this.Target,
                            this.MultisamplingLevels,
                            (SizedInternalFormat)this.PixelInternalFormat,
                            this.Width,
                            this.Height,
                            this.Depth,
                            false
                        );
                        break;
                    default:
                        GL.TexStorage3D(
                            (TextureTarget3d)this.Target,
                            this.MipmapLevels,
                            (SizedInternalFormat)this.PixelInternalFormat,
                            this.Width,
                            this.Height,
                            this.Depth
                        );
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the texture data.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the data to set as texture data.</typeparam>
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
        public void Set<T>(T[] data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
            where T : struct
        {
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
                this.Set(
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
        public abstract void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level);
    }

    [ContractClassFor(typeof(Texture3DBase))]
    internal abstract class Texture3DBaseContracts : Texture3DBase
    {
        public Texture3DBaseContracts() : base(new TextureDescription()) { }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
        {
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(yOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(zOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);
        }
    }
}
