using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Contains methods to compute the hash of multiple elements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation uses fixed parameter counts / "fixed overloads" to focus on speed which might decrease if
    /// parameter arrays are used instead.
    /// </para>
    /// <para>
    /// If you need to compute the hash code of more than six (= max amount per call) elements just nest the calls like so:
    /// <code>
    /// HashF.GetHashCode(
    ///     HashF.GetHashCode(this.M11, this.M12, this.M13, this.M14),
    ///     HashF.GetHashCode(this.M21, this.M22, this.M23, this.M24),
    ///     HashF.GetHashCode(this.M31, this.M32, this.M33, this.M34),
    ///     HashF.GetHashCode(this.M41, this.M42, this.M43, this.M44)
    /// );
    /// </code>
    /// Calls will be inlined, so there really is no performance penalty in calling the method multiple times.
    /// </para>
    /// <para>
    /// Hashes will be computed using the following method (see <see href="http://stackoverflow.com/a/263416"/>):
    /// <code>
    /// unchecked
    /// {
    ///     int hash = <see cref="HashStart"/> * <see cref="HashFactor"/> + GetHashCode(first);
    ///     hash = hash * <see cref="HashFactor"/> + GetHashCode(second);
    ///     ...
    ///     hash = hash * <see cref="HashFactor"/> + GetHashCode(nth);
    ///     return hash;
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public static class HashF
    {
        // AggressiveInlining everywhere to allow inlining even with value types. See http://blogs.msdn.com/b/davidnotario/archive/2004/11/01/250398.aspx.
        // Also, we really want to make sure the methods are inlined to make sure they are blazing fast as GetHashCode is supposed to be, especially for
        // larger structs (Matrix, for example).

        /// <summary>
        /// The factor a hash value will be multiplied before applying the next hash code.
        /// </summary>
        public const int HashFactor = 486187739;

        /// <summary>
        /// The starting value of a hash code.
        /// </summary>
        public const int HashStart = 397;

        /// <summary>
        /// Safely gets the hash code of the specified item.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the item.</typeparam>
        /// <param name="item">The item to get the hash code of.</param>
        /// <returns>The <paramref name="item"/>'s hash code or <c>0</c>, if <paramref name="item"/> was null.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<T>(T item)
        {
            return (item != null) ? item.GetHashCode() : 0;
        }

        /// <summary>
        /// Computes the hash code of two elements.
        /// </summary>
        /// <typeparam name="T1">The type of the first element.</typeparam>
        /// <typeparam name="T2">The type of the second element.</typeparam>
        /// <param name="first">The first element.</param>
        /// <param name="second">The second element.</param>
        /// <returns>The hash code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<T1, T2>(T1 first, T2 second)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + GetHashCode(first);
                hash = hash * HashFactor + GetHashCode(second);
                return hash;
            }
        }

        /// <summary>
        /// Computes the hash code of three elements.
        /// </summary>
        /// <typeparam name="T1">The type of the first element.</typeparam>
        /// <typeparam name="T2">The type of the second element.</typeparam>
        /// <typeparam name="T3">The type of the third element.</typeparam>
        /// <param name="first">The first element.</param>
        /// <param name="second">The second element.</param>
        /// <param name="third">The third element.</param>
        /// <returns>The hash code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + GetHashCode(first);
                hash = hash * HashFactor + GetHashCode(second);
                hash = hash * HashFactor + GetHashCode(third);
                return hash;
            }
        }

        /// <summary>
        /// Computes the hash code of four elements.
        /// </summary>
        /// <typeparam name="T1">The type of the first element.</typeparam>
        /// <typeparam name="T2">The type of the second element.</typeparam>
        /// <typeparam name="T3">The type of the third element.</typeparam>
        /// <typeparam name="T4">The type of the fourth element.</typeparam>
        /// <param name="first">The first element.</param>
        /// <param name="second">The second element.</param>
        /// <param name="third">The third element.</param>
        /// <param name="fourth">The fourth element.</param>
        /// <returns>The hash code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + GetHashCode(first);
                hash = hash * HashFactor + GetHashCode(second);
                hash = hash * HashFactor + GetHashCode(third);
                hash = hash * HashFactor + GetHashCode(fourth);
                return hash;
            }
        }

        /// <summary>
        /// Computes the hash code of five elements.
        /// </summary>
        /// <typeparam name="T1">The type of the first element.</typeparam>
        /// <typeparam name="T2">The type of the second element.</typeparam>
        /// <typeparam name="T3">The type of the third element.</typeparam>
        /// <typeparam name="T4">The type of the fourth element.</typeparam>
        /// <typeparam name="T5">The type of the fifth element.</typeparam>
        /// <param name="first">The first element.</param>
        /// <param name="second">The second element.</param>
        /// <param name="third">The third element.</param>
        /// <param name="fourth">The fourth element.</param>
        /// <param name="fifth">The fifth element.</param>
        /// <returns>The hash code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<T1, T2, T3, T4, T5>(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + GetHashCode(first);
                hash = hash * HashFactor + GetHashCode(second);
                hash = hash * HashFactor + GetHashCode(third);
                hash = hash * HashFactor + GetHashCode(fourth);
                hash = hash * HashFactor + GetHashCode(fifth);
                return hash;
            }
        }

        /// <summary>
        /// Computes the hash code of six elements.
        /// </summary>
        /// <typeparam name="T1">The type of the first element.</typeparam>
        /// <typeparam name="T2">The type of the second element.</typeparam>
        /// <typeparam name="T3">The type of the third element.</typeparam>
        /// <typeparam name="T4">The type of the fourth element.</typeparam>
        /// <typeparam name="T5">The type of the fifth element.</typeparam>
        /// <typeparam name="T5">The type of the sixth element.</typeparam>
        /// <param name="first">The first element.</param>
        /// <param name="second">The second element.</param>
        /// <param name="third">The third element.</param>
        /// <param name="fourth">The fourth element.</param>
        /// <param name="fifth">The fifth element.</param>
        /// <param name="fifth">The sixth element.</param>
        /// <returns>The hash code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<T1, T2, T3, T4, T5, T6>(T1 first, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + GetHashCode(first);
                hash = hash * HashFactor + GetHashCode(second);
                hash = hash * HashFactor + GetHashCode(third);
                hash = hash * HashFactor + GetHashCode(fourth);
                hash = hash * HashFactor + GetHashCode(fifth);
                hash = hash * HashFactor + GetHashCode(sixth);
                return hash;
            }
        }
    }
}
