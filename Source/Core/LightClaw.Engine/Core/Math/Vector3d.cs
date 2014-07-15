using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using LightClaw.Engine.Graphics;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a vector with three components. (X, Y, Z)
    /// </summary>
    /// <seealso cref="LightClaw.Engine.Core.Vector2"/>
    /// <seealso cref="LightClaw.Engine.Core.Vector4"/>
    [StructureInformation(3, 64, true)]
    [Serializable, DataContract, ProtoContract]
    public struct Vector3d : ICloneable, IEquatable<Vector3d>, IComparable<Vector3d>
    {
        #region Predefined Vectors

        /// <summary>
        /// A <see cref="Vector3"/> with all of its components set to zero.
        /// </summary>
        public static readonly Vector3d Zero = new Vector3d();

        /// <summary>
        /// The X unit <see cref="Vector3"/> (1, 0, 0).
        /// </summary>
        public static readonly Vector3d UnitX = new Vector3d(1.0, 0.0, 0.0);

        /// <summary>
        /// The Y unit <see cref="Vector3"/> (0, 1, 0).
        /// </summary>
        public static readonly Vector3d UnitY = new Vector3d(0.0, 1.0, 0.0);

        /// <summary>
        /// The Z unit <see cref="Vector3"/> (0, 0, 1).
        /// </summary>
        public static readonly Vector3d UnitZ = new Vector3d(0.0, 0.0, 1.0);

        /// <summary>
        /// A <see cref="Vector3"/> with all of its components set to one.
        /// </summary>
        public static readonly Vector3d One = new Vector3d(1.0, 1.0, 1.0);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating up (0, 1, 0).
        /// </summary>
        public static readonly Vector3d Up = new Vector3d(0.0, 1.0, 0.0);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating down (0, -1, 0).
        /// </summary>
        public static readonly Vector3d Down = new Vector3d(0.0, -1.0, 0.0);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating left (-1, 0, 0).
        /// </summary>
        public static readonly Vector3d Left = new Vector3d(-1.0, 0.0, 0.0);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating right (1, 0, 0).
        /// </summary>
        public static readonly Vector3d Right = new Vector3d(1.0, 0.0, 0.0);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating forward in a right-handed coordinate system (0, 0, -1).
        /// </summary>
        public static readonly Vector3d ForwardRH = new Vector3d(0.0, 0.0, -1.0);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating forward in a left-handed coordinate system (0, 0, 1).
        /// </summary>
        public static readonly Vector3d ForwardLH = new Vector3d(0.0, 0.0, 1.0);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating backward in a right-handed coordinate system (0, 0, 1).
        /// </summary>
        public static readonly Vector3d BackwardRH = new Vector3d(0.0, 0.0, 1.0);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating backward in a left-handed coordinate system (0, 0, -1).
        /// </summary>
        public static readonly Vector3d BackwardLH = new Vector3d(0.0, 0.0, -1.0);

        /// <summary>
        /// The <see cref="Random"/> instance used to obtain the random vector.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// Returns a random <see cref="Vector3"/>.
        /// </summary>
        public static Vector3d Random
        {
            get
            {
                lock (random)
                {
                    return new Vector3d(
                        random.NextDouble() * 100,
                        random.NextDouble() * 100,
                        random.NextDouble() * 100
                    );
                }
            }
        }

        #endregion

        /// <summary>
        /// X length.
        /// </summary>
        [DataMember, ProtoMember(1)]
        public double X { get; private set; }

        /// <summary>
        /// Y length.
        /// </summary>
        [DataMember, ProtoMember(2)]
        public double Y { get; private set; }

        /// <summary>
        /// Z length.
        /// </summary>
        [DataMember, ProtoMember(3)]
        public double Z { get; private set; }

        /// <summary>
        /// The squared length of this <see cref="Vector3"/>.
        /// </summary>
        public double SquaredLength
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
            }
        }

        /// <summary>
        /// The <see cref="Vector3"/>s length. See remarks.
        /// </summary>
        /// <remarks>
        /// For comparison of vector lengths it's usually better to use <see cref="P:SquaredLength"/>, as squared lengths
        /// are enough for simple comparison. Thus, you can omit the expensive <see cref="Math.Sqrt"/>.
        /// </remarks>
        public double Length
        {
            get
            {
                return Math.Sqrt(this.SquaredLength);
            }
        }

        /// <summary>
        /// All three components contained in an array.
        /// </summary>
        public double[] Array
        {
            get
            {
                Contract.Ensures(Contract.Result<double[]>() != null);

                return new double[] { this.X, this.Y, this.Z };
            }
        }

        /// <summary>
        /// Enables accessing one of the components through an index.
        /// </summary>
        /// <param name="index">The index of the component to access.</param>
        /// <returns>The component at the given index.</returns>
        public double this[int index]
        {
            get
            {
                Contract.Requires<IndexOutOfRangeException>(index >= 0 && index < 3);

                switch (index)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
                    case 2:
                        return this.Z;
                    default:
                        throw new IndexOutOfRangeException("Index has to be greater than or equal to zero and smaller than three.");
                }
            }
        }

#if SYSTEMDRAWING_INTEROP

        /// <summary>
        /// Creates a new <see cref="Vector3"/> from the given <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to create a new <see cref="Vector3"/> from.</param>
        public Vector3d(System.Drawing.Color color) : this(Color.ToFloat(color.R), Color.ToFloat(color.G), Color.ToFloat(color.B)) { }

#endif

        /// <summary>
        /// Creates a <see cref="Vector3"/> from the given <see cref="Vector2"/> and a <see cref="Single"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2"/> to create this <see cref="Vector3"/> from.</param>
        /// <param name="z">The Z-component of the <see cref="Vector3"/></param>
        public Vector3d(Vector2d vector, double z) : this(vector.X, vector.Y, z) { }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> from the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> to create a <see cref="Vector3"/> from.</param>
        public Vector3d(Vector3 vector) : this(vector.X, vector.Y, vector.Z) { }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> setting the X and Y and Z value.
        /// </summary>
        /// <param name="x">The <see cref="Vector3"/>'s X value.</param>
        /// <param name="y">The <see cref="Vector3"/>'s Y value.</param>
        /// <param name="z">The <see cref="Vector3"/>'s Z value.</param>
        public Vector3d(double x, double y, double z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Returns a negated copy of the current <see cref="Vector3"/>.
        /// </summary>
        /// <returns>The negated <see cref="Vector3"/>.</returns>
        public Vector3d Negate()
        {
            return Vector3d.Negate(ref this);
        }

        /// <summary>
        /// Returns a normalized copy of the <see cref="Vector3"/>.
        /// </summary>
        /// <returns>The normalized <see cref="Vector3"/>.</returns>
        public Vector3d Normalize()
        {
            return Vector3d.Normalize(ref this);
        }

        /// <summary>
        /// Creates a new deep copy of the <see cref="Vector3"/>.
        /// </summary>
        /// <returns>The cloned <see cref="Vector3"/>.</returns>
        public object Clone()
        {
            Contract.Ensures(Contract.Result<object>() != null);

            return new Vector3d(this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Compares the <see cref="Vector3"/> to the other by comparing the length.
        /// </summary>
        /// <param name="other">The <see cref="Vector3"/> to compare against.</param>
        /// <returns>An integer indicating their relative size to each other.</returns>
        public int CompareTo(Vector3d other)
        {
            return this.SquaredLength.CompareTo(other.SquaredLength);
        }

        /// <summary>
        /// Checks whether the <see cref="Vector3"/> equals the given <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/>to check against.</param>
        /// <returns>Whether both <see cref="Vector3"/>s are equal.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            else if (ReferenceEquals(obj, this))
                return true;

            return (obj is Vector3d) ? this.Equals((Vector3d)obj) : false;
        }

        /// <summary>
        /// Checks two <see cref="Vector3"/>s for equality.
        /// </summary>
        /// <param name="other">The other <see cref="Vector3"/> to check.</param>
        /// <returns>Whether both <see cref="Vector3"/>s are equal.</returns>
        public bool Equals(Vector3d other)
        {
            return ((this.X == other.X) && (this.Y == other.Y) && (this.Z == other.Z));
        }

        /// <summary>
        /// Returns the instance's hash code.
        /// </summary>
        /// <returns>The instance's hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Constants.HashStart * Constants.HashFactor + this.X.GetHashCode();
                hash = hash * Constants.HashFactor + this.Y.GetHashCode();
                hash = hash * Constants.HashFactor + this.Z.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.string"/> representing the <see cref="Vector3"/>.
        /// </summary>
        /// <returns>A <see cref="System.string"/> representing the <see cref="Vector3"/>.</returns>
        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Format("({0}|{1}|{2})", this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> containing the 3D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="vertex1">A <see cref="Vector3"/> containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="vertex2">A <see cref="Vector3"/> containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="vertex3">A <see cref="Vector3"/> containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="vertex2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="vertex3"/>).</param>
        public static Vector3d Barycentric(ref Vector3d vertex1, ref Vector3d vertex2, ref Vector3d vertex3, double amount1, double amount2)
        {
            return new Vector3d()
            {
                X = (vertex1.X + (amount1 * (vertex2.X - vertex1.X))) + (amount2 * (vertex3.X - vertex1.X)),
                Y = (vertex1.Y + (amount1 * (vertex2.Y - vertex1.Y))) + (amount2 * (vertex3.Y - vertex1.Y)),
                Z = (vertex1.Z + (amount1 * (vertex2.Z - vertex1.Z))) + (amount2 * (vertex3.Z - vertex1.Z))
            };
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector3d CatmullRom(ref Vector3d value1, ref Vector3d value2, ref Vector3d value3, ref Vector3d value4, ref double amount)
        {
            double squared = amount * amount;
            double cubed = amount * squared;

            return new Vector3d()
            {
                X = 0.5 * ((((2.0 * value2.X) + ((-value1.X + value3.X) * amount)) +
                    (((((2.0 * value1.X) - (5.0 * value2.X)) + (4.0 * value3.X)) - value4.X) * squared)) +
                    ((((-value1.X + (3.0 * value2.X)) - (3.0 * value3.X)) + value4.X) * cubed)),

                Y = 0.5 * ((((2.0 * value2.Y) + ((-value1.Y + value3.Y) * amount)) +
                    (((((2.0 * value1.Y) - (5.0 * value2.Y)) + (4.0 * value3.Y)) - value4.Y) * squared)) +
                    ((((-value1.Y + (3.0 * value2.Y)) - (3.0 * value3.Y)) + value4.Y) * cubed)),

                Z = 0.5 * ((((2.0 * value2.Z) + ((-value1.Z + value3.Z) * amount)) +
                    (((((2.0 * value1.Z) - (5.0 * value2.Z)) + (4.0 * value3.Z)) - value4.Z) * squared)) +
                    ((((-value1.Z + (3.0 * value2.Z)) - (3.0 * value3.Z)) + value4.Z) * cubed))
            };
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public static Vector3d Clamp(ref Vector3d value, ref Vector3d max, ref Vector3d min)
        {
            return new Vector3d()
            {
                X = (value.X > max.X) ? max.X : (value.X < min.X) ? min.X : value.X,
                Y = (value.Y > max.Y) ? max.Y : (value.Y < min.Y) ? min.Y : value.Y,
                Z = (value.Z > max.Z) ? max.Z : (value.Z < min.Z) ? min.Z : value.Z
            };
        }

        /// <summary>
        /// Calculates the cross product of two <see cref="Vector3"/>s.
        /// </summary>
        /// <param name="left">First source <see cref="Vector3"/>.</param>
        /// <param name="right">Second source <see cref="Vector3"/>.</param>
        public static Vector3d Cross(ref Vector3d left, ref Vector3d right)
        {
            return new Vector3d()
            {
                X = (left.Y * right.Z) - (left.Z * right.Y),
                Y = (left.Z * right.X) - (left.X * right.Z),
                Z = (left.X * right.Y) - (left.Y * right.X)
            };
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="M:Vector3d.DistanceSquared(Vector3, Vector3)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static double Distance(ref Vector3d value1, ref Vector3d value2)
        {
            return (double)Math.Sqrt(DistanceSquared(ref value1, ref value2));
        }

        /// <summary>
        /// Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        /// <remarks>
        /// Distance squared is the value before taking the square root. 
        /// Distance squared can often be used in place of distance if relative comparisons are being made. 
        /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
        /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
        /// involves two square roots, which are computationally expensive. However, using distance squared 
        /// provides the same information and avoids calculating two square roots.
        /// </remarks>
        public static double DistanceSquared(ref Vector3d value1, ref Vector3d value2)
        {
            double dX = value1.X - value2.X;
            double dY = value1.Y - value2.Y;
            double dZ = value1.Z - value2.Z;

            return (dX * dX) + (dY * dY) + (dZ * dZ);
        }

        /// <summary>
        /// Calculates the dot product of two <see cref="Vector3"/>s.
        /// </summary>
        /// <param name="left">First source <see cref="Vector3"/>.</param>
        /// <param name="right">Second source <see cref="Vector3"/>.</param>
        /// <returns>The dot product of the two <see cref="Vector3"/>s.</returns>
        public static double Dot(ref Vector3d left, ref Vector3d right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector3"/>.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector3"/>.</param>
        /// <param name="value2">Second source position <see cref="Vector3"/>.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector3"/>.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector3d Hermite(ref Vector3d value1, ref Vector3d tangent1, ref Vector3d value2, ref Vector3d tangent2, double amount)
        {
            double squared = amount * amount;
            double cubed = amount * squared;
            double part1 = ((2.0 * cubed) - (3.0 * squared)) + 1.0;
            double part2 = (-2.0 * cubed) + (3.0 * squared);
            double part3 = (cubed - (2.0 * squared)) + amount;
            double part4 = cubed - squared;

            return new Vector3d()
            {
                X = (((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4),
                Y = (((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4),
                Z = (((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4)
            };
        }

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static Vector3d Lerp(ref Vector3d start, ref Vector3d end, double amount)
        {
            return new Vector3d()
            {
                X = MathF.Lerp(start.X, end.X, amount),
                Y = MathF.Lerp(start.Y, end.Y, amount),
                Z = MathF.Lerp(start.Z, end.Z, amount)
            };
        }

        /// <summary>
        /// 	Constructs a new <see cref="Vector3"/> based on the biggest values in one of the two <see cref="Vector3"/>s.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/>.</param>
        /// <param name="b">The second <see cref="Vector3"/>.</param>
        /// <returns>A new <see cref="Vector3"/> with the biggest value of both input values.</returns>
        public static Vector3d Max(ref Vector3d a, ref Vector3d b)
        {
            return new Vector3d()
            {
                X = Math.Max(a.X, b.X),
                Y = Math.Max(a.Y, b.Y),
                Z = Math.Max(a.Z, b.Z)
            };
        }

        /// <summary>
        /// 	Constructs a new <see cref="Vector3"/> based on the smallest values in one of the two <see cref="Vector3"/>s.
        /// </summary>
        /// <param name="a">The first <see cref="Vector3"/>.</param>
        /// <param name="b">The second <see cref="Vector3"/>.</param>
        /// <returns>A new <see cref="Vector3"/> with the smallest value of both input values.</returns>
        public static Vector3d Min(ref Vector3d a, ref Vector3d b)
        {
            return new Vector3d()
            {
                X = Math.Min(a.X, b.X),
                Y = Math.Min(a.Y, b.Y),
                Z = Math.Min(a.Z, b.Z)
            };
        }

        /// <summary>
        /// Reverses the direction of the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">The <see cref="Vector3"/> to negate.</param>
        /// <returns>The negated <see cref="Vector3"/>.</returns>
        public static Vector3d Negate(ref Vector3d value)
        {
            return new Vector3d()
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z
            };
        }

        /// <summary>
        /// Normalizes the specified <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">The <see cref="Vector3"/> to normalize.</param>
        /// <returns>The normalized <see cref="Vector3"/>.</returns>
        public static Vector3d Normalize(ref Vector3d value)
        {
            double length = value.Length;
            if (length != 0.0)
            {
                double inv = 1.0 / length;
                return new Vector3d()
                {
                    X = value.X * inv,
                    Y = value.Y * inv,
                    Z = value.Z * inv
                };
            }
            else
            {
                return (Vector3d)value.Clone();
            }
        }

        /// <summary>
        /// Orthogonalizes a list of vectors.
        /// </summary>
        /// <param name="destination">The list of orthogonalized vectors.</param>
        /// <param name="source">The list of vectors to orthogonalize.</param>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all vectors orthogonal to each other. This
        /// means that any given vector in the list will be orthogonal to any other given vector in the
        /// list.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
        /// tend to be numerically unstable. The numeric stability decreases according to the vectors
        /// position in the list so that the first vector is the most stable and the last vector is the
        /// least stable.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Orthogonalize(Vector3d[] destination, params Vector3d[] source)
        {
            //Uses the modified Gram-Schmidt process.
            //q1 = m1
            //q2 = m2 - ((q1 ⋅ m2) / (q1 ⋅ q1)) * q1
            //q3 = m3 - ((q1 ⋅ m3) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m3) / (q2 ⋅ q2)) * q2
            //q4 = m4 - ((q1 ⋅ m4) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m4) / (q2 ⋅ q2)) * q2 - ((q3 ⋅ m4) / (q3 ⋅ q3)) * q3
            //q5 = ...

            Contract.Requires<ArgumentNullException>(source != null && destination != null);
            Contract.Requires<ArgumentException>(destination.Length > source.Length);

            for (int i = 0; i < source.Length; ++i)
            {
                Vector3d newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= destination[r] * (Vector3d.Dot(ref destination[r], ref newvector) / Vector3d.Dot(ref destination[r], ref destination[r]));
                }

                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Orthonormalizes a list of vectors.
        /// </summary>
        /// <param name="destination">The list of orthonormalized vectors.</param>
        /// <param name="source">The list of vectors to orthonormalize.</param>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all vectors orthogonal to each
        /// other and making all vectors of unit length. This means that any given vector will
        /// be orthogonal to any other given vector in the list.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
        /// tend to be numerically unstable. The numeric stability decreases according to the vectors
        /// position in the list so that the first vector is the most stable and the last vector is the
        /// least stable.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Orthonormalize(Vector3d[] destination, params Vector3d[] source)
        {
            //Uses the modified Gram-Schmidt process.
            //Because we are making unit vectors, we can optimize the math for orthogonalization
            //and simplify the projection operation to remove the division.
            //q1 = m1 / |m1|
            //q2 = (m2 - (q1 ⋅ m2) * q1) / |m2 - (q1 ⋅ m2) * q1|
            //q3 = (m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2) / |m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2|
            //q4 = (m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3) / |m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3|
            //q5 = ...

            Contract.Requires<ArgumentNullException>(source != null && destination != null);
            Contract.Requires<ArgumentException>(destination.Length > source.Length);

            for (int i = 0; i < source.Length; ++i)
            {
                Vector3d newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= destination[r] * Vector3d.Dot(ref destination[r], ref newvector);
                }

                destination[i] = newvector.Normalize();
            }
        }

        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal.
        /// </summary>
        /// <param name="vector">The source <see cref="Vector3"/>.</param>
        /// <param name="normal">Normal of the surface.</param>
        /// <remarks>
        /// Reflect only gives the direction of a reflection off a surface, it does not determine 
        /// whether the original vector was close enough to the surface to hit it.
        /// </remarks>
        public static Vector3d Reflect(ref Vector3d vector, ref Vector3d normal)
        {
            double dot = Vector3d.Dot(ref vector, ref normal);

            return new Vector3d()
            {
                X = vector.X - ((2.0 * dot) * normal.X),
                Y = vector.Y - ((2.0 * dot) * normal.Y),
                Z = vector.Z - ((2.0 * dot) * normal.Z)
            };
        }

        /// <summary>
        /// Performs a cubic interpolation between two <see cref="Vector3"/>s.
        /// </summary>
        /// <param name="start">Start <see cref="Vector3"/>.</param>
        /// <param name="end">End <see cref="Vector3"/>.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        public static Vector3d SmoothStep(ref Vector3d start, ref Vector3d end, double amount)
        {
            amount = MathF.SmoothStep(amount);
            amount = (amount * amount) * (3.0 - (2.0 * amount));

            return new Vector3d()
            {
                X = start.X + ((end.X - start.X) * amount),
                Y = start.Y + ((end.Y - start.Y) * amount),
                Z = start.Z + ((end.Z - start.Z) * amount)
            };
        }

        /// <summary>
        /// 	Adds <see cref="Vector3"/> A to <see cref="Vector3"/> B.
        /// </summary>
        /// <param name="left">First <see cref="Vector3"/> to add.</param>
        /// <param name="right">Second <see cref="Vector3"/> to add.</param>
        /// <returns>The added vector.</returns>
        public static Vector3d operator +(Vector3d left, Vector3d right)
        {
            return new Vector3d
            {
                X = left.X + right.X,
                Y = left.Y + right.Y,
                Z = left.Z + right.Z
            };
        }

        /// <summary>
        /// 	Adds v given length to X and Y of v <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">The value to add.</param>
        /// <param name="right">The length to add to the vector.</param>
        /// <returns>The vector with the added length.</returns>
        public static Vector3d operator +(Vector3d left, double right)
        {
            return new Vector3d
            {
                X = left.X + right,
                Y = left.Y + right,
                Z = left.Z + right
            };
        }

        /// <summary>
        /// Negates the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> to negate.</param>
        /// <returns>The negated <see cref="Vector3"/>.</returns>
        public static Vector3d operator -(Vector3d vector)
        {
            return Vector3d.Negate(ref vector);
        }

        /// <summary>
        /// 	Substracts one <see cref="Vector3"/> from another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">The minuend.</param>
        /// <param name="right">The Substractor.</param>
        /// <returns>The substracted <see cref="Vector3"/>s.</returns>
        public static Vector3d operator -(Vector3d left, Vector3d right)
        {
            return new Vector3d
            {
                X = left.X - right.X,
                Y = left.Y - right.Y,
                Z = left.Z - right.Z
            };
        }

        /// <summary>
        /// 	Substracts v given length from v <see cref="Vector3"/>.
        /// </summary>
        /// <param name="left">The <see cref="Vector3"/> to substract from.</param>
        /// <param name="right">The length to substract from the Vector.</param>
        /// <returns>The <see cref="Vector3"/> substracted by the length.</returns>
        public static Vector3d operator -(Vector3d left, double right)
        {
            return new Vector3d
            {
                X = left.X - right,
                Y = left.Y - right,
                Z = left.Z - right
            };
        }

        /// <summary>
        /// 	Multiplies a <see cref="Vector3"/> with v given length.
        /// </summary>
        /// <param name="right">The vector to be multiplied.</param>
        /// <param name="left">The length to multiply the <see cref="Vector3"/> with.</param>
        /// <returns>The with the length multiplied <see cref="Vector3"/>.</returns>
        public static Vector3d operator *(double left, Vector3d right)
        {
            return right * left;
        }

        /// <summary>
        /// 	Multiplies a <see cref="Vector3"/> with v given length.
        /// </summary>
        /// <param name="left">The vector to be multiplied.</param>
        /// <param name="right">The length to multiply the <see cref="Vector3"/> with.</param>
        /// <returns>The with the length multiplied <see cref="Vector3"/>.</returns>
        public static Vector3d operator *(Vector3d left, double right)
        {
            return new Vector3d
            {
                X = left.X * right,
                Y = left.Y * right,
                Z = left.Z * right
            };
        }

        /// <summary>
        /// 	Divides a <see cref="Vector3"/> by v given length.
        /// </summary>
        /// <param name="left">The <see cref="Vector3"/> to be divided.</param>
        /// <param name="right">The length to divide by.</param>
        /// <returns>The divided <see cref="Vector3"/>.</returns>
        public static Vector3d operator /(Vector3d left, double right)
        {
            double reciprocal = 1 / right; // This way it's faster.
            return new Vector3d
            {
                X = left.X * reciprocal,
                Y = left.Y * reciprocal,
                Z = left.Z * reciprocal
            };
        }

        /// <summary>
        /// Checks two <see cref="Vector3"/>s for equality.
        /// </summary>
        /// <param name="left">The first part of the equation.</param>
        /// <param name="right">The other <see cref="Vector3"/> to check.</param>
        /// <returns>Whether both <see cref="Vector3"/>s are equal.</returns>
        public static bool operator ==(Vector3d left, Vector3d right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks two <see cref="Vector3"/>s for inequality.
        /// </summary>
        /// <param name="left">The first part of the equation.</param>
        /// <param name="right">The other <see cref="Vector3"/> to check.</param>
        /// <returns>Whether both <see cref="Vector3"/>s are not equal.</returns>
        public static bool operator !=(Vector3d left, Vector3d right)
        {
            return !(left == right);
        }

        public static implicit operator Vector3d(Vector3 vector)
        {
            return new Vector3d(vector);
        }

        public static implicit operator Vector3d(OpenTK.Vector3d vector)
        {
            return new Vector3d(vector.X, vector.Y, vector.Z);
        }

        public static implicit operator OpenTK.Vector3d(Vector3d vector)
        {
            return new OpenTK.Vector3d(vector.X, vector.Y, vector.Z);
        }

        public static implicit operator Vector3d(OpenTK.Vector3 vector)
        {
            return new Vector3d(vector.X, vector.Y, vector.Z);
        }

        public static explicit operator OpenTK.Vector3(Vector3d vector)
        {
            return new OpenTK.Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }
    }
}