using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [DataContract, ProtoContract]
    public struct VertexDataLayout : ICloneable, IEquatable<VertexDataLayout>
    {
        [DataMember, ProtoMember(1)]
        public bool Normalize { get; private set; }

        [DataMember, ProtoMember(2)]
        public int Size { get; private set; }

        [DataMember, ProtoMember(3)]
        public VertexAttribPointerType Type { get; private set; }

        public VertexDataLayout(bool normalize, int size, VertexAttribPointerType type)
            : this()
        {
            this.Normalize = normalize;
            this.Size = size;
            this.Type = type;
        }

        public object Clone()
        {
            return new VertexDataLayout(this.Normalize, this.Size, this.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is VertexDataLayout) ? this.Equals((VertexDataLayout)obj) : false;
        }

        public bool Equals(VertexDataLayout other)
        {
            return (this.Normalize == other.Normalize) && (this.Size == other.Size) && (this.Type == other.Type);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Normalize, this.Size, this.Type);
        }

        public static bool operator ==(VertexDataLayout left, VertexDataLayout right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexDataLayout left, VertexDataLayout right)
        {
            return !(left == right);
        }
    }
}
