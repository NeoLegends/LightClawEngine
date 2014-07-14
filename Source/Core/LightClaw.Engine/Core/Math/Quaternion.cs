using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a Quaternion.
    /// </summary>
    [DataContract, ProtoContract]
    [StructureInformation(4, 32, true)]
    public struct Quaternion : ICloneable, IEquatable<Quaternion>
    {
        /// <summary>
        /// Defines the identity quaternion.
        /// </summary>
        public static readonly Quaternion Identity = new Quaternion(0, 0, 0, 1);

        /// <summary>
        /// A <see cref="Quaternion"/> with all components set to 1.
        /// </summary>
        public static readonly Quaternion One = new Quaternion(1, 1, 1, 1);

        /// <summary>
        /// Gets a <see cref="Vector3"/> with the X, Y and Z components of the instance.
        /// </summary>
        [DataMember, ProtoMember(1)]
        public Vector3 Xyz { get; private set; }

        /// <summary>
        /// Gets the W component of the instance.
        /// </summary>
        [DataMember, ProtoMember(2)]
        public float W { get; private set; }

        /// <summary>
        /// Gets the X component of this instance.
        /// </summary>
        public float X 
        { 
            get 
            {
                return this.Xyz.X;
            } 
        }

        /// <summary>
        /// Gets the Y component of this instance.
        /// </summary>
        public float Y 
        { 
            get 
            { 
                return this.Xyz.Y;
            } 
        }

        /// <summary>
        /// Gets the Z component of this instance.
        /// </summary>
        public float Z 
        { 
            get 
            {
                return this.Xyz.Z;
            }
        }

        /// <summary>
        /// Gets the length (magnitude) of the <see cref="Quaternion"/>.
        /// </summary>
        /// <seealso cref="SquaredLength"/>
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(W * W + Xyz.SquaredLength);
            }
        }

        /// <summary>
        /// Gets the squared length (magnitude).
        /// </summary>
        /// <remarks>
        /// When comparing lengths, use this property instead of <see cref="Length"/> as you can
        /// save on the expensive <see cref="Math.Sqrt"/>-call.
        /// </remarks>
        public float SquaredLength
        {
            get
            {
                return (this.W * this.W) + Xyz.SquaredLength;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        /// <param name="w">The w component.</param>
        public Quaternion(float x, float y, float z, float w) : this(new Vector3(x, y, z), w) { }

        /// <summary>
        /// Initializes a new <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="v">The vector part.</param>
        /// <param name="w">The w part.</param>
        public Quaternion(Vector3 v, float w)
        {
            this.Xyz = v;
            this.W = w;
        }

        /// <summary>
        /// Creates a deep clone of the <see cref="Quaternion"/>.
        /// </summary>
        /// <returns>The cloned <see cref="Quaternion"/>.</returns>
        public object Clone()
        {
            return new Quaternion(this.Xyz, this.W);
        }

        /// <summary>
        /// Inverts the <see cref="Vector3"/> component of this <see cref="Quaternion"/>.
        /// </summary>
        public Quaternion Conjugate()
        {
            return Quaternion.Conjugate(ref this);
        }

        /// <summary>
        /// Compares this object instance to another object for equality. 
        /// </summary>
        /// <param name="obj">The other object to be used in the comparison.</param>
        /// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is Quaternion) ? this.Equals((Quaternion)obj) : false;
        }

        /// <summary>
        /// Compares this Quaternion instance to another Quaternion for equality. 
        /// </summary>
        /// <param name="other">The other Quaternion to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            return (this.Xyz == other.Xyz) && (this.W == other.W);
        }

        /// <summary>
        /// Provides the hash code for this object. 
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Constants.HashStart * Constants.HashFactor + this.Xyz.GetHashCode();
                hash = hash * Constants.HashFactor + this.W.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Reverses the rotation angle of this <see cref="Quaternion"/>.
        /// </summary>
        public Quaternion Invert()
        {
            return Quaternion.Invert(ref this);
        }

        /// <summary>
        /// Scales the <see cref="Quaternion"/> to unit length.
        /// </summary>
        public Quaternion Normalize()
        {
            return Quaternion.Normalize(ref this);
        }

        /// <summary>
        /// Convert this instance to an axis-angle representation.
        /// </summary>
        /// <returns>A Vector4 that is the axis-angle representation of this quaternion.</returns>
        public Vector4 ToAxisAngle()
        {
            Quaternion q = this;
            if (Math.Abs(q.W) > 1.0f)
            {
                q = q.Normalize();
            }

            float w = 2.0f * (float)System.Math.Acos(q.W);
            float den = (float)System.Math.Sqrt(1.0 - q.W * q.W);
            Vector3 xyz = (den > 0.0001f) ? q.Xyz / den : Vector3.UnitX;

            return new Vector4(xyz, w);
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current Quaternion.
        /// </summary>
        /// <returns>A <see cref="String"/> representing the <see cref="Quaternion"/>.</returns>
        public override string ToString()
        {
            return String.Format("({0}|{1})", this.Xyz, this.W);
        }

        /// <summary>
        /// Adds two quaternions.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns>The result of the addition.</returns>
        public static Quaternion Add(ref Quaternion left, ref Quaternion right)
        {
            return new Quaternion(left.Xyz + right.Xyz, left.W + right.W);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>The result of the operation.</returns>
        public static Quaternion Sub(ref Quaternion left, ref Quaternion right)
        {
            return  new Quaternion(left.Xyz - right.Xyz, left.W - right.W);
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(ref Quaternion left, ref Quaternion right)
        {
            Vector3 leftXyz = left.Xyz;
            Vector3 rightXyz = right.Xyz;
            return new Quaternion(
                right.W * left.Xyz + left.W * right.Xyz + Vector3.Cross(ref leftXyz, ref rightXyz),
                left.W * right.W - Vector3.Dot(ref leftXyz, ref rightXyz)
            );
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(ref Quaternion quaternion, float scale)
        {
            return new Quaternion(
                quaternion.X * scale, 
                quaternion.Y * scale, 
                quaternion.Z * scale, 
                quaternion.W * scale
            );
        }

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <returns>The conjugate of the given quaternion</returns>
        public static Quaternion Conjugate(ref Quaternion q)
        {
            return new Quaternion(-q.Xyz, q.W);
        }

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The inverse of the given quaternion</returns>
        public static Quaternion Invert(ref Quaternion q)
        {
            return new Quaternion(q.Xyz, -q.W);
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static Quaternion Normalize(ref Quaternion q)
        {
            float scale = 1.0f / q.Length;
            return new Quaternion(q.Xyz * scale, q.W * scale);
        }

        /// <summary>
        /// Build a quaternion from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns>The equivalent quaternion</returns>
        public static Quaternion FromAxisAngle(ref Vector3 axis, float angle)
        {
            if (axis.SquaredLength == 0.0f)
            {
                return Identity;
            }

            angle *= 0.5f;
            Quaternion q = new Quaternion(axis.Normalize() * (float)Math.Sin(angle), (float)Math.Cos(angle));
            return Quaternion.Normalize(ref q);
        }

        ///// <summary>
        ///// Builds a quaternion from the given rotation matrix
        ///// </summary>
        ///// <param name="matrix">A rotation matrix</param>
        ///// <returns>The equivalent quaternion</returns>
        //public static Quaternion FromMatrix(Matrix3 matrix)
        //{
        //    Quaternion result;
        //    FromMatrix(ref matrix, out result);
        //    return result;
        //}

        ///// <summary>
        ///// Builds a quaternion from the given rotation matrix
        ///// </summary>
        ///// <param name="matrix">A rotation matrix</param>
        ///// <param name="result">The equivalent quaternion</param>
        //public static void FromMatrix(ref Matrix3 matrix, out Quaternion result)
        //{
        //    float trace = matrix.Trace;

        //    if (trace > 0)
        //    {
        //        float s = (float)Math.Sqrt(trace + 1) * 2;
        //        float invS = 1f / s;

        //        result.w = s * 0.25f;
        //        result.xyz.X = (matrix.Row2.Y - matrix.Row1.Z) * invS;
        //        result.xyz.Y = (matrix.Row0.Z - matrix.Row2.X) * invS;
        //        result.xyz.Z = (matrix.Row1.X - matrix.Row0.Y) * invS;
        //    }
        //    else
        //    {
        //        float m00 = matrix.Row0.X, m11 = matrix.Row1.Y, m22 = matrix.Row2.Z;

        //        if (m00 > m11 && m00 > m22)
        //        {
        //            float s = (float)Math.Sqrt(1 + m00 - m11 - m22) * 2;
        //            float invS = 1f / s;

        //            result.w = (matrix.Row2.Y - matrix.Row1.Z) * invS;
        //            result.xyz.X = s * 0.25f;
        //            result.xyz.Y = (matrix.Row0.Y + matrix.Row1.X) * invS;
        //            result.xyz.Z = (matrix.Row0.Z + matrix.Row2.X) * invS;
        //        }
        //        else if (m11 > m22)
        //        {
        //            float s = (float)Math.Sqrt(1 + m11 - m00 - m22) * 2;
        //            float invS = 1f / s;

        //            result.w = (matrix.Row0.Z - matrix.Row2.X) * invS;
        //            result.xyz.X = (matrix.Row0.Y + matrix.Row1.X) * invS;
        //            result.xyz.Y = s * 0.25f;
        //            result.xyz.Z = (matrix.Row1.Z + matrix.Row2.Y) * invS;
        //        }
        //        else
        //        {
        //            float s = (float)Math.Sqrt(1 + m22 - m00 - m11) * 2;
        //            float invS = 1f / s;

        //            result.w = (matrix.Row1.X - matrix.Row0.Y) * invS;
        //            result.xyz.X = (matrix.Row0.Z + matrix.Row2.X) * invS;
        //            result.xyz.Y = (matrix.Row1.Z + matrix.Row2.Y) * invS;
        //            result.xyz.Z = s * 0.25f;
        //        }
        //    }
        //}

        /// <summary>
        /// Performs a spherical linear interpolation between two quaternions.
        /// </summary>
        /// <param name="q1">The first quaternion.</param>
        /// <param name="q2">The second quaternion.</param>
        /// <param name="blend">The blend factor.</param>
        /// <returns>A smooth blend between the given quaternions.</returns>
        public static Quaternion Slerp(ref Quaternion q1, ref Quaternion q2, float blend)
        {
            // if either input is zero, return the other.
            if (q1.SquaredLength == 0.0f)
            {
                if (q2.SquaredLength == 0.0f)
                {
                    return Identity;
                }
                return q2;
            }
            else if (q2.SquaredLength == 0.0f)
            {
                return q1;
            }

            Vector3 q1Xyz = q1.Xyz;
            Vector3 q2Xyz = q2.Xyz;
            float cosHalfAngle = q1.W * q2.W + Vector3.Dot(ref q1Xyz, ref q2Xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return q1;
            }
            else if (cosHalfAngle < 0.0f)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (float)Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            Quaternion result = new Quaternion(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            return (result.SquaredLength > 0.0f) ? Quaternion.Normalize(ref result) : Quaternion.Identity;
        }

        public static Vector4 ToAxisAngle(ref Quaternion q)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            left.Xyz += right.Xyz;
            left.W += right.W;
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            left.Xyz -= right.Xyz;
            left.W -= right.W;
            return left;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            return Multiply(ref left, ref right);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(Quaternion quaternion, float scale)
        {
            return scale * quaternion;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(float scale, Quaternion quaternion)
        {
            return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !left.Equals(right);
        }
    }
}
