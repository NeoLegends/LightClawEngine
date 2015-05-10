using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point : ICloneable, IEquatable<Point>
    {
        public int X;

        public int Y;

        public Point(int x, int y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public object Clone()
        {
            return new Point(this.X, this.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is Point) ? this.Equals((Point)obj) : false;
        }

        public bool Equals(Point other)
        {
            return (this.X == other.X) && (this.Y == other.Y);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.X, this.Y);
        }

        public static float Distance(Point a, Point b)
        {
            return (float)Math.Sqrt(DistanceSquared(a, b));
        }

        public static float DistanceSquared(Point a, Point b)
        {
            float x = a.X - b.X;
            float y = a.Y - b.Y;

            return (x * x) + (y * y);
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }
    }
}
