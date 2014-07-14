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
    public struct Rectangled : IEquatable<Rectangled>, IComparable<Rectangled>
#if SYSTEMDRAWING_INTEROP
                               , IEquatable<System.Drawing.RectangleF>
#endif
    {
        [DataMember, ProtoMember(1)]
        public Vector2d Position { get; private set; }

        [DataMember, ProtoMember(2)]
        public Sized Size { get; private set; }

        public double Width
        {
            get
            {
                return this.Size.Width;
            }
        }

        public double Height
        {
            get
            {
                return this.Size.Height;
            }
        }

        public double Top
        {
            get
            {
                return this.Position.Y;
            }
        }

        public double Left
        {
            get
            {
                return this.Position.X;
            }
        }

        public double Bottom
        {
            get
            {
                return this.Top + this.Size.Height;
            }
        }

        public double Right
        {
            get
            {
                return this.Left + this.Size.Width;
            }
        }

        public double Area
        {
            get
            {
                return this.Width * this.Height;
            }
        }

#if SYSTEMDRAWING_INTEROP

        public Rectangled(System.Drawing.RectangleF rect) : this(rect.Location, rect.Size) { }

#endif

        public Rectangled(Rectangle rectangle) : this(rectangle.Position, rectangle.Size) { }

        public Rectangled(double posX, double posY, double sizeW, double sizeH)
            : this(new Vector2d(posX, posY), new Sized(sizeW, sizeH))
        {
            Contract.Requires<ArgumentOutOfRangeException>(sizeW >= 0.0f);
            Contract.Requires<ArgumentOutOfRangeException>(sizeH >= 0.0f);
        }

        public Rectangled(Vector2d position, Sized size)
            : this()
        {
            this.Position = position;
            this.Size = size;
        }

        public int CompareTo(Rectangled other)
        {
            return this.Area.CompareTo(other.Area);
        }

        public bool ContainsX(double x)
        {
            return Rectangled.ContainsX(this, x);
        }

        public bool ContainsY(double y)
        {
            return Rectangled.ContainsY(this, y);
        }

#if SYSTEMDRAWING_INTEROP

        public bool Equals(System.Drawing.RectangleF other)
        {
            return (this.Position == (Vector2)other.Location) && (this.Size == (Sized)other.Size);
        }

#endif

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

#if SYSTEMDRAWING_INTEROP
            if (obj is System.Drawing.RectangleF)
            {
                return this.Equals((System.Drawing.RectangleF)obj);
            }
#endif

            return (obj is Rectangled) ? this.Equals((Rectangled)obj) : false;
        }

        public bool Equals(Rectangled other)
        {
            return (this.Position == other.Position) && (this.Size == other.Size);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Constants.HashStart * Constants.HashFactor + this.Position.GetHashCode();
                hash = hash * Constants.HashFactor + this.Size.GetHashCode();
                return hash;
            }
        }

        public bool Intersects(Rectangled other)
        {
            return Rectangled.Intersect(this, other);
        }

        public static bool ContainsX(Rectangled rect, double x)
        {
            return (rect.Left <= x) && (rect.Right >= x);
        }

        public static bool ContainsY(Rectangled rect, double y)
        {
            return (rect.Top <= y) && (rect.Bottom >= y);
        }

        public static bool Intersect(Rectangled left, Rectangled right)
        {
            return ((right.Position.X + right.Width > left.Position.X) &&
                   ((right.Position.X < left.Position.X) || left.ContainsX(right.Position.X))) &&
                   ((right.Position.Y + right.Height > left.Position.Y) &&
                   ((right.Position.Y > left.Position.Y) || left.ContainsY(right.Position.Y)));
        }

        public static implicit operator Rectangled(Rectangle rect)
        {
            return new Rectangled(rect);
        }

#if SYSTEMDRAWING_INTEROP

        public static implicit operator Rectangled(System.Drawing.RectangleF rect)
        {
            return new Rectangled(rect);
        }

        public static implicit operator System.Drawing.RectangleF(Rectangled rect)
        {
            return new System.Drawing.RectangleF(rect.Position, rect.Size);
        }

#endif
    }
}
