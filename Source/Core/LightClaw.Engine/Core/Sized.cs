using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [DataContract, ProtoContract]
    public struct Sized : IEquatable<Sized>, IComparable<Sized>
#if SYSTEMDRAWING_INTEROP
                          , IEquatable<System.Drawing.SizeF>, IComparable<System.Drawing.SizeF>
#endif
    {
        [DataMember, ProtoMember(1)]
        public double Width { get; private set; }

        [DataMember, ProtoMember(2)]
        public double Height { get; private set; }

        public bool IsEmpty
        {
            get
            {
                return (this.Width * this.Height) == 0.0f;
            }
        }

#if SYSTEMDRAWING_INTEROP

        public Sized(System.Drawing.SizeF size)
            : this(size.Width, size.Height)
        {
            Contract.Requires<ArgumentOutOfRangeException>(size.Width >= 0.0);
            Contract.Requires<ArgumentOutOfRangeException>(size.Height >= 0.0);
        }

#endif

        public Sized(Size size) : this(size.Width, size.Height) { }

        public Sized(Vector2d vector)
            : this(vector.X, vector.Y) 
        { 
            Contract.Requires<ArgumentOutOfRangeException>(vector.X >= 0.0f);
            Contract.Requires<ArgumentOutOfRangeException>(vector.Y >= 0.0f);
        }

        public Sized(double width, double height)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0.0f);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0.0f);

            this.Width = width;
            this.Height = height;
        }

        public int CompareTo(Sized other)
        {
            return (this.Width * this.Height).CompareTo(other.Width * other.Height);
        }

#if SYSTEMDRAWING_INTEROP

        public int CompareTo(System.Drawing.SizeF other)
        {
            return (this.Width * this.Height).CompareTo(other.Width * other.Height);
        }

#endif

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

#if SYSTEMDRAWING_INTEROP
            if (obj is System.Drawing.SizeF)
            {
                return this.Equals((System.Drawing.SizeF)obj);
            }
#endif

            return (obj is Sized) ? this.Equals((Sized)obj) : false;
        }

        public bool Equals(Sized other)
        {
            return (this.Width == other.Width) && (this.Height == other.Height);
        }

#if SYSTEMDRAWING_INTEROP

        public bool Equals(System.Drawing.SizeF other)
        {
            return (this.Width == other.Width) && (this.Height == other.Height);
        }

#endif

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Constants.HashStart * Constants.HashFactor + this.Width.GetHashCode();
                hash = hash * Constants.HashFactor + this.Height.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Sized left, Sized right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Sized left, Sized right)
        {
            return !(left == right);
        }

        public static implicit operator Sized(Size size)
        {
            return new Sized(size);
        }

        public static explicit operator Sized(Vector2d vector)
        {
            return new Sized(vector);
        }

#if SYSTEMDRAWING_INTEROP

        public static implicit operator Sized(System.Drawing.SizeF size)
        {
            return new Sized(size);
        }

        public static implicit operator System.Drawing.SizeF(Sized size)
        {
            return new System.Drawing.SizeF((float)size.Width, (float)size.Height);
        }

#endif

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Width >= 0.0f);
            Contract.Invariant(this.Height >= 0.0f);
        }
    }
}
