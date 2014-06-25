using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LightClaw.Engine.Graphics
{
    public struct Vertex
    {
        public Vector2 TexCoord { get; private set; }

        public Vector3 Position { get; private set; }

        public Vector3 Normal { get; private set; }

        public Vertex(Vector2 texCoord, Vector3 position, Vector3 normal)
            : this()
        {
            this.Normal = normal;
            this.Position = position;
            this.TexCoord = texCoord;
        }
    }
}
