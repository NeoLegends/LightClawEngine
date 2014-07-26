using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class Texture : GLObject, IBindable
    {
        public TextureTarget Target { get; private set; }

        public Texture()
            : base(GL.GenTexture())
        {

        }

        public void Bind()
        {
            GL.BindTexture(this.Target, this);
        }

        public void Unbind()
        {
            GL.BindTexture(this.Target, 0);
        }
    }
}
