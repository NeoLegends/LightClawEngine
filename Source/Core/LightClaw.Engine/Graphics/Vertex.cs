using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [ProtoContract]
    public struct Vertex
    {
        [ProtoMember(1)]
        public Vector2 TexCoord { get; private set; }

        [ProtoMember(3)]
        public Vector3 Position { get; private set; }

        [ProtoMember(2)]
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
