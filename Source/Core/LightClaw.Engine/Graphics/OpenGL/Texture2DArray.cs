using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class Texture2DArray : Texture3DBase
    {
        public Texture2DArray(TextureDescription description)
            : base(description)
        {
            Contract.Requires<ArgumentException>(description.Target.IsTexture2DArrayTarget());
        }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
        {
            this.Initialize();
            using (Binding texture2dBinding = new Binding(this))
            {
                GL.TexSubImage3D(this.Target, level, xOffset, yOffset, zOffset, width, height, depth, pixelFormat, pixelType, data);
            }
        }
    }
}
