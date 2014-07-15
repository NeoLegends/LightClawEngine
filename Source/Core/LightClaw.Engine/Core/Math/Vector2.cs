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
    /// Represents a two-dimensional Vector with two components.
    /// </summary>
    /// <seealso cref="LightClaw.Engine.Core.Vector3"/>
    /// <seealso cref="LightClaw.Engine.Core.Vector4"/>
    [StructureInformation(2, 4, true)]
    [Serializable, DataContract, ProtoContract]
    public struct Vector2 : ICloneable, IEquatable<Vector2>, IComparable<Vector2>
#if SYSTEMDRAWING_INTEROP
                            , IEquatable<System.Drawing.PointF>
#endif
    {
        #region Predefined Vectors

        /// <summary>
        /// A <see cref="Vector2"/> with all components set to zero.
        /// </summary>
        public static readonly Vector2 Zero = new Vector2(0.0f, 0.0f);

        /// <summary>
        /// A <see cref="Vector2"/> with all components set to one.
        /// </summary>
        public static readonly Vector2 One = new Vector2(1.0f, 1.0f);

        /// <summary>
        /// A <see cref="Vector2"/> pointing upwards.
        /// </summary>
        public static readonly Vector2 Up = new Vector2(0f, 1.0f);

        /// <summary>
        /// A <see cref="Vector2"/> pointing downwards.
        /// </summary>
        public static readonly Vector2 Down = new Vector2(0.0f, -1.0f);

        /// <summary>
        /// A <see cref="Vector2"/> pointing right.
        /// </summary>
        public static readonly Vector2 Right = new Vector2(1.0f, 0.0f);

        /// <summary>
        /// A <see cref="Vector2"/> pointing left.
        /// </summary>
        public static readonly Vector2 Left = new Vector2(-1.0f, 0.0f);

        /// <summary>
        /// A <see cref="Vector2"/> pointing into the top right corner with a length of sqrt(2).
        /// </summary>
        public static readonly Vector2 TopRight = One;

        /// <summary>
        /// A <see cref="Vector2"/> pointing into the top left corner with a length of sqrt(2).
        /// </summary>
        public static readonly Vector2 TopLeft = new Vector2(-1.0f, 1.0f);

        /// <summary>
        /// A <see cref="Vector2"/> pointing into the bottom right corner with a length of sqrt(2).
        /// </summary>
        public static readonly Vector2 BottomRight = new Vector2(1.0f, -1.0f);

        /// <summary>
        /// A <see cref="Vector2"/> pointing into the bottom right corner with a length of sqrt(2).
        /// </summary>
        public static readonly Vector2 BottomLeft = new Vector2(-1.0f, -1.0f);

        /// <summary>
        /// The <see cref="Random"/> instance used to obtain the random vector.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// Returns a random <see cref="Vector2"/>.
        /// </summary>
        public static Vector2 Random
        {
            get
            {
                lock (random)
                {
                    return new Vector2(
                        (float)random.NextDouble() * 100,
                        (float)random.NextDouble() * 100
                    );
                }
            }
        }

        #endregion

        /// <summary>
        /// X length.
        /// </summary>
        [DataMember, ProtoMember(1)]
        public float X { get; private set; }

        /// <summary>
        /// Y length.
        /// </summary>
        [DataMember, ProtoMember(2)]
        public float Y { get; private set; }

        /// <summary>
        /// Both components contained in an array.
        /// </summary>
        public float[] Array
        {
            get
            {
                Contract.Ensures(Contract.Result<float[]>() != null);

                return new float[] { this.X, this.Y };
            }
        }

        /// <summary>
        /// The squared length of the <see cref="Vector2"/> (= c^2).
        /// </summary>
        public float SquaredLength
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y);
            }
        }

        /// <summary>
        /// The length of the <see cref="Vector2"/>. See remarks.
        /// </summary>
        /// <remarks>
        /// For comparison of vector lengths it's usually better to use <see cref="P:SquaredLength"/>, as squared lengths
        /// are enough for simple comparison. Thus, you can omit the expensive <see cref="Math.Sqrt"/>.
        /// </remarks>
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(this.SquaredLength);
            }
        }

        /// <summary>
        /// Returns a new vector with exchanged X and Y components.
        /// </summary>
        public Vector2 Yx
        {
            get
            {
                return new Vector2(this.Y, this.X);
            }
        }

        /// <summary>
        /// Enables accessing one of the components through an index.
        /// </summary>
        /// <param name="index">The index of the component to access.</param>
        /// <returns>The component at the given index.</returns>
        public float this[int index]
        {
            get
            {
                Contract.Requires<IndexOutOfRangeException>(index >= 0 && index < 2);

                switch (index)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
                    default:
                        throw new IndexOutOfRangeException("Index has to be greater than or equal to zero and smaller than two.");
                }
            }
        }

#if SYSTEMDRAWING_INTEROP

        /// <summary>
        /// Creates a new <see cref="Vector2"/> from the given <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to create a new <see cref="Vector2"/> from</param>
        public Vector2(System.Drawing.Color color) : this(color.R, color.G) { }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> from the given <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// <param name="point">The <see cref="System.Drawing.Point"/> to create a new <see cref="Vector2"/> from</param>
        public Vector2(System.Drawing.Point point) : this(point.X, point.Y) { }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> from the given <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// <param name="point">The <see cref="System.Drawing.Point"/> to create a new <see cref="Vector2"/> from</param>
        public Vector2(System.Drawing.PointF point) : this(point.X, point.Y) { }

#endif

        /// <summary>
        /// Creates a new <see cref="Vector2"/> from the given <see cref="LightClaw.Engine.Graphics.Color"/>.
        /// </summary>
        /// <param name="color">The <see cref="LightClaw.Engine.Graphics.Color"/> to create a new <see cref="Vector2"/> from.</param>
        public Vector2(Color color) : this(color.R, color.G) { }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> from the given <see cref="Vector2"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2"/> to create a new <see cref="Vector2"/> from.</param>
        public Vector2(Vector2d vector) : this((float)vector.X, (float)vector.Y) { }

        /// <summary>
        /// Initializes a new <see cref="Vector2"/> and sets the X and Y values.
        /// </summary>
        /// <param name="x">The X length of the <see cref="Vector2"/>.</param>
        /// <param name="y">The X length of the <see cref="Vector2"/>.</param>
        public Vector2(float x, float y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Returns a negated copy of the current <see cref="Vector2"/>.
        /// </summary>
        /// <returns>The negated <see cref="Vector2"/>.</returns>
        public Vector2 Negate()
        {
            return Vector2.Negate(this);
        }

        /// <summary>
        /// Returns a normalized copy of the <see cref="Vector2"/>.
        /// </summary>
        /// <returns>The normalized <see cref="Vector2"/>.</returns>
        public Vector2 Normalize()
        {
            return Vector2.Normalize(this);
        }

        /// <summary>
        /// Creates a new deep copy of this <see cref="Vector2"/>
        /// </summary>
        /// <returns>A new instance of the <see cref="Vector2"/> with the same values</returns>
        public object Clone()
        {
            Contract.Ensures(Contract.Result<object>() != null);

            return new Vector2(this.X, this.Y);
        }

        /// <summary>
        /// Compares the <see cref="Vector2"/> to the other by comparing the length.
        /// </summary>
        /// <param name="other">The <see cref="Vector2"/> to compare against.</param>
        /// <returns>An integer indicating their relative size to each other.</returns>
        public int CompareTo(Vector2 other)
        {
            return this.SquaredLength.CompareTo(other.SquaredLength);
        }

        /// <summary>
        /// Checks whether this <see cref="Vector2"/> equals the given <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to check against</param>
        /// <returns>Whether this <see cref="Vector2"/> equals the given <see cref="System.Object"/></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            else if (ReferenceEquals(obj, this))
                return true;

#if SYSTEMDRAWING_INTEROP
            if (obj is System.Drawing.PointF)
            {
                return this.Equals((System.Drawing.PointF)obj);
            }
#endif
            return (obj is Vector2) ? this.Equals((Vector2)obj) : false;
        }

        /// <summary>
        /// Checks two <see cref="Vector2"/>s for equality
        /// </summary>
        /// <param name="other">The other <see cref="Vector2"/></param>
        /// <returns>Whether both <see cref="Vector2"/>s are equal</returns>
        public bool Equals(Vector2 other)
        {
            return ((this.X == other.X) && (this.Y == other.Y));
        }

#if SYSTEMDRAWING_INTEROP

        /// <summary>
        /// Checks whether the current instance equals the specified <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// <param name="other">The <see cref="System.Drawing.PointF"/> to check.</param>
        /// <returns>Whether both instances are equal.</returns>
        public bool Equals(System.Drawing.PointF other)
        {
            return (this.X == other.X) && (this.Y == other.Y);
        }

#endif

        /// <summary>
        /// Derived from <see cref="System.Object"/>
        /// </summary>
        /// <returns>Derived from <see cref="System.Object"/></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Constants.HashStart * Constants.HashFactor + this.X.GetHashCode();
                hash = hash * Constants.HashFactor + this.Y.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.string"/> representing the <see cref="Vector3"/>
        /// </summary>
        /// <returns>A <see cref="System.string"/> representing the <see cref="Vector3"/></returns>
        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Format("({0}|{1})", this.X, this.Y);
        }

        /// <summary>
        /// Adds two <see cref="Vector2"/>s together.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="left">The second operand.</param>
        /// <returns>The result.</returns>
        public static Vector2 Add(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        /// Returns a <see cref="SharpDX.Vector2"/> containing the 2D Cartesian coordinates of a point specified in Barycentric 
        /// coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="SharpDX.Vector2"/> containing the 2D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="SharpDX.Vector2"/> containing the 2D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="SharpDX.Vector2"/> containing the 2D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        public static Vector2 Barycentric(ref Vector2 value1, Vector2 value2, Vector2 value3, float amount1, float amount2)
        {
            return new Vector2()
            {
                X = (value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X)),
                Y = (value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y))
            };
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="valueA">The first position in the interpolation.</param>
        /// <param name="valueB">The second position in the interpolation.</param>
        /// <param name="valueC">The third position in the interpolatio.</param>
        /// <param name="valueD">The fourth position in the interpolation.</param>
        /// <param name="amount">The weighting factor.</param>
        public static Vector2 CatmullRom(Vector2 valueA, Vector2 valueB, Vector2 valueC, Vector2 valueD, float amount)
        {
            float squared = amount * amount,
                  cubed = amount * squared;

            return new Vector2()
            {
                X = 0.5f * ((((2.0f * valueB.X) + ((-valueA.X + valueC.X) * amount)) +
                    (((((2.0f * valueA.X) - (5.0f * valueB.X)) + (4.0f * valueC.X)) - valueD.X) * squared)) +
                    ((((-valueA.X + (3.0f * valueB.X)) - (3.0f * valueC.X)) + valueD.X) * cubed)),

                Y = 0.5f * ((((2.0f * valueB.Y) + ((-valueA.Y + valueC.Y) * amount)) +
                    (((((2.0f * valueA.Y) - (5.0f * valueB.Y)) + (4.0f * valueC.Y)) - valueD.Y) * squared)) +
                    ((((-valueA.Y + (3.0f * valueB.Y)) - (3.0f * valueC.Y)) + valueD.Y) * cubed))
            };
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            return new Vector2()
            {
                X = MathF.Clamp(value.X, min.X, max.X),
                Y = MathF.Clamp(value.Y, min.Y, max.Y)
            };
        }

        /// <summary>
        /// Calculates the distance between two <see cref="Vector2"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="Vector2"/>.</param>
        /// <param name="right">The second <see cref="Vector2"/>.</param>
        /// <returns>The distance between the two <see cref="Vector2"/>s.</returns>
        public static float Distance(Vector2 left, Vector2 right)
        {
            return (float)Math.Sqrt(DistanceSquared(left, right));
        }

        /// <summary>
        /// Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        /// <remarks>Distance squared is the value before taking the square root. 
        /// Distance squared can often be used in place of distance if relative comparisons are being made. 
        /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
        /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
        /// involves two square roots, which are computationally expensive. However, using distance squared 
        /// provides the same information and avoids calculating two square roots.
        /// </remarks>
        public static float DistanceSquared(Vector2 value1, Vector2 value2)
        {
            float x = value1.X - value2.X;
            float y = value1.Y - value2.Y;

            return (x * x) + (y * y);
        }

        /// <summary>
        /// Divides the <see cref="Vector2"/> by the specified value.
        /// </summary>
        /// <param name="left">The <see cref="Vector2"/> to divide.</param>
        /// <param name="right">The divisor.</param>
        /// <returns>The result.</returns>
        public static Vector2 Divide(Vector2 left, float right)
        {
            return Multiply(left, 1 / right);
        }

        /// <summary>
        /// Calculates the dot product of two <see cref="Vector2"/>s.
        /// </summary>
        /// <param name="left">First source <see cref="Vector2"/>.</param>
        /// <param name="right">Second source <see cref="Vector2"/>.</param>
        /// <returns>The dot product of the two <see cref="Vector2"/>s.</returns>
        public static float Dot(Vector2 left, Vector2 right)
        {
            return (left.X * right.X) + (left.Y * right.Y);
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="valueA">First source position <see cref="Vector2"/>.</param>
        /// <param name="tangentA">First source tangent <see cref="Vector2"/>.</param>
        /// <param name="valueB">Second source position <see cref="Vector2"/>.</param>
        /// <param name="tangentB">Second source tangent <see cref="Vector2"/>.</param>
        /// <param name="amount">Weighting <see cref="Vector2"/>.</param>
        public static Vector2 Hermite(Vector2 valueA, Vector2 tangentA, Vector2 valueB, Vector2 tangentB, float amount)
        {
            float squared = amount * amount,
                  cubed = amount * squared,
                  part1 = ((2.0f * cubed) - (3.0f * squared)) + 1.0f,
                  part2 = (-2.0f * cubed) + (3.0f * squared),
                  part3 = (cubed - (2.0f * squared)) + amount,
                  part4 = cubed - squared;

            return new Vector2()
            {
                X = (((valueA.X * part1) + (valueB.X * part2)) + (tangentA.X * part3)) + (tangentB.X * part4),
                Y = (((valueA.Y * part1) + (valueB.Y * part2)) + (tangentA.Y * part3)) + (tangentB.Y * part4)
            };
        }

        /// <summary>
        /// Performs a linear interpolation between two <see cref="Vector2"/>s.
        /// </summary>
        /// <param name="start">Start <see cref="Vector2"/>.</param>
        /// <param name="end">End <see cref="Vector2"/>.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramname="end"/>.</param>
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// <code>start + (end - start) * amount</code>
        /// Passing <paramname="amount"/> a value of 0 will cause <paramname="start"/> to be returned; a value of 1 will cause <paramname="end"/> to be returned.
        /// </remarks>
        public static Vector2 Lerp(Vector2 start, Vector2 end, float amount)
        {
            return new Vector2()
            {
                X = MathF.Lerp(start.X, end.X, amount),
                Y = MathF.Lerp(start.Y, end.Y, amount)
            };
        }

        /// <summary>
        /// 	Constructs a new <see cref="Vector2"/> based on the biggest values in one of the two <see cref="Vector2"/>s.
        /// </summary>
        /// <param name="a">The first <see cref="Vector2"/>.</param>
        /// <param name="b">The second <see cref="Vector2"/>.</param>
        /// <returns>A new <see cref="Vector2"/> with the biggest value of both input values.</returns>
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2()
            {
                X = Math.Max(a.X, b.X),
                Y = Math.Max(a.Y, b.Y)
            };
        }

        /// <summary>
        /// 	Constructs a new <see cref="Vector2"/> based on the smallest values in one of the two <see cref="Vector2"/>s.
        /// </summary>
        /// <param name="a">The first <see cref="Vector2"/>.</param>
        /// <param name="b">The second <see cref="Vector2"/>.</param>
        /// <returns>A new <see cref="Vector2"/> with the smallest value of both input values.</returns>
        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2()
            {
                X = Math.Min(a.X, b.X),
                Y = Math.Min(a.Y, b.Y)
            };
        }

        /// <summary>
        /// Multiplies the <see cref="Vector2"/> with the specified factor.
        /// </summary>
        /// <param name="left">The <see cref="Vector2"/> to multiply.</param>
        /// <param name="right">The factor.</param>
        /// <returns>The multiplication result.</returns>
        public static Vector2 Multiply(Vector2 left, float right)
        {
            return new Vector2(left.X * right, left.Y * right);
        }

        /// <summary>
        /// Reverses the direction of the given <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value">The <see cref="Vector2"/> to negate.</param>
        /// <returns>The negated <see cref="Vector2"/>.</returns>
        public static Vector2 Negate(Vector2 value)
        {
            return new Vector2()
            {
                X = -value.X,
                Y = -value.Y
            };
        }

        /// <summary>
        /// Normalizes the specified <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value">The <see cref="Vector2"/> to normalize.</param>
        /// <returns>The normalized <see cref="Vector2"/>.</returns>
        public static Vector2 Normalize(Vector2 value)
        {
            float length = value.Length;
            if (length != 0.0f)
            {
                float inv = 1.0f / length;
                return new Vector2()
                {
                    X = value.X * inv,
                    Y = value.Y * inv
                };
            }
            else
            {
                return (Vector2)value.Clone();
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
        public static void Orthogonalize(Vector2[] destination, params Vector2[] source)
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
                Vector2 newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= destination[r] * (Vector2.Dot(destination[r], newvector) / Vector2.Dot(destination[r], destination[r]));
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
        public static void Orthonormalize(Vector2[] destination, params Vector2[] source)
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
                Vector2 newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= destination[r] * Vector2.Dot(destination[r], newvector);
                }

                destination[i] = newvector.Normalize();
            }
        }

        /// <summary>
        /// Returns the reflection of a <see cref="Vector2"/> off a surface that has the specified normal.
        /// </summary>
        /// <param name="vec">The source <see cref="Vector2"/>.</param>
        /// <param name="surfaceNormal">Normal of the surface.</param>
        /// <remarks>
        /// Reflect only gives the direction of a reflection off a surface, it does not determine 
        /// whether the original <see cref="Vector2"/> was close enough to the surface to hit it.
        /// </remarks>
        public static Vector2 Reflect(Vector2 vec, Vector2 surfaceNormal)
        {
            float dotProduct = Vector2.Dot(vec, surfaceNormal);

            return new Vector2(
                vec.X - ((2.0f * dotProduct) * surfaceNormal.X),
                vec.Y - ((2.0f * dotProduct) * surfaceNormal.Y)
            );
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        public static Vector2 SmoothStep(Vector2 start, Vector2 end, float amount)
        {
            amount = (float)MathF.SmoothStep(amount);
            return Lerp(start, end, amount);
        }

        /// <summary>
        /// Subtracts one <see cref="Vector2"/> from the other.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns>The result.</returns>
        public static Vector2 Subtract(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        /// 	Adds <see cref="Vector2"/> A to <see cref="Vector2"/> B.
        /// </summary>
        /// <param name="left">First <see cref="Vector2"/> to add.</param>
        /// <param name="right">Second <see cref="Vector2"/> to add.</param>
        /// <returns>The added vector.</returns>
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Negates the given <see cref="Vector2"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2"/> to negate.</param>
        /// <returns>The negated <see cref="Vector2"/>.</returns>
        public static Vector2 operator -(Vector2 vector)
        {
            return Negate(vector);
        }

        /// <summary>
        /// 	Subtracts one <see cref="Vector2"/> from another <see cref="Vector2"/>.
        /// </summary>
        /// <param name="left">The minuend.</param>
        /// <param name="right">The Substractor.</param>
        /// <returns>The substracted <see cref="Vector2"/>s.</returns>
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return Subtract(left, right);
        }

        /// <summary>
        /// 	Multiplies one <see cref="Vector2"/> with v given length.
        /// </summary>
        /// <param name="right">The vector to be multiplied.</param>
        /// <param name="left">The length to multiply the <see cref="Vector2"/> with.</param>
        /// <returns>The with the length multiplied <see cref="Vector2"/>.</returns>
        public static Vector2 operator *(float left, Vector2 right)
        {
            return right * left;
        }

        /// <summary>
        /// 	Multiplies one <see cref="Vector2"/> with v given length.
        /// </summary>
        /// <param name="left">The vector to be multiplied.</param>
        /// <param name="right">The length to multiply the <see cref="Vector2"/> with.</param>
        /// <returns>The with the length multiplied <see cref="Vector2"/>.</returns>
        public static Vector2 operator *(Vector2 left, float right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        /// 	Divides one <see cref="Vector2"/> by v given length.
        /// </summary>
        /// <param name="left">The <see cref="Vector2"/> to be divided.</param>
        /// <param name="right">The length to divide by.</param>
        /// <returns>The divided <see cref="Vector2"/>.</returns>
        public static Vector2 operator /(Vector2 left, float right)
        {
            return Divide(left, right);
        }

        /// <summary>
        /// Checks two <see cref="Vector2"/>s for equality.
        /// </summary>
        /// <param name="left">The first <see cref="Vector2"/>.</param>
        /// <param name="right">The second <see cref="Vector2"/>.</param>
        /// <returns>Whether the <see cref="Vector2"/>s are equal.</returns>
        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 	Checks whether the <see cref="Vector2"/>s are unequal.
        /// </summary>
        /// <param name="left">The first <see cref="Vector2"/>.</param>
        /// <param name="right">The second <see cref="Vector2"/>.</param>
        /// <returns>Whether the <see cref="Vector2"/>s are unequal.</returns>
        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implicitly converts the specified <see cref="Vector2"/> into a <see cref="OpenTK.Vector2"/>.
        /// </summary>
        /// <param name="color">The <see cref="Vector2"/> to convert.</param>
        /// <returns>The converted <see cref="OpenTK.Vector2"/>.</returns>
        public static explicit operator Vector2(Vector2d vector)
        {
            return new Vector2(vector);
        }

        /// <summary>
        /// Implicitly converts the specified <see cref="OpenTK.Vector2"/> into a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="color">The <see cref="OpenTK.Vector2"/> to convert.</param>
        /// <returns>The converted <see cref="Vector2"/>.</returns>
        public static implicit operator Vector2(OpenTK.Vector2 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        /// <summary>
        /// Implicitly converts the specified <see cref="Vector2"/> into a <see cref="OpenTK.Vector2"/>.
        /// </summary>
        /// <param name="color">The <see cref="Vector2"/> to convert.</param>
        /// <returns>The converted <see cref="OpenTK.Vector2"/>.</returns>
        public static implicit operator OpenTK.Vector2(Vector2 vector)
        {
            return new OpenTK.Vector2(vector.X, vector.Y);
        }

        /// <summary>
        /// Implicitly converts the specified <see cref="OpenTK.Vector2d"/> into a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="color">The <see cref="OpenTK.Vector2d"/> to convert.</param>
        /// <returns>The converted <see cref="Vector2"/>.</returns>
        public static explicit operator Vector2(OpenTK.Vector2d vector)
        {
            return new Vector2((float)vector.X, (float)vector.Y);
        }

        /// <summary>
        /// Implicitly converts the specified <see cref="Vector2"/> into a <see cref="OpenTK.Vector2d"/>.
        /// </summary>
        /// <param name="color">The <see cref="Vector2"/> to convert.</param>
        /// <returns>The converted <see cref="OpenTK.Vector2d"/>.</returns>
        public static implicit operator OpenTK.Vector2d(Vector2 vector)
        {
            return new OpenTK.Vector2d(vector.X, vector.Y);
        }

#if SYSTEMDRAWING_INTEROP

        /// <summary>
        /// Explicitly converts the given <see cref="System.Drawing.Point"/> into a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="point">The <see cref="System.Drawing.Point"/> to convert</param>
        /// <returns>The converted <see cref="Vector2"/></returns>
        public static implicit operator Vector2(System.Drawing.Point point)
        {
            return new Vector2(point);
        }

        /// <summary>
        /// Explicitly converts the given <see cref="Vector2"/> into a <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2"/> to convert</param>
        /// <returns>The converted <see cref="System.Drawing.Point"/></returns>
        public static implicit operator System.Drawing.Point(Vector2 vector)
        {
            return new System.Drawing.Point((int)vector.X, (int)vector.Y);
        }

        /// <summary>
        /// Explicitly converts the given <see cref="System.Drawing.PointF"/> into a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="point">The <see cref="System.Drawing.PointF"/> to convert</param>
        /// <returns>The converted <see cref="Vector2"/></returns>
        public static implicit operator Vector2(System.Drawing.PointF point)
        {
            return new Vector2(point);
        }

        /// <summary>
        /// Explicitly converts the given <see cref="Vector2"/> into a <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2"/> to convert</param>
        /// <returns>The converted <see cref="System.Drawing.PointF"/></returns>
        public static implicit operator System.Drawing.PointF(Vector2 vector)
        {
            return new System.Drawing.PointF(vector.X, vector.Y);
        }

#endif
    }
}