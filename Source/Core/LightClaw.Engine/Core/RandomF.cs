using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public static class RandomF
    {
        private static readonly char[] characters = Enumerable.Range(33, 93).Select(i => (char)i).ToArray();

        private static readonly Random random = new Random();

        public static int GetInt32()
        {
            return GetInt32(int.MaxValue);
        }

        public static int GetInt32(int maxExclusive)
        {
            Contract.Requires<ArgumentOutOfRangeException>(maxExclusive > int.MinValue);

            return GetInt32(int.MinValue, maxExclusive);
        }

        public static int GetInt32(int minInclusive, int maxExclusive)
        {
            Contract.Requires<ArgumentOutOfRangeException>(minInclusive < maxExclusive);

            lock (random)
            {
                return random.Next(minInclusive, maxExclusive);
            }
        }

        public static int[] GetInt32s(int minInclusive, int maxExclusive, int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(minInclusive < maxExclusive);
            Contract.Requires<ArgumentOutOfRangeException>(count > 0);
            Contract.Ensures(Contract.Result<int[]>().Length == count);

            lock (random)
            {
                return Enumerable.Range(0, count).Select(i => random.Next(minInclusive, maxExclusive)).ToArray();
            }
        }

        public static byte GetByte()
        {
            lock (random)
            {
                return (byte)random.Next(0, 256);
            }
        }

        public static byte[] GetBytes(int count)
        {
            Contract.Requires<ArgumentOutOfRangeException>(count > 0);
            Contract.Ensures(Contract.Result<byte[]>().Length == count);

            lock (random)
            {
                byte[] buffer = new byte[count];
                random.NextBytes(buffer);
                return buffer;
            }
        }

        public static float GetSingle()
        {
            Contract.Ensures(Contract.Result<float>() >= 0.0f);

            lock (random)
            {
                return (float)random.NextDouble();
            }
        }

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

        public static double GetDouble()
        {
            Contract.Ensures(Contract.Result<double>() >= 0.0f);

            lock (random)
            {
                return random.NextDouble();
            }
        }

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

        public static T[] GetRandomElements<T>(T[] array, int count)
        {
            Contract.Requires<ArgumentNullException>(array != null);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);

            lock (random)
            {
                return Enumerable.Range(0, count).Select(i => array[random.Next(0, array.Length)]).ToArray();
            }
        }

        public static string GetString(int length)
        {
            Contract.Requires<ArgumentOutOfRangeException>(length >= 0);

            return new string(GetRandomElements(characters, length));
        }
    }
}
