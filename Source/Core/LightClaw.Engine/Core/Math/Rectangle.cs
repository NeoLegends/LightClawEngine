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
    public struct Rectangle : IEquatable<Rectangle>, IComparable<Rectangle>
#if SYSTEMDRAWING_INTEROP
                              , IEquatable<System.Drawing.RectangleF>
#endif
    {
        [DataMember, ProtoMember(1)]
        public Vector2 Position { get; private set; }

        [DataMember, ProtoMember(2)]
        public Size Size { get; private set; }

        public float Width
        {
            get
            {
                return this.Size.Width;
            }
        }

        public float Height
        {
            get
            {
                return this.Size.Height;
            }
        }

        public float Top
        {
            get
            {
                return this.Position.Y;
            }
        }

        public float Left
        {
            get
            {
                return this.Position.X;
            }
        }

        public float Bottom
        {
            get
            {
                return this.Top + this.Size.Height;
            }
        }

        public float Right
        {
            get
            {
                return this.Left + this.Size.Width;
            }
        }

        public float Area
        {
            get
            {
                return this.Width * this.Height;
            }
        }

#if SYSTEMDRAWING_INTEROP

        public Rectangle(System.Drawing.RectangleF rect) : this(rect.Location, rect.Size) { }

#endif

        public Rectangle(Rectangled rect) : this((Vector2)rect.Position, (Size)rect.Size) { }

        public Rectangle(float posX, float posY, float sizeW, float sizeH)
            : this(new Vector2(posX, posY), new Size(sizeW, sizeH))
        {
            Contract.Requires<ArgumentOutOfRangeException>(sizeW >= 0.0f);
            Contract.Requires<ArgumentOutOfRangeException>(sizeH >= 0.0f);
        }

        public Rectangle(Vector2 position, Size size)
            : this()
        {
            this.Position = position;
            this.Size = size;
        }

        public int CompareTo(Rectangle other)
        {
            return this.Area.CompareTo(other.Area);
        }

        public bool ContainsX(float x)
        {
            return Rectangle.ContainsX(this, x);
        }

        public bool ContainsY(float y)
        {
            return Rectangle.ContainsY(this, y);
        }

#if SYSTEMDRAWING_INTEROP

        public bool Equals(System.Drawing.RectangleF other)
        {
            return (this.Position == (Vector2)other.Location) && (this.Size == (Size)other.Size);
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

            return (obj is Rectangle) ? this.Equals((Rectangle)obj) : false;
        }

        public bool Equals(Rectangle other)
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

        public bool Intersects(Rectangle other)
        {
            return Rectangle.Intersect(this, other);
        }

        public static bool ContainsX(Rectangle rect, float x)
        {
            return (rect.Left <= x) && (rect.Right >= x);
        }

        public static bool ContainsY(Rectangle rect, float y)
        {
            return (rect.Top <= y) && (rect.Bottom >= y);
        }

        public static bool Intersect(Rectangle left, Rectangle right)
        {
            return ((right.Position.X + right.Width > left.Position.X) &&
                   ((right.Position.X < left.Position.X) || left.ContainsX(right.Position.X))) &&
                   ((right.Position.Y + right.Height > left.Position.Y) &&
                   ((right.Position.Y > left.Position.Y) || left.ContainsY(right.Position.Y)));
        }

        public static explicit operator Rectangle(Rectangled rect)
        {
            return new Rectangle(rect);
        }

#if SYSTEMDRAWING_INTEROP

        public static implicit operator Rectangle(System.Drawing.RectangleF rect)
        {
            return new Rectangle(rect);
        }

        public static implicit operator System.Drawing.RectangleF(Rectangle rect)
        {
            return new System.Drawing.RectangleF(rect.Position, rect.Size);
        }

#endif
    }
}
