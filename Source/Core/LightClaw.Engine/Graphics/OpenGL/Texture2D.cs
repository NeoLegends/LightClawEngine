using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class Texture2D : Texture2DBase
    {
        public Texture2D(TextureDescription description)
            : base(description)
        {
            Contract.Requires<ArgumentException>(description.Target.IsTexture2DTarget());
        }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
        {
            this.Initialize();
            using (Binding texture2dBinding = new Binding(this))
            {
                GL.TexSubImage2D(this.Target, level, xOffset, yOffset, width, height, pixelFormat, pixelType, data);
            }
        }
    }
}
