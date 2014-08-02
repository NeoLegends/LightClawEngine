using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct P3N3T2Vertex
    {
        [DataMember]
        public Vector3 Position;

        [DataMember]
        public Vector3 Normal;

        [DataMember]
        public Vector2 TexCoord;

        public P3N3T2Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            this.Normal = normal;
            this.Position = position;
            this.TexCoord = texCoord;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is P3N3T2Vertex) ? this.Equals((P3N3T2Vertex)obj) : false;
        }

        public bool Equals(P3N3T2Vertex other)
        {
            return (this.Position == other.Position) && (this.Normal == other.Normal) && (this.TexCoord == other.TexCoord);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Constants.HashStart * Constants.HashFactor + this.Position.GetHashCode();
                hash = hash * Constants.HashFactor + this.Normal.GetHashCode();
                hash = hash * Constants.HashFactor + this.TexCoord.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(P3N3T2Vertex left, P3N3T2Vertex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(P3N3T2Vertex left, P3N3T2Vertex right)
        {
            return !(left == right);
        }
    }
}
