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

namespace LightClaw.Engine.Graphics
{
    public class Texture1D : Texture
    {
        public Texture1D(TextureDescription description) 
            : base(description) 
        {
            Contract.Requires<ArgumentException>(IsTexture1DTarget(description.Target));
        }

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

        public void Set(IntPtr data, PixelType pixelType, PixelFormat pixelFormat, int width, int xOffset, int level)
        {
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(xOffset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(level >= 0);

            this.Initialize();
            using (GLBinding textureBinding = new GLBinding(this))
            {
                GL.TexSubImage1D(this.Target, level, xOffset, width, pixelFormat, pixelType, data);
            }
        }

        protected override void OnInitialize()
        {
            using (GLBinding textureBinding = new GLBinding(this))
            {
                GL.TexStorage1D(TextureTarget1d.Texture1D, this.Levels, (SizedInternalFormat)this.PixelInternalFormat, this.Width);
            }
        }

        [Pure]
        public static bool IsTexture1DTarget(TextureTarget target)
        {
            Contract.Ensures(!Contract.Result<bool>() || Enum.IsDefined(typeof(TextureTarget1d), target));

            return (target == TextureTarget.Texture1D) || (target == TextureTarget.ProxyTexture1D);
        }
    }
}
