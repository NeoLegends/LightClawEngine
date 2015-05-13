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
    /// A base class of all two-dimensional textures.
    /// </summary>
    [ContractClass(typeof(Texture2DBaseContracts))]
    public abstract class Texture2DBase : Texture
    {
        /// <summary>
        /// Initializes a new <see cref="Texture2DBase"/> from a <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The <see cref="TextureDescription"/> describing the texture layout.</param>
        protected Texture2DBase(TextureDescription description)
            : base(description)
        {
            this.VerifyAccess();

            using (Binding textureBinding = this.Bind())
            {
                switch (this.Target)
                {
                    case TextureTarget.Texture2DMultisample:
                    case TextureTarget.ProxyTexture2DMultisample:
                        GL.TexStorage2DMultisample(
                            (TextureTargetMultisample2d)this.Target,
                            this.MultisamplingLevels,
                            (SizedInternalFormat)this.PixelInternalFormat,
                            this.Width,
                            this.Height,
                            false
                        );
                        break;
                    default:
                        GL.TexStorage2D(
                            (TextureTarget2d)this.Target,
                            this.MipmapLevels,
                            (SizedInternalFormat)this.PixelInternalFormat,
                            this.Width,
                            this.Height
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
        /// <param name="xOffset">The offset in X direction of the data to be set.</param>
        /// <param name="yOffset">The offset in Y direction of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        public void Set<T>(T[] data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(yOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                this.Set(
                    dataHandle.AddrOfPinnedObject(),
                    pixelFormat, pixelType,
                    width, height,
                    xOffset, yOffset,
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
        /// <param name="width">The width of the block of texture data to be set.</param>
        /// <param name="height">The height of the block of texture data to be set.</param>
        /// <param name="xOffset">The offset in X direction of the data to be set.</param>
        /// <param name="yOffset">The offset in Y direction of the data to be set.</param>
        /// <param name="level">The mipmap level of the data to be set.</param>
        public abstract void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level);
    }

    [ContractClassFor(typeof(Texture2DBase))]
    internal abstract class Texture2DBaseContracts : Texture2DBase
    {
        public Texture2DBaseContracts() : base(new TextureDescription()) { }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
        {
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(yOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);
        }
    }
}
