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
    public class MeshPart : Entity, IDrawable
    {
        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public ShaderProgram Shader { get; private set; }

        public VertexArrayObject Vao { get; private set; }

        public MeshPart(ShaderProgram shader, VertexArrayObject vao)
        {
            Contract.Requires<ArgumentNullException>(shader != null);
            Contract.Requires<ArgumentNullException>(vao != null);

            this.Shader = shader;
            this.Vao = vao;
        }

        public void Draw()
        {
            this.Raise(this.Drawing);

            using (GLBinding shaderBinding = new GLBinding(this.Shader))
            using (GLBinding vaoBinding = new GLBinding(this.Vao))
            {
                GL.DrawElements(BeginMode.Triangles, this.Vao.IndexCount, DrawElementsType.UnsignedShort, 0);
            }

            this.Raise(this.Drawn);
        }
    }
}
