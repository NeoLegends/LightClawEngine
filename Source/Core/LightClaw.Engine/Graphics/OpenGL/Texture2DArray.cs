using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class Texture2DArray : Texture3DBase
    {
        public Texture2DArray(TextureDescription description) 
            : base(description) 
        {
            Contract.Requires<ArgumentException>(IsTexture2DArrayTarget(description.Target));
        }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
        {
            this.Initialize();
            using (GLBinding texture2dBinding = new GLBinding(this))
            {
                GL.TexSubImage3D(this.Target, level, xOffset, yOffset, zOffset, width, height, depth, pixelFormat, pixelType, data);
            }
        }

        [Pure]
        public static bool IsTexture2DArrayTarget(TextureTarget target)
        {
            Contract.Ensures(!Contract.Result<bool>() || Enum.IsDefined(typeof(TextureTarget3d), target));

            return (target == TextureTarget.Texture2DArray) || (target == TextureTarget.Texture2DMultisampleArray) ||
                   (target == TextureTarget.ProxyTexture2DArray) || (target == TextureTarget.ProxyTexture2DMultisampleArray);
        }
    }
}
