using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// A class containing functions that simplify working with <see cref="Random"/>.
    /// </summary>
    /// <remarks>This class is thread-safe.</remarks>
    public static class RandomF
    {
        /// <summary>
        /// The character used to build up the random string.
        /// </summary>
        private static readonly char[] characters = Enumerable.Range(33, 93).Select(i => (char)i).ToArray();

        /// <summary>
        /// The underlying instance of <see cref="Random"/>.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// Gets a random boolean.
        /// </summary>
        /// <returns>A random boolean.</returns>
        public static bool GetBoolean()
        {
            return GetInt32() >= 0;
        }

        /// <summary>
        /// Gets a random <see cref="Int32"/>.
        /// </summary>
        /// <returns>A random number.</returns>
        public static int GetInt32()
        {
            return GetInt32(int.MaxValue);
        }

        /// <summary>
        /// Gets a random <see cref="Int32"/> that is smaller than the specified <see cref="Value"/>.
        /// </summary>
        /// <param name="maxExclusive">The maximum value.</param>
        /// <returns>A random number smaller than <paramref name="maxExclusive"/>.</returns>
        public static int GetInt32(int maxExclusive)
        {
            Contract.Requires<ArgumentOutOfRangeException>(maxExclusive > int.MinValue);

            return GetInt32(int.MinValue, maxExclusive);
        }

        /// <summary>
        /// Gets a random number that is larger than <paramref name="minIncluse"/> and smaller than <paramref name="maxExclusive"/>.
        /// </summary>
        /// <param name="minInclusive">The lower boundary of the value to obtain.</param>
        /// <param name="maxExclusive">The upper boundary of the value to obtain.</param>
        /// <returns>A random number in the specified range.</returns>
        public static int GetInt32(int minInclusive, int maxExclusive)
        {
            Contract.Requires<ArgumentOutOfRangeException>(minInclusive < maxExclusive);

            lock (random)
            {
                return random.Next(minInclusive, maxExclusive);
            }
        }

        /// <summary>
        /// Gets an array of random numbers.
        /// </summary>
        /// <param name="minInclusive">The lower boundary of all numbers.</param>
        /// <param name="maxExclusive">The upper boundary of all numbers.</param>
        /// <param name="count">The amount of numbers to get.</param>
        /// <returns>An array of random numbers.</returns>
        public static int[] GetInt32s(int minInclusive, int maxExclusive, int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(minInclusive < maxExclusive);
            Contract.Requires<ArgumentOutOfRangeException>(count > 0);
            Contract.Ensures(Contract.Result<int[]>().Length == count);

            return Enumerable.Range(0, count).Select(i => GetInt32(minInclusive, maxExclusive)).ToArray();
        }

        /// <summary>
        /// Gets a single random byte.
        /// </summary>
        /// <returns>A random <see cref="Byte"/>.</returns>
        public static byte GetByte()
        {
            lock (random)
            {
                return (byte)random.Next(0, 256);
            }
        }

        /// <summary>
        /// Gets an array of random <see cref="Byte"/>s.
        /// </summary>
        /// <param name="count">The amount of <see cref="Byte"/>s to get.</param>
        /// <returns>The array of <see cref="Byte"/>s.</returns>
        public static byte[] GetBytes(int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(count > 0);
            Contract.Ensures(Contract.Result<byte[]>().Length == count);

            byte[] buffer = new byte[count];
            lock (random)
            {
                random.NextBytes(buffer);
            }
            return buffer;
        }

        /// <summary>
        /// Gets a <see cref="Single"/> in the range of 0.0 to 1.0.
        /// </summary>
        /// <returns>The random number.</returns>
        public static float GetSingle()
        {
            Contract.Ensures(Contract.Result<float>() >= 0.0f);

            lock (random)
            {
                return (float)random.NextDouble();
            }
        }

        /// <summary>
        /// Gets an array of <see cref="Single"/>s in the range of 0.0 to 1.0.
        /// </summary>
        /// <param name="count">The amount of <see cref="Single"/>s to get.</param>
        /// <returns>The array of random <see cref="Single"/>s.</returns>
        public static float[] GetSingles(int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(count > 0);
            Contract.Ensures(Contract.Result<float[]>().Length == count);
            Contract.Ensures(Contract.ForAll(Contract.Result<float[]>(), f => f >= 0.0f));

            lock (random)
            {
                return Enumerable.Range(0, count).Select(i => (float)random.NextDouble()).ToArray();
            }
        }

        /// <summary>
        /// Gets a <see cref="Double"/> in the range of 0.0 to 1.0.
        /// </summary>
        /// <returns>The random number.</returns>
        public static double GetDouble()
        {
            Contract.Ensures(Contract.Result<double>() >= 0.0f);

            lock (random)
            {
                return random.NextDouble();
            }
        }

        /// <summary>
        /// Gets an array of <see cref="Double"/>s in the range of 0.0 to 1.0.
        /// </summary>
        /// <param name="count">The amount of <see cref="Double"/>s to get.</param>
        /// <returns>The array of random <see cref="Double"/>s.</returns>
        public static double[] GetDoubles(int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(count > 0);
            Contract.Ensures(Contract.Result<double[]>().Length == count);
            Contract.Ensures(Contract.ForAll(Contract.Result<double[]>(), f => f >= 0.0));

            lock (random)
            {
                return Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
            }
        }

        /// <summary>
        /// Returns random elements from the specified <paramref name="array"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of elements to obtain.</typeparam>
        /// <param name="array">The source array.</param>
        /// <param name="count">The amount of elements to obtain.</param>
        /// <returns>An array of random elements from the input array.</returns>
        public static T[] GetRandomElements<T>(T[] array, int count)
        {
            Contract.Requires<ArgumentNullException>(array != null);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);

            lock (random)
            {
                return Enumerable.Range(0, count).Select(i => array[random.Next(0, array.Length)]).ToArray();
            }
        }

        /// <summary>
        /// Gets a random string.
        /// </summary>
        /// <param name="length">The desired length of the string.</param>
        /// <returns>The random string.</returns>
        public static string GetString(int length)
        {
            Contract.Requires<ArgumentOutOfRangeException>(length >= 0);

            return new string(GetRandomElements(characters, length));
        }
    }
}
