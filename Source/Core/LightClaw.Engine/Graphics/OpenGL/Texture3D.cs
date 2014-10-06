using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class Texture3D : Texture3DBase
    {
        public Texture3D(TextureDescription description)
            : base(description)
        {
            Contract.Requires<ArgumentException>(IsTexture3DTarget(description.Target));
        }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
        {
            this.Initialize();
            using (Binding texture2dBinding = new Binding(this))
            {
                GL.TexSubImage3D(this.Target, level, xOffset, yOffset, zOffset, width, height, depth, pixelFormat, pixelType, data);
            }
        }

        [Pure]
        public static bool IsTexture3DTarget(TextureTarget target)
        {
            Contract.Ensures(!Contract.Result<bool>() || Enum.IsDefined(typeof(TextureTarget3d), target));

            return (target == TextureTarget.Texture3D) || (target == TextureTarget.ProxyTexture3D) ||
                   (target == TextureTarget.TextureCubeMap) || (target == TextureTarget.ProxyTextureCubeMap);
        }
    }
}
