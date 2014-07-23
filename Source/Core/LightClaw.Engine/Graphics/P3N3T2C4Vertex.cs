using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public struct P3N3T2C4Vertex
    {
        public Vector3 Position { get; private set; }

        public Vector3 Normal { get; private set; }

        public Vector2 TexCoord { get; private set; }

        public ColorF Color { get; private set; }

        public P3N3T2C4Vertex(Vector3 position, Vector3 normal, Vector2 texCoord, ColorF color)
            : this()
        {
            this.Normal = normal;
            this.Position = position;
            this.TexCoord = texCoord;
            this.Color = color;
        }
    }
}
