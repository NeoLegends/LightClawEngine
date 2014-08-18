using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    [ContractClass(typeof(Texture3DBaseContracts))]
    public abstract class Texture3DBase : Texture
    {
        protected Texture3DBase(TextureDescription description) : base(description) { }

        public abstract void Set<T>(T[] data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level) where T : struct;

        protected override void OnInitialize()
        {
            using (GLBinding textureBinding = new GLBinding(this))
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
                            this.Levels,
                            (SizedInternalFormat)this.PixelInternalFormat,
                            this.Width,
                            this.Height,
                            this.Depth
                        );
                        break;
                }
            }
        }
    }

    [ContractClassFor(typeof(Texture3DBase))]
    abstract class Texture3DBaseContracts : Texture3DBase
    {
        public Texture3DBaseContracts() : base(new TextureDescription()) { }

        public override void Set<T>(T[] data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
        {
            Contract.Requires<ArgumentNullException>(data != null);
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
