using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a range inside a UBO.
    /// </summary>
    [DataContract, ProtoContract]
    public struct BufferRange : IEquatable<BufferRange>
    {
        /// <summary>
        /// The starting location (in bytes).
        /// </summary>
        [DataMember, ProtoMember(1)]
        public int Start { get; private set; }

        /// <summary>
        /// The length of the range (in bytes).
        /// </summary>
        [DataMember, ProtoMember(2)]
        public int Length { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="BufferRange"/>.
        /// </summary>
        /// <param name="start">The starting location (in bytes).</param>
        /// <param name="length">The length of the range (in bytes).</param>
        public BufferRange(int start, int length)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(start >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(length > 0);

            this.Length = length;
            this.Start = start;
        }

        /// <summary>
        /// Checks whether the <see cref="BufferRange"/> equals the specified object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is BufferRange) ? this.Equals((BufferRange)obj) : false;
        }

        /// <summary>
        /// Checks whether the current instance is equal to the specified <see cref="BufferRange"/>.
        /// </summary>
        /// <param name="other">The <see cref="BufferRange"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="BufferRange"/>s are equal, otherwise <c>false</c>.</returns>
        public bool Equals(BufferRange other)
        {
            return (this.Start == other.Start) && (this.Length == other.Length);
        }

        /// <summary>
        /// Gets the <see cref="BufferRange"/>'s hash code.
        /// </summary>
        /// <returns>The <see cref="BufferRange"/>'s hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Start, this.Length);
        }

        /// <summary>
        /// Checks whether two <see cref="BufferRange"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="BufferRange"/>s are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(BufferRange left, BufferRange right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="BufferRange"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="BufferRange"/>s are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(BufferRange left, BufferRange right)
        {
            return !(left == right);
        }
    }
}
