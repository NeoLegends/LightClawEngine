using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class Texture2D : Texture2DBase, IBindable
    {
        public Texture2D() { }

        public void Update<T>(T[] data, int width, int height)
        {
            using (GLBinding texture2dBinding = new GLBinding(this))
            {
                GL.TexStorage2D(
                    TextureTarget2d.Texture2D,
                    (int)Math.Min(Math.Log(width, 2), Math.Log(height, 2)) + 1, // Generate as many mipmaps as needed to shrink width or height to 1px
                    SizedInternalFormat.Rgba8,
                    width,
                    height
                );
            }
        }
    }
}
