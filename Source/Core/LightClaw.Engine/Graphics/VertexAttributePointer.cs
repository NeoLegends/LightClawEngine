using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public struct VertexAttributePointer
    {
        [DataMember]
        public int Index { get; private set; }

        [DataMember]
        public bool IsNormalized { get; private set; }

        [DataMember]
        public IntPtr Offset { get; private set; }

        [DataMember]
        public int Size { get; private set; }

        [DataMember]
        public int Stride { get; private set; }

        [DataMember]
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
