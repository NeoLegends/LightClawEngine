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
    public class MeshPart : Entity, IDisposable, IDrawable
    {
        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public Material Material { get; private set; }

        public VertexArrayObject Vao { get; private set; }

        public MeshPart(Material material, VertexArrayObject vao)
        {
            Contract.Requires<ArgumentNullException>(material != null);
            Contract.Requires<ArgumentNullException>(vao != null);

            this.Material = material;
            this.Vao = vao;
        }

        ~MeshPart()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Draw()
        {
            this.Raise(this.Drawing);

            using (GLBinding materialBinding = new GLBinding(this.Material))
            using (GLBinding vaoBinding = new GLBinding(this.Vao))
            {
                GL.DrawElements(BeginMode.Triangles, this.Vao.IndexCount, DrawElementsType.UnsignedShort, 0);
            }

            this.Raise(this.Drawn);
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}
