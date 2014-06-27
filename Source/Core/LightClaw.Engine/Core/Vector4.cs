using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using LightClaw.Engine.Graphics;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// A four-dimensional mathematical vector.
    /// </summary>
    /// <seealso cref="LightClaw.Engine.Core.Vector2"/>
    /// <seealso cref="LightClaw.Engine.Core.Vector3"/>
    /// <seealso cref="SharpDX.Vector4"/>
    [Serializable, DataContract, ProtoContract]
    public struct Vector4 : ICloneable, IEquatable<Vector4>, IComparable<Vector4>
    {
        #region Predefined Vectors

        /// <summary>
        /// A <see cref="Vector4"/> with all components set to 0.
        /// </summary>
        public static readonly Vector4 Zero = new Vector4(0, 0, 0, 0);

        /// <summary>
        /// A <see cref="Vector4"/> with all components set to 1.
        /// </summary>
        public static readonly Vector4 One = new Vector4(1, 1, 1, 1);

        /// <summary>
        /// The X unit <see cref="Vector4"/> (1, 0, 0, 0).
        /// </summary>
        public static readonly Vector4 UnitX = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// The Y unit <see cref="Vector4"/> (0, 1, 0, 0).
        /// </summary>
        public static readonly Vector4 UnitY = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);

        /// <summary>
        /// The Z unit <see cref="Vector4"/> (0, 0, 1, 0).
        /// </summary>
        public static readonly Vector4 UnitZ = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);

        /// <summary>
        /// The W unit <see cref="Vector4"/> (0, 0, 0, 1).
        /// </summary>
        public static readonly Vector4 UnitW = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// The <see cref="Random"/> instance used to obtain the random vector.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// Returns a random <see cref="Vector4"/>.
        /// </summary>
        public static Vector4 Random
        {
            get
            {
                return new Vector4(
                    (float)random.NextDouble() * 100,
                    (float)random.NextDouble() * 100,
                    (float)random.NextDouble() * 100,
                    (float)random.NextDouble() * 100
                );
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
        /// Z length.
        /// </summary>
        [DataMember, ProtoMember(3)]
        public float Z { get; private set; }

        /// <summary>
        /// W length.
        /// </summary>
        [DataMember, ProtoMember(4)]
        public float W { get; private set; }

        /// <summary>
        /// The squared length of this <see cref="Vector4"/>.
        /// </summary>
        public float SquaredLength
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z) + (this.W * this.W);
            }
        }

        /// <summary>
        /// The <see cref="Vector4"/>s length.
        /// </summary>
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(this.SquaredLength);
            }
        }

        /// <summary>
        /// All three components contained in an array.
        /// </summary>
        public float[] Array
        {
            get
            {
                Contract.Ensures(Contract.Result<float[]>() != null);

                return new float[] { this.X, this.Y, this.Z, this.W };
            }
        }

        /// <summary>
        /// Enables accessing one of the components through an index.
        /// </summary>
        /// <param name="index">The index of the component to access.</param>
        /// <returns>The component at the given index.</returns>
        [ProtoIgnore, IgnoreDataMember]
        public float this[int index]
        {
            get
            {
                Contract.Requires<IndexOutOfRangeException>(index >= 0 && index < 4);
                
                switch (index)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
                    case 2:
                        return this.Z;
                    case 3:
                        return this.W;
                    default:
                        throw new IndexOutOfRangeException("Index has to be greater than or equal to zero and smaller than four.");
                }
            }
        }

#if SYSTEMDRAWING_INTEROP

        /// <summary>
        /// Creates a new <see cref="Vector4"/> from the given <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// <param name="point">The <see cref="System.Drawing.Point"/> to create a new <see cref="Vector4"/> from.</param>
        public Vector4(System.Drawing.Point point) : this(point.X, point.Y, 0.0f, 0.0f) { }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> from the given <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// <param name="point">The <see cref="System.Drawing.Point"/> to create a new <see cref="Vector4"/> from.</param>
        public Vector4(System.Drawing.PointF point) : this(point.X, point.Y, 0.0f, 0.0f) { }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> from the given <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to create a new <see cref="Vector4"/> from.</param>
        public Vector4(System.Drawing.Color color) : this(color.R, color.G, color.B, color.A) { }

#endif

        /// <summary>
        /// Creates a new <see cref="Vector4"/> from the given <see cref="Vector2"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2"/> to create this <see cref="Vector4"/> from.</param>
        public Vector4(Vector2 vector) : this(vector.X, vector.Y, 0.0f, 0.0f) { }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> from the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> to create a <see cref="Vector4"/> from.</param>
        public Vector4(Vector3 vector) : this(vector.X, vector.Y, vector.Z, 0.0f) { }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> from the given <see cref="Vector4"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector4"/> to create a <see cref="Vector4"/> from.</param>
        public Vector4(Vector4 vector) : this(vector.X, vector.Y, vector.Z, vector.W) { }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> from the given <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to create a <see cref="Vector4"/> from.</param>
        public Vector4(Color color) : this(color.R, color.G, color.B, color.A) { }

        /// <summary>
        /// Creates a new <see cref="Vector4"/> setting the X and Y and Z value.
        /// </summary>
        /// <param name="x">The <see cref="Vector4"/>'s X value.</param>
        /// <param name="y">The <see cref="Vector4"/>'s Y value.</param>
        /// <param name="z">The <see cref="Vector4"/>'s Z value.</param>
        /// <param name="w">The <see cref="Vector4"/>'s W value.</param>
        public Vector4(float x, float y, float z, float w)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Returns a negated copy of the current <see cref="Vector4"/>.
        /// </summary>
        /// <returns>The negated <see cref="Vector4"/>.</returns>
        public Vector4 Negate()
        {
            return Vector4.Negate(ref this);
        }

        /// <summary>
        /// Returns a normalized copy of the <see cref="Vector4"/>.
        /// </summary>
        /// <returns>The normalized <see cref="Vector4"/>.</returns>
        public Vector4 Normalize()
        {
            return Vector4.Normalize(this);
        }

        /// <summary>
        /// Creates a new deep copy of the <see cref="Vector4"/>.
        /// </summary>
        /// <returns>The cloned <see cref="Vector4"/>.</returns>
        public object Clone()
        {
            Contract.Ensures(Contract.Result<object>() != null);

            return new Vector4(this.X, this.Y, this.Z, this.W);
        }

        /// <summary>
        /// Compares the <see cref="Vector4"/> to the other by comparing the length.
        /// </summary>
        /// <param name="other">The <see cref="Vector4"/> to compare against.</param>
        /// <returns>An integer indicating their relative size to each other.</returns>
        public int CompareTo(Vector4 other)
        {
            return this.SquaredLength.CompareTo(other.SquaredLength);
        }

        /// <summary>
        /// Checks whether the <see cref="Vector4"/> equals the given <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/>to check against.</param>
        /// <returns>Whether both <see cref="Vector4"/>s are equal.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            else if (ReferenceEquals(obj, this))
                return true;

            return (obj is Vector4) ? this.Equals((Vector4)obj) : false;
        }

        /// <summary>
        /// Checks two <see cref="Vector4"/>s for equality.
        /// </summary>
        /// <param name="other">The other <see cref="Vector4"/> to check.</param>
        /// <returns>Whether both <see cref="Vector4"/>s are equal.</returns>
        public bool Equals(Vector4 other)
        {
            return ((this.X == other.X) && (this.Y == other.Y) && (this.Z == other.Z) && (this.W == other.W));
        }

        /// <summary>
        /// Returns the instance's hash code.
        /// </summary>
        /// <returns>The instance's hash code.</returns>
        public override int GetHashCode()
        {
            const int hashFactor = 486187739;
            unchecked
            {
                int hash = 397 * hashFactor + this.X.GetHashCode();
                hash = hash * hashFactor + this.Y.GetHashCode();
                hash = hash * hashFactor + this.Z.GetHashCode();
                hash = hash * hashFactor + this.W.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representing the <see cref="Vector4"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representing the <see cref="Vector4"/>.</returns>
        public override String ToString()
        {
            Contract.Ensures(Contract.Result<String>() != null);

            return String.Format("({0}|{1}|{2}|{3})", this.X, this.Y, this.Z, this.W);
        }

        /// <summary>
        /// 	Constructs a new <see cref="Vector4"/> based on the smallest values in one of the two <see cref="Vector4"/>s.
        /// </summary>
        /// <param name="a">The first <see cref="Vector4"/>.</param>
        /// <param name="b">The second <see cref="Vector4"/>.</param>
        /// <returns>A new <see cref="Vector4"/> with the smallest value of both input values.</returns>
        public static Vector4 Min(ref Vector4 a, ref Vector4 b)
        {
            return new Vector4()
            {
                X = (a.X < b.X) ? a.X : b.X,
                Y = (a.Y < b.Y) ? a.Y : b.Y,
                Z = (a.Z < b.Z) ? a.Z : b.Z,
                W = (a.W < b.W) ? a.W : b.W
            };
        }

        /// <summary>
        /// 	Constructs a new <see cref="Vector4"/> based on the biggest values in one of the two <see cref="Vector4"/>s.
        /// </summary>
        /// <param name="a">The first <see cref="Vector4"/>.</param>
        /// <param name="b">The second <see cref="Vector4"/>.</param>
        /// <returns>A new <see cref="Vector4"/> with the biggest value of both input values.</returns>
        public static Vector4 Max(ref Vector4 a, ref Vector4 b)
        {
            return new Vector4()
            {
                X = (a.X > b.X) ? a.X : b.X,
                Y = (a.Y > b.Y) ? a.Y : b.Y,
                Z = (a.Z > b.Z) ? a.Z : b.Z,
                W = (a.W > b.W) ? a.W : b.W
            };
        }

        /// <summary>
        /// Returns a negated copy of the given <see cref="Vector4"/>.
        /// </summary>
        /// <param name="vec">The <see cref="Vector4"/> to negate.</param>
        /// <returns>The negated <see cref="Vector4"/>.</returns>
        public static Vector4 Negate(ref Vector4 vec)
        {
            return new Vector4()
            {
                X = -vec.X,
                Y = -vec.Y,
                Z = -vec.Z,
                W = -vec.W
            };
        }

        /// <summary>
        /// Returns a <see cref="SharpDX.Vector4"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 4D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="SharpDX.Vector4"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="SharpDX.Vector4"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="SharpDX.Vector4"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        public static Vector4 Barycentric(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, float amount1, float amount2)
        {
            return new Vector4()
            {
                X = (value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X)),
                Y = (value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)),
                Z = (value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z)),
                W = (value1.W + (amount1 * (value2.W - value1.W))) + (amount2 * (value3.W - value1.W))
            };
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public static Vector4 Clamp(ref Vector4 value, ref Vector4 min, ref Vector4 max)
        {
            float x = value.X;
            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            float y = value.Y;
            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            float z = value.Z;
            z = (z > max.Z) ? max.Z : z;
            z = (z < min.Z) ? min.Z : z;

            float w = value.W;
            w = (w > max.W) ? max.W : w;
            w = (w < min.W) ? min.W : w;

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="M:DistanceSquared(Vector4, Vector4)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static float Distance(Vector4 value1, Vector4 value2)
        {
            float x = value1.X - value2.X;
            float y = value1.Y - value2.Y;
            float z = value1.Z - value2.Z;
            float w = value1.W - value2.W;

            return (float)Math.Sqrt(DistanceSquared(value1, value2));
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
        public static float DistanceSquared(Vector4 value1, Vector4 value2)
        {
            float x = value1.X - value2.X;
            float y = value1.Y - value2.Y;
            float z = value1.Z - value2.Z;
            float w = value1.W - value2.W;

            return (x * x) + (y * y) + (z * z) + (w * w);
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="left">First source vector.</param>
        /// <param name="right">Second source vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public static float Dot(Vector4 left, Vector4 right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
        }

        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        /// <param name="value">The vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        public static Vector4 Normalize(Vector4 value)
        {
            float length = value.Length;
            if (length != 0.0f)
            {
                float inv = 1.0f / length;
                return new Vector4()
                {
                    X = value.X * inv,
                    Y = value.Y * inv,
                    Z = value.Z * inv,
                    W = value.W * inv
                };
            }
            else
            {
                return (Vector4)value.Clone();
            }
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
        public static Vector4 Lerp(ref Vector4 start, ref Vector4 end, float amount)
        {
            return new Vector4()
            {
                X = start.X + ((end.X - start.X) * amount),
                Y = start.Y + ((end.Y - start.Y) * amount),
                Z = start.Z + ((end.Z - start.Z) * amount),
                W = start.W + ((end.W - start.W) * amount)
            };
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        public static Vector4 SmoothStep(ref Vector4 start, ref Vector4 end, float amount)
        {
            amount = Mathf.SmoothStep(amount);
            amount = (amount * amount) * (3.0f - (2.0f * amount));

            return new Vector4()
            {
                X = start.X + ((end.X - start.X) * amount),
                Y = start.Y + ((end.Y - start.Y) * amount),
                Z = start.Z + ((end.Z - start.Z) * amount),
                W = start.W + ((end.W - start.W) * amount)
            };
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position vector.</param>
        /// <param name="tangent1">First source tangent vector.</param>
        /// <param name="value2">Second source position vector.</param>
        /// <param name="tangent2">Second source tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector4 Hermite(ref Vector4 value1, ref Vector4 tangent1, ref Vector4 value2, ref Vector4 tangent2, float amount)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = ((2.0f * cubed) - (3.0f * squared)) + 1.0f;
            float part2 = (-2.0f * cubed) + (3.0f * squared);
            float part3 = (cubed - (2.0f * squared)) + amount;
            float part4 = cubed - squared;

            return new Vector4()
            {
                X = (((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4),
                Y = (((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4),
                Z = (((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4),
                W = (((value1.W * part1) + (value2.W * part2)) + (tangent1.W * part3)) + (tangent2.W * part4)
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
        public static Vector4 CatmullRom(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, ref Vector4 value4, float amount)
        {
            float squared = amount * amount;
            float cubed = amount * squared;

            return new Vector4()
            {
                X = 0.5f * ((((2.0f * value2.X) + ((-value1.X + value3.X) * amount)) + (((((2.0f * value1.X) - (5.0f * value2.X)) + (4.0f * value3.X)) - value4.X) * squared)) + ((((-value1.X + (3.0f * value2.X)) - (3.0f * value3.X)) + value4.X) * cubed)),
                Y = 0.5f * ((((2.0f * value2.Y) + ((-value1.Y + value3.Y) * amount)) + (((((2.0f * value1.Y) - (5.0f * value2.Y)) + (4.0f * value3.Y)) - value4.Y) * squared)) + ((((-value1.Y + (3.0f * value2.Y)) - (3.0f * value3.Y)) + value4.Y) * cubed)),
                Z = 0.5f * ((((2.0f * value2.Z) + ((-value1.Z + value3.Z) * amount)) + (((((2.0f * value1.Z) - (5.0f * value2.Z)) + (4.0f * value3.Z)) - value4.Z) * squared)) + ((((-value1.Z + (3.0f * value2.Z)) - (3.0f * value3.Z)) + value4.Z) * cubed)),
                W = 0.5f * ((((2.0f * value2.W) + ((-value1.W + value3.W) * amount)) + (((((2.0f * value1.W) - (5.0f * value2.W)) + (4.0f * value3.W)) - value4.W) * squared)) + ((((-value1.W + (3.0f * value2.W)) - (3.0f * value3.W)) + value4.W) * cubed))
            };
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
        public static void Orthogonalize(Vector4[] destination, params Vector4[] source)
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
                Vector4 newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= destination[r] * (Vector4.Dot(destination[r], newvector) / Vector4.Dot(destination[r], destination[r]));
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
        public static void Orthonormalize(Vector4[] destination, params Vector4[] source)
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
                Vector4 newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= destination[r] * Vector4.Dot(destination[r], newvector);
                }

                destination[i] = newvector.Normalize();
            }
        }

        /// <summary>
        /// 	Adds <see cref="Vector4"/> A to <see cref="Vector4"/> B.
        /// </summary>
        /// <param name="left">First <see cref="Vector4"/> to add.</param>
        /// <param name="right">Second <see cref="Vector4"/> to add.</param>
        /// <returns>The added vector.</returns>
        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            return new Vector4
            {
                X = left.X + right.X,
                Y = left.Y + right.Y,
                Z = left.Z + right.Z,
                W = left.W + right.W
            };
        }

        /// <summary>
        /// 	Adds v given length to X and Y of v <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">The value to add.</param>
        /// <param name="right">The length to add to the vector.</param>
        /// <returns>The vector with the added length.</returns>
        public static Vector4 operator +(Vector4 left, float right)
        {
            return new Vector4
            {
                X = left.X + right,
                Y = left.Y + right,
                Z = left.Z + right,
                W = left.W + right
            };
        }

        /// <summary>
        /// Negates the given <see cref="Vector4"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector4"/> to negate.</param>
        /// <returns>The negated <see cref="Vector4"/>.</returns>
        public static Vector4 operator -(Vector4 vector)
        {
            return Vector4.Negate(ref vector);
        }

        /// <summary>
        /// 	Substracts one <see cref="Vector4"/> from another <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">The minuend.</param>
        /// <param name="right">The Substractor.</param>
        /// <returns>The substracted <see cref="Vector4"/>s.</returns>
        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            return new Vector4
            {
                X = left.X - right.X,
                Y = left.Y - right.Y,
                Z = left.Z - right.Z,
                W = left.W - right.W
            };
        }

        /// <summary>
        /// 	Substracts v given length from v <see cref="Vector4"/>.
        /// </summary>
        /// <param name="left">The <see cref="Vector4"/> to substract from.</param>
        /// <param name="right">The length to substract from the Vector.</param>
        /// <returns>The <see cref="Vector4"/> substracted by the length.</returns>
        public static Vector4 operator -(Vector4 left, float right)
        {
            return new Vector4
            {
                X = left.X - right,
                Y = left.Y - right,
                Z = left.Z - right,
                W = left.W - right
            };
        }

        /// <summary>
        /// 	Multiplies a <see cref="Vector4"/> with v given length.
        /// </summary>
        /// <param name="right">The vector to be multiplied.</param>
        /// <param name="left">The length to multiply the <see cref="Vector4"/> with.</param>
        /// <returns>The with the length multiplied <see cref="Vector4"/>.</returns>
        public static Vector4 operator *(float left, Vector4 right)
        {
            return right * left;
        }

        /// <summary>
        /// 	Multiplies a <see cref="Vector4"/> with v given length.
        /// </summary>
        /// <param name="left">The vector to be multiplied.</param>
        /// <param name="right">The length to multiply the <see cref="Vector4"/> with.</param>
        /// <returns>The with the length multiplied <see cref="Vector4"/>.</returns>
        public static Vector4 operator *(Vector4 left, float right)
        {
            return new Vector4
            {
                X = left.X * right,
                Y = left.Y * right,
                Z = left.Z * right,
                W = left.W * right
            };
        }

        /// <summary>
        /// 	Divides a <see cref="Vector4"/> by v given length.
        /// </summary>
        /// <param name="left">The <see cref="Vector4"/> to be divided.</param>
        /// <param name="right">The length to divide by.</param>
        /// <returns>The divided <see cref="Vector4"/>.</returns>
        public static Vector4 operator /(Vector4 left, float right)
        {
            float reciprocal = 1 / right; // This way it's faster.
            return new Vector4
            {
                X = left.X * reciprocal,
                Y = left.Y * reciprocal,
                Z = left.Z * reciprocal,
                W = left.W * reciprocal
            };
        }

        /// <summary>
        /// Checks two <see cref="Vector3"/>s for equality.
        /// </summary>
        /// <param name="left">The first part of the equation.</param>
        /// <param name="right">The other <see cref="Vector3"/> to check.</param>
        /// <returns>Whether both <see cref="Vector4"/>s are equal.</returns>
        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks two <see cref="Vector3"/>s for inequality.
        /// </summary>
        /// <param name="left">The first part of the equation.</param>
        /// <param name="right">The other <see cref="Vector3"/> to check.</param>
        /// <returns>Whether both <see cref="Vector4"/>s are not equal.</returns>
        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Explicitly converts the given <see cref="Vector2"/> into a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2"/> to convert.</param>
        /// <returns>The converted <see cref="Vector4"/>.</returns>
        public static explicit operator Vector4(Vector2 vector)
        {
            return new Vector4(vector);
        }

        /// <summary>
        /// Explicitly converts the given <see cref="Vector3"/> into a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> to convert.</param>
        /// <returns>The converted <see cref="Vector4"/>.</returns>
        public static explicit operator Vector4(Vector3 vector)
        {
            return new Vector4(vector);
        }

        /// <summary>
        /// Explicitly converts the given <see cref="Color"/> into a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to convert.</param>
        /// <returns>The converted <see cref="Vector4"/>.</returns>
        public static explicit operator Vector4(Color color)
        {
            return new Vector4(color);
        }
    }
}