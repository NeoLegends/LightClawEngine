using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a <see cref="SamplerParameterName"/>-value association.
    /// </summary>
    [DataContract, ProtoContract]
    public struct SamplerParameterDescription : IEquatable<SamplerParameterDescription>
    {
        /// <summary>
        /// The sampler parameter to set.
        /// </summary>
        [DataMember, ProtoMember(1)]
        public SamplerParameterName ParameterName { get; private set; }

        /// <summary>
        /// The value.
        /// </summary>
        [DataMember, ProtoMember(2)]
        public float Value { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="SamplerParameterDescription"/>.
        /// </summary>
        /// <param name="parameterName">The sampler parameter to set.</param>
        /// <param name="value">The value.</param>
        public SamplerParameterDescription(SamplerParameterName parameterName, float value)
            : this()
        {
            this.ParameterName = parameterName;
            this.Value = value;
        }

        /// <summary>
        /// Checks whether the <see cref="SamplerParameterDescription"/> equals the specified object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is SamplerParameterDescription) ? this.Equals((SamplerParameterDescription)obj) : false;
        }

        /// <summary>
        /// Checks whether the current instance is equal to the specified <see cref="SamplerParameterDescription"/>.
        /// </summary>
        /// <param name="other">The <see cref="SamplerParameterDescription"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="SamplerParameterDescription"/>s are equal, otherwise <c>false</c>.</returns>
        public bool Equals(SamplerParameterDescription other)
        {
            return (this.ParameterName == other.ParameterName) && (this.Value == other.Value);
        }

        /// <summary>
        /// Gets the <see cref="SamplerParameterDescription"/>'s hash code.
        /// </summary>
        /// <returns>The <see cref="SamplerParameterDescription"/>'s hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.ParameterName, this.Value);
        }

        /// <summary>
        /// Checks whether two <see cref="SamplerParameterDescription"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="SamplerParameterDescription"/>s are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(SamplerParameterDescription left, SamplerParameterDescription right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="SamplerParameterDescription"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="SamplerParameterDescription"/>s are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(SamplerParameterDescription left, SamplerParameterDescription right)
        {
            return !(left == right);
        }
    }
}
