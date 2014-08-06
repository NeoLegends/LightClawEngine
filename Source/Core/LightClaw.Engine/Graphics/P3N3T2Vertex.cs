using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [DataContract, ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct P3N3T2Vertex
    {
        [DataMember, ProtoMember(1)]
        public Vector3 Position;

        [DataMember, ProtoMember(2)]
        public Vector3 Normal;

        [DataMember, ProtoMember(3)]
        public Vector2 TexCoord;

        public P3N3T2Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            this.Normal = normal;
            this.Position = position;
            this.TexCoord = texCoord;
        }

        public P3N3T2Vertex(float[] data)
            : this(data, 0)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Length >= 8);
        }

        public P3N3T2Vertex(float[] data, int offset)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(offset + 8 <= data.Length);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);

            this.Position.X = data[offset];
            this.Position.Y = data[offset + 1];
            this.Position.Z = data[offset + 2];
            this.Normal.X = data[offset + 3];
            this.Normal.Y = data[offset + 4];
            this.Normal.Z = data[offset + 5];
            this.TexCoord.X = data[offset + 6];
            this.TexCoord.Y = data[offset + 7];
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
            return HashF.GetHashCode(this.Position, this.Normal, this.TexCoord);
        }

        public float[] ToArray()
        {
            return new[]
            {
                this.Position.X, this.Position.Y, this.Position.Z,
                this.Normal.X, this.Normal.Y, this.Normal.Z,
                this.TexCoord.X, this.TexCoord.Y
            };
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
