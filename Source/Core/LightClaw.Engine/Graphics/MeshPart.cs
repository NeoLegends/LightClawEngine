using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class MeshPart : IDrawable
    {
        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public ShaderProgram Shader { get; private set; }

        public VertexArrayObject Vao { get; private set; }

        public void Draw()
        {
            this.Raise(this.Drawing);

            using (BindableClause shaderReleaser = new BindableClause(this.Shader))
            using (BindableClause vaoReleaser = new BindableClause(this.Vao))
            {
                GL.DrawElements(BeginMode.Triangles, this.Vao.IndexCount, DrawElementsType.UnsignedShort, 0);
            }

            this.Raise(this.Drawn);
        }

        private void Raise(EventHandler<ParameterEventArgs> handler, ParameterEventArgs args = null)
        {
            if (handler != null)
            {
                handler(this, args ?? new ParameterEventArgs());
            }
        }
    }
}
