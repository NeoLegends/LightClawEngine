using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.Graphics.OpenGL
{
    [DataContract, ProtoContract]
    public struct VertexAttributePointer : ICloneable, IEquatable<VertexAttributePointer>
    {
        [DataMember, ProtoMember(1)]
        public int Index { get; private set; }

        [DataMember, ProtoMember(2)]
        public bool Normalize { get; private set; }

        [DataMember, ProtoMember(3)]
        public IntPtr Offset { get; private set; }

        [DataMember, ProtoMember(4)]
        public int Size { get; private set; }

        [DataMember, ProtoMember(5)]
        public int Stride { get; private set; }

        [DataMember, ProtoMember(6)]
        public VertexAttribPointerType Type { get; private set; }
        
        public VertexAttributePointer(int index, int size, VertexAttribPointerType type, bool normalize, int stride, int offset)
            : this(index, size, type, normalize, stride, (IntPtr)offset) { }

        public VertexAttributePointer(int index, int size, VertexAttribPointerType type, bool normalize, int stride, IntPtr offset)
            : this()
        {
            this.Index = index;
            this.Normalize = normalize;
            this.Offset = offset;
            this.Size = size;
            this.Stride = stride;
            this.Type = type;
        }

        public object Clone()
        {
            return new VertexAttributePointer(this.Index, this.Size, this.Type, this.Normalize, this.Stride, this.Offset);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is VertexAttributePointer) ? this.Equals((VertexAttributePointer)obj) : false;
        }

        public bool Equals(VertexAttributePointer other)
        {
            return (this.Index == other.Index) && (this.Normalize == other.Normalize) &&
                   (this.Offset == other.Offset) && (this.Size == other.Size) &&
                   (this.Stride == other.Stride) && (this.Type == other.Type);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Index, this.Normalize, this.Offset, this.Size, this.Stride, this.Type);
        }

        public static bool operator ==(VertexAttributePointer left, VertexAttributePointer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexAttributePointer left, VertexAttributePointer right)
        {
            return !(left == right);
        }
    }
}
