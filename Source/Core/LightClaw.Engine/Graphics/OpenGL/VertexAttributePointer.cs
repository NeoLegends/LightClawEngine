using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    [Serializable, DataContract]
    [DebuggerDisplay("Index = {Index}, Size = {Size}, Type = {Type}, Stride = {Stride}, Offset = {Offset}, Normalize = {Normalize}")]
    public struct VertexAttributePointer : IBindable, ICloneable, IEquatable<VertexAttributePointer>
    {
        [DataMember]
        public VertexAttributeLocation Index { get; private set; }

        [DataMember]
        public bool Normalize { get; private set; }

        [DataMember]
        public IntPtr Offset { get; private set; }

        [DataMember]
        public int Size { get; private set; }

        [DataMember]
        public int Stride { get; private set; }

        [DataMember]
        public VertexAttribPointerType Type { get; private set; }

        public VertexAttributePointer(VertexAttributeLocation index, int size, VertexAttribPointerType type, bool normalize, int stride, int offset)
            : this(index, size, type, normalize, stride, (IntPtr)offset)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(size >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(stride >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
        }

        public VertexAttributePointer(VertexAttributeLocation index, int size, VertexAttribPointerType type, bool normalize, int stride, IntPtr offset)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(size >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(stride >= 0);
            Contract.Requires<ArgumentOutOfRangeException>((int)offset >= 0);

            this.Index = index;
            this.Normalize = normalize;
            this.Offset = offset;
            this.Size = size;
            this.Stride = stride;
            this.Type = type;
        }

        public void Apply()
        {
            GL.VertexAttribPointer(
                this.Index,
                this.Size,
                this.Type,
                this.Normalize,
                this.Stride,
                this.Offset
            );
        }

        void IBindable.Bind()
        {
            this.Enable();
        }

        public object Clone()
        {
            return new VertexAttributePointer(this.Index, this.Size, this.Type, this.Normalize, this.Stride, this.Offset);
        }

        public void Disable()
        {
            GL.DisableVertexAttribArray(this.Index);
        }

        public void Enable()
        {
            GL.EnableVertexAttribArray(this.Index);
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

        void IBindable.Unbind()
        {
            this.Disable();
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
