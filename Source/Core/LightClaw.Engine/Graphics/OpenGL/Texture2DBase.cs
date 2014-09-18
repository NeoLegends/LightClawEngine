using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    [ContractClass(typeof(Texture2DBaseContracts))]
    public abstract class Texture2DBase : Texture
    {
        protected Texture2DBase(TextureDescription description) 
            : base(description) 
        {
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(TextureTarget2d), description.Target));
        }

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

        public abstract void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level);

        protected override void OnInitialize()
        {
            using (Binding textureBinding = new Binding(this))
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
                            this.Levels, 
                            (SizedInternalFormat)this.PixelInternalFormat, 
                            this.Width, 
                            this.Height
                        );
                        break;
                }
            }
        }
    }

    [ContractClassFor(typeof(Texture2DBase))]
    abstract class Texture2DBaseContracts : Texture2DBase
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
