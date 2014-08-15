using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a buffer to binding index-association.
    /// </summary>
    public struct UniformBufferDescription
    {
        /// <summary>
        /// The buffer binding index.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The range of the <see cref="IBuffer"/> to bind to the index.
        /// </summary>
        public RangedBuffer RangedBuffer { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="UniformBufferDescription"/>.
        /// </summary>
        /// <param name="index">The buffer binding index.</param>
        /// <param name="rangedBuffer">The range of the <see cref="IBuffer"/> to bind to the index.</param>
        public UniformBufferDescription(int index, RangedBuffer rangedBuffer)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentNullException>(rangedBuffer != null);

            this.Index = index;
            this.RangedBuffer = rangedBuffer;
        }

        /// <summary>
        /// Checks whether the <see cref="UniformBufferDescription"/> equals the specified object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is UniformBufferDescription) ? this.Equals((UniformBufferDescription)obj) : false;
        }

        /// <summary>
        /// Checks whether the current instance is equal to the specified <see cref="UniformBufferDescription"/>.
        /// </summary>
        /// <param name="other">The <see cref="UniformBufferDescription"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="UniformBufferDescription"/>s are equal, otherwise <c>false</c>.</returns>
        public bool Equals(UniformBufferDescription other)
        {
            return (this.RangedBuffer == other.RangedBuffer) && (this.Index == other.Index);
        }

        /// <summary>
        /// Gets the <see cref="UniformBufferDescription"/>'s hash code.
        /// </summary>
        /// <returns>The <see cref="UniformBufferDescription"/>'s hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.RangedBuffer, this.Index);
        }

        /// <summary>
        /// Checks whether two <see cref="UniformBufferDescription"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="UniformBufferDescription"/>s are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(UniformBufferDescription left, UniformBufferDescription right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="UniformBufferDescription"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="UniformBufferDescription"/>s are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(UniformBufferDescription left, UniformBufferDescription right)
        {
            return !(left == right);
        }
    }
}
