using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [DataContract, ProtoContract]
    public struct VertexAttributePointer
    {
        [DataMember, ProtoMember(1)]
        public int Index { get; private set; }

        [DataMember, ProtoMember(2)]
        public bool IsNormalized { get; private set; }

        [DataMember, ProtoMember(3)]
        public IntPtr Offset { get; private set; }

        [DataMember, ProtoMember(4)]
        public int Size { get; private set; }

        [DataMember, ProtoMember(5)]
        public int Stride { get; private set; }

        [DataMember, ProtoMember(6)]
        public VertexAttribPointerType Type { get; private set; }

        public VertexAttributePointer(int index, int size, VertexAttribPointerType type, bool isNormalized, int stride, IntPtr offset)
            : this()
        {
            this.Index = index;
            this.IsNormalized = isNormalized;
            this.Offset = offset;
            this.Size = size;
            this.Stride = stride;
            this.Type = type;
        }
    }
}
