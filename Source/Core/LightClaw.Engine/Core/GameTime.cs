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
    /// <summary>
    /// Represents the game's time in fixed- or variable-step game loops.
    /// </summary>
    [DataContract, ProtoContract]
    public struct GameTime : ICloneable, IEquatable<GameTime>
    {
        /// <summary>
        /// Gets a <see cref="GameTime"/> with time zero.
        /// </summary>
        public static GameTime Null
        {
            get
            {
                return new GameTime();
            }
        }

        /// <summary>
        /// The time in seconds that passed since the last call to <see cref="IUpdateable.Update"/>.
        /// </summary>
        [DataMember, ProtoMember(1)]
        public double ElapsedSinceLastUpdate { get; private set; }

        /// <summary>
        /// The game's total running time in seconds.
        /// </summary>
        [DataMember, ProtoMember(2)]
        public double TotalGameTime { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="GameTime"/> setting the delta time and the total game time.
        /// </summary>
        /// <param name="elapsedSinceUpdate">The time in seconds that passed since the last call to <see cref="IUpdateable.Update"/>.</param>
        /// <param name="totalTime">The game's total running time in seconds.</param>
        public GameTime(double elapsedSinceUpdate, double totalTime)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(elapsedSinceUpdate >= 0.0);
            Contract.Requires<ArgumentOutOfRangeException>(totalTime >= 0.0);

            this.ElapsedSinceLastUpdate = elapsedSinceUpdate;
            this.TotalGameTime = totalTime;
        }

        /// <summary>
        /// Clones the <see cref="GameTime"/> creating a deep copy.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new GameTime(this.ElapsedSinceLastUpdate, this.TotalGameTime);
        }

        /// <summary>
        /// Tests whether the current instance and the specified object are the same.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to test against.</param>
        /// <returns><c>true</c> if both instances are the same, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is GameTime) ? this.Equals((GameTime)obj) : false;
        }

        /// <summary>
        /// Checks whether the two <see cref="GameTime"/>s are equal.
        /// </summary>
        /// <param name="other">The <see cref="GameTime"/> to test against.</param>
        /// <returns><c>true</c> if both instances are the same, otherwise <c>false</c>.</returns>
        public bool Equals(GameTime other)
        {
            return MathF.AlmostEquals(this.ElapsedSinceLastUpdate, other.ElapsedSinceLastUpdate) &&
                   MathF.AlmostEquals(this.TotalGameTime, other.TotalGameTime);
        }

        /// <summary>
        /// Obtains the <see cref="GameTime"/>'s hash code.
        /// </summary>
        /// <returns>The <see cref="GameTime"/>'s hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.ElapsedSinceLastUpdate, this.TotalGameTime);
        }

        /// <summary>
        /// Checks whether two <see cref="GameTime"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if both instances are the same, otherwise <c>false</c>.</returns>
        public static bool operator ==(GameTime left, GameTime right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="GameTime"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if both instances are the same, otherwise <c>false</c>.</returns>
        public static bool operator !=(GameTime left, GameTime right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Contains Contract.Invariant-definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.ElapsedSinceLastUpdate >= 0.0);
            Contract.Invariant(this.TotalGameTime >= 0.0);
        }
    }
}
