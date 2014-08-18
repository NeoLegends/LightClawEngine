using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Contains methods to compute the hash of multiple elements.
    /// </summary>
    public static class HashF
    {
        /// <summary>
        /// The factor a hash value will be multiplied before applying the next hash code.
        /// </summary>
        public const int HashFactor = 486187739;

        /// <summary>
        /// The starting value of a hash code.
        /// </summary>
        public const int HashStart = 397;

        /// <summary>
        /// Computes the hash code of two elements.
        /// </summary>
        /// <typeparam name="T1">The type of the first element.</typeparam>
        /// <typeparam name="T2">The type of the second element.</typeparam>
        /// <param name="first">The first element.</param>
        /// <param name="second">The second element.</param>
        /// <returns>The hash code.</returns>
        public static int GetHashCode<T1, T2>(T1 first, T2 second)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + first.GetHashCode();
                hash = hash * HashFactor + second.GetHashCode();
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
        public static int GetHashCode<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + first.GetHashCode();
                hash = hash * HashFactor + second.GetHashCode();
                hash = hash * HashFactor + third.GetHashCode();
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
        public static int GetHashCode<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + first.GetHashCode();
                hash = hash * HashFactor + second.GetHashCode();
                hash = hash * HashFactor + third.GetHashCode();
                hash = hash * HashFactor + fourth.GetHashCode();
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
        public static int GetHashCode<T1, T2, T3, T4, T5>(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + first.GetHashCode();
                hash = hash * HashFactor + second.GetHashCode();
                hash = hash * HashFactor + third.GetHashCode();
                hash = hash * HashFactor + fourth.GetHashCode();
                hash = hash * HashFactor + fifth.GetHashCode();
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
        public static int GetHashCode<T1, T2, T3, T4, T5, T6>(T1 first, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth)
        {
            unchecked
            {
                int hash = HashStart * HashFactor + first.GetHashCode();
                hash = hash * HashFactor + second.GetHashCode();
                hash = hash * HashFactor + third.GetHashCode();
                hash = hash * HashFactor + fourth.GetHashCode();
                hash = hash * HashFactor + fifth.GetHashCode();
                hash = hash * HashFactor + sixth.GetHashCode();
                return hash;
            }
        }
    }
}
