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
    public class Texture3D : Texture3DBase
    {
        public Texture3D(TextureDescription description)
            : base(description)
        {
            Contract.Requires<ArgumentException>(description.Target.IsTexture3DTarget());

            this.VerifyAccess();
        }

        public override void Set(IntPtr data, PixelFormat pixelFormat, PixelType pixelType, int width, int height, int depth, int xOffset, int yOffset, int zOffset, int level)
        {
            this.VerifyAccess();

            using (Binding texture2dBinding = new Binding(this))
            {
                GL.TexSubImage3D(this.Target, level, xOffset, yOffset, zOffset, width, height, depth, pixelFormat, pixelType, data);
            }
        }
    }
}
