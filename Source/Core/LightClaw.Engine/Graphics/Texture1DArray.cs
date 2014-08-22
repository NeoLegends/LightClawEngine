using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class Texture1DArray : Texture2DBase
    {
        public Texture1DArray(TextureDescription description) : base(description) { }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int xOffset, int yOffset, int level)
        {
            this.Initialize();
            using (GLBinding texture2dBinding = new GLBinding(this))
            {
                GL.TexSubImage2D(this.Target, level, 0, 0, width, height, pixelFormat, pixelType, data);
            }
        }
    }
}
