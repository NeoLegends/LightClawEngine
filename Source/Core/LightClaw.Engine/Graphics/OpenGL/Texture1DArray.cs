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
    public class Texture1DArray : Texture2DBase
    {
        public Texture1DArray(TextureDescription description)
            : base(description) 
        {
            Contract.Requires<ArgumentException>(IsTexture1DArrayTarget(description.Target));
        }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
        {
            this.Initialize();
            using (GLBinding texture2dBinding = new GLBinding(this))
            {
                GL.TexSubImage2D(this.Target, level, 0, 0, width, height, pixelFormat, pixelType, data);
            }
        }

        [Pure]
        public static bool IsTexture1DArrayTarget(TextureTarget target)
        {
            Contract.Ensures(!Contract.Result<bool>() || Enum.IsDefined(typeof(TextureTarget2d), target));

            return (target == TextureTarget.Texture1DArray) || (target == TextureTarget.Texture1DArray);
        }
    }
}
