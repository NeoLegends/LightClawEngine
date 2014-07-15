using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a four dimensional mathematical quaternion.
    /// </summary>
    [DataContract, ProtoContract]
    public struct Quaternion : ICloneable, IEquatable<Quaternion>
    {
        /// <summary>
        /// The size of the <see cref="Quaternion"/> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Quaternion));

        /// <summary>
        /// A <see cref="Quaternion"/> with all of its components set to zero.
        /// </summary>
        public static readonly Quaternion Zero = new Quaternion();

        /// <summary>
        /// A <see cref="Quaternion"/> with all of its components set to one.
        /// </summary>
        public static readonly Quaternion One = new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// The identity <see cref="Quaternion"/> (0, 0, 0, 1).
        /// </summary>
        public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// The X component of the quaternion.
        /// </summary>
        [DataMember, ProtoMember(1)]
        public float X { get; private set; }

        /// <summary>
        /// The Y component of the quaternion.
        /// </summary>
        [DataMember, ProtoMember(2)]
        public float Y { get; private set; }

        /// <summary>
        /// The Z component of the quaternion.
        /// </summary>
        [DataMember, ProtoMember(3)]
        public float Z { get; private set; }

        /// <summary>
        /// The W component of the quaternion.
        /// </summary>
        [DataMember, ProtoMember(4)]
        public float W { get; private set; }

        /// <summary>
        /// Gets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        public float this[int index]
        {
            get
            {
                Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < 4);

                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new ArgumentOutOfRangeException("index", "Indices for Quaternion run from 0 to 3, inclusive.");
                }
            }
        }

        /// <summary>
        /// Gets the angle of the quaternion.
        /// </summary>
        /// <value>The quaternion's angle.</value>
        public float Angle
        {
            get
            {
                return (!MathF.IsAlmostZero((X * X) + (Y * Y) + (Z * Z))) ? 
                    (float)(2.0 * Math.Acos(MathF.Clamp(W, -1f, 1f))) : 
                    0.0f;
            }
        }

        /// <summary>
        /// Gets the axis components of the quaternion.
        /// </summary>
        /// <value>The axis components of the quaternion.</value>
        public Vector3 Axis
        {
            get
            {
                float length = (X * X) + (Y * Y) + (Z * Z);
                if (MathF.IsAlmostZero(length))
                    return Vector3.UnitX;

                float inv = 1.0f / (float)Math.Sqrt(length);
                return new Vector3(X * inv, Y * inv, Z * inv);
            }
        }

        /// <summary>
        /// Gets all <see cref="Quaternion"/>-components as array.
        /// </summary>
        public float[] Array
        {
            get
            {
                Contract.Ensures(Contract.Result<float[]>() != null);
                Contract.Ensures(Contract.Result<float[]>().Length == 4);

                return new[] { this.X, this.Y, this.Z, this.W };
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is equivalent to the identity quaternion.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is an identity quaternion; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdentity
        {
            get
            {
                return (this == Quaternion.Identity);
            }
        }

        /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public bool IsNormalized
        {
            get
            {
                return this.SquaredLength == 1.0f;
            }
        }

        /// <summary>
        /// Gets the <see cref="Quaternion"/>s length.
        /// </summary>
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(this.SquaredLength);
            }
        }

        /// <summary>
        /// Gets the <see cref="Quaternion"/>s squared length.
        /// </summary>
        public float SquaredLength
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z) + (this.W * this.W);
            }
        }

        /// <summary>
        /// Gets the X, Y and Z-components as <see cref="Vector3"/>.
        /// </summary>
        public Vector3 Xyz
        {
            get
            {
                return new Vector3(this.X, this.Y, this.Z);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Quaternion(float value) : this(value, value, value, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the components.</param>
        public Quaternion(Vector4 value) : this(value.X, value.Y, value.Z, value.W) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the X, Y, and Z components.</param>
        /// <param name="w">Initial value for the W component of the quaternion.</param>
        public Quaternion(Vector3 value, float w) : this(value.X, value.Y, value.Z, w) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the X and Y components.</param>
        /// <param name="z">Initial value for the Z component of the quaternion.</param>
        /// <param name="w">Initial value for the W component of the quaternion.</param>
        public Quaternion(Vector2 value, float z, float w) : this(value.X, value.Y, z, w) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y, Z, and W components of the quaternion. This must be an array with four elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
        public Quaternion(float[] values)
            : this(values[0], values[1], values[2], values[3])
        {
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentOutOfRangeException>(values.Length == 4);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="x">Initial value for the X component of the quaternion.</param>
        /// <param name="y">Initial value for the Y component of the quaternion.</param>
        /// <param name="z">Initial value for the Z component of the quaternion.</param>
        /// <param name="w">Initial value for the W component of the quaternion.</param>
        public Quaternion(float x, float y, float z, float w)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Creates a deep clone of the <see cref="Quaternion"/>.
        /// </summary>
        /// <returns>The clone.</returns>
        public object Clone()
        {
            return new Quaternion(this.X, this.Y, this.Z, this.W);
        }

        /// <summary>
        /// Conjugates the quaternion.
        /// </summary>
        public Quaternion Conjugate()
        {
            return Quaternion.Conjugate(ref this);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (ReferenceEquals(value, null))
                return false;

            return (value is Quaternion) ? this.Equals((Quaternion)value) : false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Quaternion"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Quaternion"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Quaternion"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Quaternion other)
        {
            return (this.X == other.X) && (this.Y == other.Y) && (this.Z == other.Z) && (this.W == other.W);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Constants.HashStart * Constants.HashFactor + this.X.GetHashCode();
                hash = hash * Constants.HashFactor + this.Y.GetHashCode();
                hash = hash * Constants.HashFactor + this.Z.GetHashCode();
                hash = hash * Constants.HashFactor + this.W.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        public Quaternion Invert()
        {
            return Quaternion.Invert(ref this);
        }

        /// <summary>
        /// Converts the quaternion into a unit quaternion.
        /// </summary>
        public Quaternion Normalize()
        {
            return Quaternion.Normalize(ref this);
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<{0}|{1}|{2}|{3}>", this.X, this.Y, this.Z, this.W);
        }

        /// <summary>
        /// Adds two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to add.</param>
        /// <param name="right">The second quaternion to add.</param>
        /// <param name="result">When the method completes, contains the sum of the two quaternions.</param>
        public static Quaternion Add(ref Quaternion left, ref Quaternion right)
        {
            return new Quaternion(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        /// <summary>
        /// Returns a <see cref="Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric 
        /// coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <param name="result">When the method completes, contains a new <see cref="Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</param>
        public static Quaternion Barycentric(ref Quaternion value1, ref Quaternion value2, ref Quaternion value3, float amount1, float amount2)
        {
            Quaternion start = Slerp(ref value1, ref value2, amount1 + amount2);
            Quaternion end = Slerp(ref value1, ref value3, amount1 + amount2);
            return Slerp(ref start, ref end, amount2 / (amount1 + amount2));
        }

        /// <summary>
        /// Conjugates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to conjugate.</param>
        /// <returns>The conjugated quaternion.</returns>
        public static Quaternion Conjugate(ref Quaternion value)
        {
            return new Quaternion(-value.X, -value.Y, -value.Z, value.W);
        }

        /// <summary>
        /// Calculates the dot product of two quaternions.
        /// </summary>
        /// <param name="left">First source quaternion.</param>
        /// <param name="right">Second source quaternion.</param>
        /// <returns>The dot product of the two quaternions.</returns>
        public static float Dot(ref Quaternion left, ref Quaternion right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
        }

        /// <summary>
        /// Exponentiates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to exponentiate.</param>
        /// <param name="result">When the method completes, contains the exponentiated quaternion.</param>
        public static Quaternion Exponential(ref Quaternion value)
        {
            float angle = (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
            float sin = (float)Math.Sin(angle);
            Quaternion result = Quaternion.Zero;

            if (!MathF.IsAlmostZero(sin))
            {
                float coeff = sin / angle;
                result.X = coeff * value.X;
                result.Y = coeff * value.Y;
                result.Z = coeff * value.Z;
            }
            else
            {
                result = value;
            }

            result.W = (float)Math.Cos(angle);
            return result;
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        /// <param name="value">The quaternion to conjugate and renormalize.</param>
        /// <param name="result">When the method completes, contains the conjugated and renormalized quaternion.</param>
        public static Quaternion Invert(ref Quaternion value)
        {
            float lengthSq = value.SquaredLength;
            if (!MathF.IsAlmostZero(lengthSq))
            {
                lengthSq = 1.0f / lengthSq;
                return new Quaternion(-value.X * lengthSq, -value.Y * lengthSq, -value.Z * lengthSq, value.W * lengthSq);
            }
            else
            {
                return Quaternion.Identity;
            }
        }

        /// <summary>
        /// Performs a linear interpolation between two quaternions.
        /// </summary>
        /// <param name="start">Start quaternion.</param>
        /// <param name="end">End quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the linear interpolation of the two quaternions.</param>
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// <code>start + (end - start) * amount</code>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static Quaternion Lerp(ref Quaternion start, ref Quaternion end, float amount)
        {
            float inverse = 1.0f - amount;

            Quaternion result = Quaternion.Zero;
            if (Quaternion.Dot(ref start, ref end) >= 0.0f)
            {
                result.X = (inverse * start.X) + (amount * end.X);
                result.Y = (inverse * start.Y) + (amount * end.Y);
                result.Z = (inverse * start.Z) + (amount * end.Z);
                result.W = (inverse * start.W) + (amount * end.W);
            }
            else
            {
                result.X = (inverse * start.X) - (amount * end.X);
                result.Y = (inverse * start.Y) - (amount * end.Y);
                result.Z = (inverse * start.Z) - (amount * end.Z);
                result.W = (inverse * start.W) - (amount * end.W);
            }

            return result.Normalize();
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified quaternion.
        /// </summary>
        /// <param name="value">The quaternion whose logarithm will be calculated.</param>
        /// <param name="result">When the method completes, contains the natural logarithm of the quaternion.</param>
        public static Quaternion Logarithm(ref Quaternion value)
        {
            Quaternion result = Quaternion.Zero;
            if (Math.Abs(value.W) < 1.0)
            {
                float angle = (float)Math.Acos(value.W);
                float sin = (float)Math.Sin(angle);

                if (!MathF.IsAlmostZero(sin))
                {
                    float coeff = angle / sin;
                    result.X = value.X * coeff;
                    result.Y = value.Y * coeff;
                    result.Z = value.Z * coeff;
                }
                else
                {
                    result = value;
                }
            }
            else
            {
                result = value;
            }
            return result;
        }

        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scale">The amount by which to scale the quaternion.</param>
        /// <param name="result">When the method completes, contains the scaled quaternion.</param>
        public static Quaternion Multiply(ref Quaternion value, float scale)
        {
            return new Quaternion(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
        }

        /// <summary>
        /// Multiplies a quaternion by another.
        /// </summary>
        /// <param name="left">The first quaternion to multiply.</param>
        /// <param name="right">The second quaternion to multiply.</param>
        /// <returns>The multiplied quaternion.</returns>
        public static Quaternion Multiply(ref Quaternion left, ref Quaternion right)
        {
            float lx = left.X;
            float ly = left.Y;
            float lz = left.Z;
            float lw = left.W;
            float rx = right.X;
            float ry = right.Y;
            float rz = right.Z;
            float rw = right.W;
            float a = (ly * rz - lz * ry);
            float b = (lz * rx - lx * rz);
            float c = (lx * ry - ly * rx);
            float d = (lx * rx + ly * ry + lz * rz);
            return new Quaternion(
                (lx * rw + rx * lw) + a,
                (ly * rw + ry * lw) + b,
                (lz * rw + rz * lw) + c,
                lw * rw - d
            );
        }

        /// <summary>
        /// Reverses the direction of a given quaternion.
        /// </summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>A quaternion facing in the opposite direction.</returns>
        public static Quaternion Negate(ref Quaternion value)
        {
            return new Quaternion(-value.X, -value.Y, -value.Z, -value.W);
        }

        /// <summary>
        /// Converts the quaternion into a unit quaternion.
        /// </summary>
        /// <param name="value">The quaternion to normalize.</param>
        /// <param name="result">When the method completes, contains the normalized quaternion.</param>
        public static Quaternion Normalize(ref Quaternion value)
        {
            float length = value.Length;
            if (!MathF.IsAlmostZero(length))
            {
                float inverse = 1.0f / length;
                return new Quaternion(value.X * inverse, value.Y * inverse, value.Z * inverse, value.W * inverse);
            }
            else
            {
                return Quaternion.Identity;
            }
        }

        /// <summary>
        /// Creates a quaternion given a rotation and an axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static Quaternion RotationAxis(ref Vector3 axis, float angle)
        {
            Vector3 normalized = axis.Normalize();

            float half = angle * 0.5f;
            float sin = (float)Math.Sin(half);
            float cos = (float)Math.Cos(half);

            return new Quaternion(
                normalized.X * sin,
                normalized.Y * sin,
                normalized.Z * sin,
                cos
            );
        }

        ///// <summary>
        ///// Creates a quaternion given a rotation matrix.
        ///// </summary>
        ///// <param name="matrix">The rotation matrix.</param>
        ///// <param name="result">When the method completes, contains the newly created quaternion.</param>
        //public static void RotationMatrix(ref Matrix matrix, out Quaternion result)
        //{
        //    float sqrt;
        //    float half;
        //    float scale = matrix.M11 + matrix.M22 + matrix.M33;

        //    if (scale > 0.0f)
        //    {
        //        sqrt = (float)Math.Sqrt(scale + 1.0f);
        //        result.W = sqrt * 0.5f;
        //        sqrt = 0.5f / sqrt;

        //        result.X = (matrix.M23 - matrix.M32) * sqrt;
        //        result.Y = (matrix.M31 - matrix.M13) * sqrt;
        //        result.Z = (matrix.M12 - matrix.M21) * sqrt;
        //    }
        //    else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
        //    {
        //        sqrt = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
        //        half = 0.5f / sqrt;

        //        result.X = 0.5f * sqrt;
        //        result.Y = (matrix.M12 + matrix.M21) * half;
        //        result.Z = (matrix.M13 + matrix.M31) * half;
        //        result.W = (matrix.M23 - matrix.M32) * half;
        //    }
        //    else if (matrix.M22 > matrix.M33)
        //    {
        //        sqrt = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
        //        half = 0.5f / sqrt;

        //        result.X = (matrix.M21 + matrix.M12) * half;
        //        result.Y = 0.5f * sqrt;
        //        result.Z = (matrix.M32 + matrix.M23) * half;
        //        result.W = (matrix.M31 - matrix.M13) * half;
        //    }
        //    else
        //    {
        //        sqrt = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
        //        half = 0.5f / sqrt;

        //        result.X = (matrix.M31 + matrix.M13) * half;
        //        result.Y = (matrix.M32 + matrix.M23) * half;
        //        result.Z = 0.5f * sqrt;
        //        result.W = (matrix.M12 - matrix.M21) * half;
        //    }
        //}

        ///// <summary>
        ///// Creates a quaternion given a rotation matrix.
        ///// </summary>
        ///// <param name="matrix">The rotation matrix.</param>
        ///// <param name="result">When the method completes, contains the newly created quaternion.</param>
        //public static void RotationMatrix(ref Matrix3x3 matrix, out Quaternion result)
        //{
        //    float sqrt;
        //    float half;
        //    float scale = matrix.M11 + matrix.M22 + matrix.M33;

        //    if (scale > 0.0f)
        //    {
        //        sqrt = (float)Math.Sqrt(scale + 1.0f);
        //        result.W = sqrt * 0.5f;
        //        sqrt = 0.5f / sqrt;

        //        result.X = (matrix.M23 - matrix.M32) * sqrt;
        //        result.Y = (matrix.M31 - matrix.M13) * sqrt;
        //        result.Z = (matrix.M12 - matrix.M21) * sqrt;
        //    }
        //    else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
        //    {
        //        sqrt = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
        //        half = 0.5f / sqrt;

        //        result.X = 0.5f * sqrt;
        //        result.Y = (matrix.M12 + matrix.M21) * half;
        //        result.Z = (matrix.M13 + matrix.M31) * half;
        //        result.W = (matrix.M23 - matrix.M32) * half;
        //    }
        //    else if (matrix.M22 > matrix.M33)
        //    {
        //        sqrt = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
        //        half = 0.5f / sqrt;

        //        result.X = (matrix.M21 + matrix.M12) * half;
        //        result.Y = 0.5f * sqrt;
        //        result.Z = (matrix.M32 + matrix.M23) * half;
        //        result.W = (matrix.M31 - matrix.M13) * half;
        //    }
        //    else
        //    {
        //        sqrt = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
        //        half = 0.5f / sqrt;

        //        result.X = (matrix.M31 + matrix.M13) * half;
        //        result.Y = (matrix.M32 + matrix.M23) * half;
        //        result.Z = 0.5f * sqrt;
        //        result.W = (matrix.M12 - matrix.M21) * half;
        //    }
        //}

        ///// <summary>
        ///// Creates a left-handed, look-at quaternion.
        ///// </summary>
        ///// <param name="eye">The position of the viewer's eye.</param>
        ///// <param name="target">The camera look-at target.</param>
        ///// <param name="up">The camera's up vector.</param>
        ///// <param name="result">When the method completes, contains the created look-at quaternion.</param>
        //public static void LookAtLH(ref Vector3 eye, ref Vector3 target, ref Vector3 up, out Quaternion result)
        //{
        //    Matrix3x3 matrix;
        //    Matrix3x3.LookAtLH(ref eye, ref target, ref up, out matrix);
        //    RotationMatrix(ref matrix, out result);
        //}

        ///// <summary>
        ///// Creates a left-handed, look-at quaternion.
        ///// </summary>
        ///// <param name="eye">The position of the viewer's eye.</param>
        ///// <param name="target">The camera look-at target.</param>
        ///// <param name="up">The camera's up vector.</param>
        ///// <returns>The created look-at quaternion.</returns>
        //public static Quaternion LookAtLH(Vector3 eye, Vector3 target, Vector3 up)
        //{
        //    Quaternion result;
        //    LookAtLH(ref eye, ref target, ref up, out result);
        //    return result;
        //}

        ///// <summary>
        ///// Creates a left-handed, look-at quaternion.
        ///// </summary>
        ///// <param name="forward">The camera's forward direction.</param>
        ///// <param name="up">The camera's up vector.</param>
        ///// <param name="result">When the method completes, contains the created look-at quaternion.</param>
        //public static void RotationLookAtLH(ref Vector3 forward, ref Vector3 up, out Quaternion result)
        //{
        //    Vector3 eye = Vector3.Zero;
        //    Quaternion.LookAtLH(ref eye, ref forward, ref up, out result);
        //}

        ///// <summary>
        ///// Creates a left-handed, look-at quaternion.
        ///// </summary>
        ///// <param name="forward">The camera's forward direction.</param>
        ///// <param name="up">The camera's up vector.</param>
        ///// <returns>The created look-at quaternion.</returns>
        //public static Quaternion RotationLookAtLH(Vector3 forward, Vector3 up)
        //{
        //    Quaternion result;
        //    RotationLookAtLH(ref forward, ref up, out result);
        //    return result;
        //}

        ///// <summary>
        ///// Creates a right-handed, look-at quaternion.
        ///// </summary>
        ///// <param name="eye">The position of the viewer's eye.</param>
        ///// <param name="target">The camera look-at target.</param>
        ///// <param name="up">The camera's up vector.</param>
        ///// <param name="result">When the method completes, contains the created look-at quaternion.</param>
        //public static void LookAtRH(ref Vector3 eye, ref Vector3 target, ref Vector3 up, out Quaternion result)
        //{
        //    Matrix3x3 matrix;
        //    Matrix3x3.LookAtRH(ref eye, ref target, ref up, out matrix);
        //    RotationMatrix(ref matrix, out result);
        //}

        ///// <summary>
        ///// Creates a right-handed, look-at quaternion.
        ///// </summary>
        ///// <param name="eye">The position of the viewer's eye.</param>
        ///// <param name="target">The camera look-at target.</param>
        ///// <param name="up">The camera's up vector.</param>
        ///// <returns>The created look-at quaternion.</returns>
        //public static Quaternion LookAtRH(Vector3 eye, Vector3 target, Vector3 up)
        //{
        //    Quaternion result;
        //    LookAtRH(ref eye, ref target, ref up, out result);
        //    return result;
        //}

        ///// <summary>
        ///// Creates a right-handed, look-at quaternion.
        ///// </summary>
        ///// <param name="forward">The camera's forward direction.</param>
        ///// <param name="up">The camera's up vector.</param>
        ///// <param name="result">When the method completes, contains the created look-at quaternion.</param>
        //public static void RotationLookAtRH(ref Vector3 forward, ref Vector3 up, out Quaternion result)
        //{
        //    Vector3 eye = Vector3.Zero;
        //    Quaternion.LookAtRH(ref eye, ref forward, ref up, out result);
        //}

        ///// <summary>
        ///// Creates a right-handed, look-at quaternion.
        ///// </summary>
        ///// <param name="forward">The camera's forward direction.</param>
        ///// <param name="up">The camera's up vector.</param>
        ///// <returns>The created look-at quaternion.</returns>
        //public static Quaternion RotationLookAtRH(Vector3 forward, Vector3 up)
        //{
        //    Quaternion result;
        //    RotationLookAtRH(ref forward, ref up, out result);
        //    return result;
        //}

        ///// <summary>
        ///// Creates a left-handed spherical billboard that rotates around a specified object position.
        ///// </summary>
        ///// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        ///// <param name="cameraPosition">The position of the camera.</param>
        ///// <param name="cameraUpVector">The up vector of the camera.</param>
        ///// <param name="cameraForwardVector">The forward vector of the camera.</param>
        ///// <param name="result">When the method completes, contains the created billboard quaternion.</param>
        //public static void BillboardLH(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, ref Vector3 cameraForwardVector, out Quaternion result)
        //{
        //    Matrix3x3 matrix;
        //    Matrix3x3.BillboardLH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out matrix);
        //    RotationMatrix(ref matrix, out result);
        //}

        ///// <summary>
        ///// Creates a left-handed spherical billboard that rotates around a specified object position.
        ///// </summary>
        ///// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        ///// <param name="cameraPosition">The position of the camera.</param>
        ///// <param name="cameraUpVector">The up vector of the camera.</param>
        ///// <param name="cameraForwardVector">The forward vector of the camera.</param>
        ///// <returns>The created billboard quaternion.</returns>
        //public static Quaternion BillboardLH(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
        //{
        //    Quaternion result;
        //    BillboardLH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out result);
        //    return result;
        //}

        ///// <summary>
        ///// Creates a right-handed spherical billboard that rotates around a specified object position.
        ///// </summary>
        ///// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        ///// <param name="cameraPosition">The position of the camera.</param>
        ///// <param name="cameraUpVector">The up vector of the camera.</param>
        ///// <param name="cameraForwardVector">The forward vector of the camera.</param>
        ///// <param name="result">When the method completes, contains the created billboard quaternion.</param>
        //public static void BillboardRH(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, ref Vector3 cameraForwardVector, out Quaternion result)
        //{
        //    Matrix3x3 matrix;
        //    Matrix3x3.BillboardRH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out matrix);
        //    RotationMatrix(ref matrix, out result);
        //}

        ///// <summary>
        ///// Creates a right-handed spherical billboard that rotates around a specified object position.
        ///// </summary>
        ///// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        ///// <param name="cameraPosition">The position of the camera.</param>
        ///// <param name="cameraUpVector">The up vector of the camera.</param>
        ///// <param name="cameraForwardVector">The forward vector of the camera.</param>
        ///// <returns>The created billboard quaternion.</returns>
        //public static Quaternion BillboardRH(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
        //{
        //    Quaternion result;
        //    BillboardRH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out result);
        //    return result;
        //}

        ///// <summary>
        ///// Creates a quaternion given a rotation matrix.
        ///// </summary>
        ///// <param name="matrix">The rotation matrix.</param>
        ///// <returns>The newly created quaternion.</returns>
        //public static Quaternion RotationMatrix(Matrix matrix)
        //{
        //    Quaternion result;
        //    RotationMatrix(ref matrix, out result);
        //    return result;
        //}

        /// <summary>
        /// Creates a quaternion given a yaw, pitch, and roll value.
        /// </summary>
        /// <param name="yaw">The yaw of rotation.</param>
        /// <param name="pitch">The pitch of rotation.</param>
        /// <param name="roll">The roll of rotation.</param>
        /// <returns>The newly created quaternion.</returns>
        public static Quaternion RotationYawPitchRoll(float yaw, float pitch, float roll)
        {
            float halfRoll = roll * 0.5f;
            float halfPitch = pitch * 0.5f;
            float halfYaw = yaw * 0.5f;

            float sinRoll = (float)Math.Sin(halfRoll);
            float cosRoll = (float)Math.Cos(halfRoll);
            float sinPitch = (float)Math.Sin(halfPitch);
            float cosPitch = (float)Math.Cos(halfPitch);
            float sinYaw = (float)Math.Sin(halfYaw);
            float cosYaw = (float)Math.Cos(halfYaw);

            return new Quaternion(
                (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll),
                (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll),
                (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll),
                (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll)
            );
        }

        /// <summary>
        /// Interpolates between two quaternions, using spherical linear interpolation.
        /// </summary>
        /// <param name="start">Start quaternion.</param>
        /// <param name="end">End quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the spherical linear interpolation of the two quaternions.</param>
        public static Quaternion Slerp(ref Quaternion start, ref Quaternion end, float amount)
        {
            float opposite;
            float inverse;
            float dot = Dot(ref start, ref end);

            if (Math.Abs(dot) > 1.0f - MathF.ZeroThresholds[7])
            {
                inverse = 1.0f - amount;
                opposite = amount * Math.Sign(dot);
            }
            else
            {
                float acos = (float)Math.Acos(Math.Abs(dot));
                float invSin = (float)(1.0 / Math.Sin(acos));

                inverse = (float)Math.Sin((1.0f - amount) * acos) * invSin;
                opposite = (float)Math.Sin(amount * acos) * invSin * Math.Sign(dot);
            }

            return new Quaternion(
                (inverse * start.X) + (opposite * end.X),
                (inverse * start.Y) + (opposite * end.Y),
                (inverse * start.Z) + (opposite * end.Z),
                (inverse * start.W) + (opposite * end.W)
            );
        }

        /// <summary>
        /// Interpolates between quaternions, using spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Third source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
        /// <param name="result">When the method completes, contains the spherical quadrangle interpolation of the quaternions.</param>
        public static Quaternion Squad(ref Quaternion value1, ref Quaternion value2, ref Quaternion value3, ref Quaternion value4, float amount)
        {
            Quaternion start = Slerp(ref value1, ref value4, amount);
            Quaternion end = Slerp(ref value2, ref value3, amount);
            return Slerp(ref start, ref end, 2.0f * amount * (1.0f - amount));
        }

        /// <summary>
        /// Sets up control points for spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Third source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <returns>An array of three quaternions that represent control points for spherical quadrangle interpolation.</returns>
        public static Quaternion[] SquadSetup(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4)
        {
            Quaternion q0 = (value1 + value2).SquaredLength < (value1 - value2).SquaredLength ? -value1 : value1;
            Quaternion q2 = (value2 + value3).SquaredLength < (value2 - value3).SquaredLength ? -value3 : value3;
            Quaternion q3 = (value3 + value4).SquaredLength < (value3 - value4).SquaredLength ? -value4 : value4;
            Quaternion q1 = value2;

            Quaternion q1Exp = Exponential(ref q1);
            Quaternion q2Exp = Exponential(ref q2);

            Quaternion q1Expq0 = q1Exp * q0;
            Quaternion q1Expq2 = q1Exp * q2;
            Quaternion q2Expq1 = q2Exp * q1;
            Quaternion q2Expq3 = q2Exp * q3;

            Quaternion calc1 = -0.25f * (Logarithm(ref q1Expq2) + Logarithm(ref q1Expq0));
            Quaternion calc2 = -0.25f * (Logarithm(ref q2Expq3) + Logarithm(ref q2Expq1));

            return new[] { q1 * Exponential(ref calc1), q2 * Exponential(ref calc2), q2 };
        }

        /// <summary>
        /// Subtracts two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to subtract.</param>
        /// <param name="right">The second quaternion to subtract.</param>
        /// <param name="result">When the method completes, contains the difference of the two quaternions.</param>
        public static Quaternion Subtract(ref Quaternion left, ref Quaternion right)
        {
            return new Quaternion(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        /// <summary>
        /// Adds two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to add.</param>
        /// <param name="right">The second quaternion to add.</param>
        /// <returns>The sum of the two quaternions.</returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            return Add(ref left, ref right);
        }

        /// <summary>
        /// Subtracts two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to subtract.</param>
        /// <param name="right">The second quaternion to subtract.</param>
        /// <returns>The difference of the two quaternions.</returns>
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            return Subtract(ref left, ref right);
        }

        /// <summary>
        /// Reverses the direction of a given quaternion.
        /// </summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>A quaternion facing in the opposite direction.</returns>
        public static Quaternion operator -(Quaternion value)
        {
            return Negate(ref value);
        }

        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scale">The amount by which to scale the quaternion.</param>
        /// <returns>The scaled quaternion.</returns>
        public static Quaternion operator *(float scale, Quaternion value)
        {
            return value * scale;
        }

        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scale">The amount by which to scale the quaternion.</param>
        /// <returns>The scaled quaternion.</returns>
        public static Quaternion operator *(Quaternion value, float scale)
        {
            return Multiply(ref value, scale);
        }

        /// <summary>
        /// Multiplies a quaternion by another.
        /// </summary>
        /// <param name="left">The first quaternion to multiply.</param>
        /// <param name="right">The second quaternion to multiply.</param>
        /// <returns>The multiplied quaternion.</returns>
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            return Multiply(ref left, ref right);
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !(left == right);
        }
    }
}
