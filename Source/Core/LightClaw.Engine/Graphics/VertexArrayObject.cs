using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class VertexArrayObject : GLObject
    {
        public Buffer[] IndexBuffers { get; private set; }

        public Buffer[] VertexBuffers { get; private set; }

        public VertexArrayObject(IEnumerable<Buffer> indexBuffers, IEnumerable<Buffer> vertexBuffers)
            : base(GL.GenVertexArray())
        {
            Contract.Requires<ArgumentNullException>(indexBuffers != null);
            Contract.Requires<ArgumentNullException>(vertexBuffers != null);
            Contract.Requires<ArgumentException>(indexBuffers.Count() == vertexBuffers.Count());

            this.IndexBuffers = indexBuffers.ToArray();
            this.VertexBuffers = vertexBuffers.ToArray();

            for (int i = 0; i < this.IndexBuffers.Length; i++)
            {
                throw new NotImplementedException();
            }
        }
    }
}
