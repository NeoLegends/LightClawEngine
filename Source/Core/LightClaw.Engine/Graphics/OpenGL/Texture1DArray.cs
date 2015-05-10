using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Threading;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class Texture1DArray : Texture2DBase
    {
        public Texture1DArray(TextureDescription description)
            : base(description)
        {
            Contract.Requires<ArgumentException>(description.Target.IsTexture1DArrayTarget());
        }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
        {
            this.VerifyAccess();
            using (Binding texture2dBinding = new Binding(this))
            {
                GL.TexSubImage2D(this.Target, level, 0, 0, width, height, pixelFormat, pixelType, data);
            }
        }
    }
}
