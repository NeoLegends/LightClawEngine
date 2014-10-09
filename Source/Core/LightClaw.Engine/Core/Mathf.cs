using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Contains extensions to the math class.
    /// </summary>
    /// <remarks>This class is thread-safe.</remarks>
    [Pure]
    public static class MathF
    {
        /// <summary>
        /// Backing field.
        /// </summary>
        private static readonly double[] _ZeroThresholds = Enumerable.Range(0, 16).Select(i => Math.Pow(10, -i)).ToArray();

        /// <summary>
        /// Contains the thesholds used to determine whether a value is almost zero or not.
        /// </summary>
        public static double[] ZeroThresholds
        {
            get
            {
                Contract.Ensures(Contract.Result<double[]>() != null);
                Contract.Ensures(Contract.Result<double[]>().Length == 16);

                return _ZeroThresholds.ToArray();
            }
        }

        /// <summary>
        /// Gets the default zero threshold.
        /// </summary>
        public static double DefaultZeroThreshold
        {
            get
            {
                Contract.Assume(_ZeroThresholds.Length > 8);

                return _ZeroThresholds[8]; // We want the backing field here to avoid the ToArray
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private static readonly double _RootTwo = Math.Sqrt(2);

        /// <summary>
        /// The square root of two.
        /// </summary>
        public static double RootTwo
        {
            get
            {
                return _RootTwo;
            }
        }

        /// <summary>
        /// Checks whether two doubles are almost the same number.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the numbers are almost the same number, otherwise <c>false</c>.</returns>
        public static bool AlmostEquals(double left, double right)
        {
            return IsAlmostZero(left - right);
        }

        /// <summary>
        /// Makes sure the value stays in the given area.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value the value may have.</param>
        /// <param name="max">The maximum value the value may have.</param>
        /// <returns>The value cut off at the boundaries.</returns>
        public static int Clamp(int value, int min, int max)
        {
            return (value > max) ? max : (value < min) ? min : value;
        }

        /// <summary>
        /// Makes sure the value stays in the given area.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value the value may have.</param>
        /// <param name="max">The maximum value the value may have.</param>
        /// <returns>The value cut off at the boundaries.</returns>
        public static float Clamp(float value, float min, float max)
        {
            return (value > max) ? max : (value < min) ? min : value;
        }

        /// <summary>
        /// Makes sure the value stays in the given area.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value the value may have.</param>
        /// <param name="max">The maximum value the value may have.</param>
        /// <returns>The value cut off at the boundaries.</returns>
        public static double Clamp(double value, double min, double max)
        {
            return (value > max) ? max : (value < min) ? min : value;
        }

        /// <summary>
        /// Clamps the value to the <see cref="Int32"/>-range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The clamped value.</returns>
        [CLSCompliant(false)]
        public static int ClampToInt32(uint value)
        {
            return (value <= int.MaxValue) ? (int)value : int.MaxValue;
        }

        /// <summary>
        /// Clamps the value to the <see cref="Int32"/>-range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The clamped value.</returns>
        public static int ClampToInt32(long value)
        {
            return (value <= int.MaxValue) ? (int)value : int.MaxValue;
        }

        /// <summary>
        /// Clamps the value to the <see cref="Int32"/>-range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The clamped value.</returns>
        [CLSCompliant(false)]
        public static int ClampToInt32(ulong value)
        {
            return (value <= int.MaxValue) ? (int)value : int.MaxValue;
        }

        /// <summary>
        /// Clamps the value to the <see cref="Int642"/>-range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The clamped value.</returns>
        [CLSCompliant(false)]
        public static long ClampToInt64(ulong value)
        {
            return (value <= long.MaxValue) ? (long)value : long.MaxValue;
        }

        /// <summary>
        /// Converts the specified value in degrees to radians.
        /// </summary>
        /// <param name="value">The value to convert to radians.</param>
        /// <returns>The value in radians.</returns>
        public static float DegreesToRadians(float value)
        {
            return (float)(value * (Math.PI / 180));
        }

        /// <summary>
        /// Converts the specified value in degrees to radians.
        /// </summary>
        /// <param name="value">The value to convert to radians.</param>
        /// <returns>The value in radians.</returns>
        public static double DegreesToRadians(double value)
        {
            return value * (Math.PI / 180);
        }

        /// <summary>
        /// Gets the biggest common divisor of two numbers.
        /// </summary>
        /// <param name="a">The first number.</param>
        /// <param name="b">The second number.</param>
        /// <returns>The greatest common divisor of the two numbers.</returns>
        public static int GreatestCommonDivisor(int a, int b)
        {
            while (b > 0)
            {
                int rem = a % b;
                a = b;
                b = rem;
            }

            return a;
        }

        /// <summary>
        /// Gets the biggest common divisor of a range of numbers.
        /// </summary>
        /// <param name="values">The numbers.</param>
        /// <returns>The greatest common divisor of the two numbers.</returns>
        public static int GreatestCommonDivisor(IEnumerable<int> values)
        {
            return values.Aggregate((gcd, arg) => GreatestCommonDivisor(gcd, arg));
        }

        /// <summary>
        /// Checks whether the specified <paramref name="value"/> is almost one.
        /// </summary>
        /// <param name="value">The value to check for whether it is one.</param>
        /// <returns><c>true</c> if the value was one, otherwise <c>false</c>.</returns>
        /// <remarks>
        /// As floating point arithmetic is always prone to subtle errors, use this method instead of <c>value == 1</c>
        /// . As == checks for absolute equality it fails if there are small (usually negligible) inaccuracies involved.
        /// </remarks>
        public static bool IsAlmostOne(double value)
        {
            return IsAlmostZero(value - 1);
        }

        /// <summary>
        /// Checks whether the specified <paramref name="value"/> is almost one.
        /// </summary>
        /// <param name="value">The value to check for whether it is one.</param>
        /// <param name="decimalPlaceCount">The accuracy in decimal place counts.</param>
        /// <returns><c>true</c> if the value was one, otherwise <c>false</c>.</returns>
        /// <remarks>
        /// As floating point arithmetic is always prone to subtle errors, use this method instead of <c>value == 1</c>
        /// . As == checks for absolute equality it fails if there are small (usually negligible) inaccuracies involved.
        /// </remarks>
        public static bool IsAlmostOne(double value, int decimalPlaceCount)
        {
            Contract.Requires<ArgumentOutOfRangeException>(decimalPlaceCount >= 0 && decimalPlaceCount < 16);

            return IsAlmostZero(value - 1, decimalPlaceCount);
        }

        /// <summary>
        /// Checks whether a given value is almost zero using default accuracy.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>Whether the input number is almost zero or not.</returns>
        /// <remarks>
        /// As floating point arithmetic is always prone to subtle errors, use this method instead of <c>value == 0</c>
        /// . As == checks for absolute equality it fails if there are small (usually negligible) inaccuracies involved.
        /// </remarks>
        public static bool IsAlmostZero(double value)
        {
            return (-DefaultZeroThreshold < value) && (value < DefaultZeroThreshold);
        }

        /// <summary>
        /// Checks whether a given value is almost zero using the given accuracy.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="decimalPlaceCount">The amount of accuracy in decimal places.</param>
        /// <returns>Whether the input number is almost zero or not.</returns>
        /// <remarks>
        /// As floating point arithmetic is always prone to subtle errors, use this method instead of <c>value == 0</c>
        /// . As == checks for absolute equality it fails if there are small (usually negligible) inaccuracies involved.
        /// </remarks>
        public static bool IsAlmostZero(double value, int decimalPlaceCount)
        {
            Contract.Requires<ArgumentOutOfRangeException>(decimalPlaceCount >= 0 && decimalPlaceCount < ZeroThresholds.Length);

            return (-ZeroThresholds[decimalPlaceCount] < value) && (value < ZeroThresholds[decimalPlaceCount]);
        }

        /// <summary>
        /// Checks whether a number is a divisor of another number.
        /// </summary>
        /// <param name="n">The number to be divided.</param>
        /// <param name="divisor">The numbers divisor.</param>
        /// <returns>Whether n is dividable by the divisor.</returns>
        public static bool IsDivisorOf(int n, int divisor)
        {
            return (n % divisor == 0);
        }

        /// <summary>
        /// Checks whether a number is a divisor of another number.
        /// </summary>
        /// <param name="n">The number to be divided.</param>
        /// <param name="divisor">The numbers divisor.</param>
        /// <returns>Whether n is dividable by the divisor.</returns>
        public static bool IsDivisorOf(double n, double divisor)
        {
            return IsAlmostZero(n % divisor);
        }

        /// <summary>
        /// Checks whether the specified value is in the specified range.
        /// </summary>
        /// <param name="value">The value to check for.</param>
        /// <param name="min">The range's lower boundary.</param>
        /// <param name="max">The range's lower boundary.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="value"/> is greater than <paramref name="min"/> and smaller
        /// than <paramref name="max"/>.
        /// </returns>
        public static bool IsInRange(int value, int min, int max)
        {
            return (value > min) && (value < max);
        }

        /// <summary>
        /// Checks whether the specified value is in the specified range.
        /// </summary>
        /// <param name="value">The value to check for.</param>
        /// <param name="min">The range's lower boundary.</param>
        /// <param name="max">The range's lower boundary.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="value"/> is greater than <paramref name="min"/> and smaller
        /// than <paramref name="max"/>.
        /// </returns>
        public static bool IsInRange(double value, double min, double max)
        {
            return (value > min) && (value < max);
        }

        /// <summary>
        /// Checks whether the specified number is a prime number.
        /// </summary>
        /// <param name="number">The number to test.</param>
        /// <returns><c>true</c> if the specified number is prime, otherwise <c>false</c>.</returns>
        /// <remarks>
        /// This function is expensive (lots of divisions (worst case Sqrt( <paramref name="number"/> ) - 2), one
        /// Math.Sqrt). Call only if absolutely required.
        /// </remarks>
        public static bool IsPrime(int number)
        {
            if (number == 1)
            {
                return false;
            }
            if (number == 2)
            {
                return true;
            }

            int boundary = (int)Math.Floor(Math.Sqrt(number));
            for (int i = 2; i <= boundary; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether the specified number is a power of two.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if the specified number is a power of two, otherwise <c>false</c>.</returns>
        [CLSCompliant(false)]
        public static bool IsPowerOfTwo(uint value)
        {
            return (value != 0U) && (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Checks whether the specified number is a power of two.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if the specified number is a power of two, otherwise <c>false</c>.</returns>
        [CLSCompliant(false)]
        public static bool IsPowerOfTwo(ulong value)
        {
            return (value != 0UL) && (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Determines the least common multiple of two numbers.
        /// </summary>
        /// <param name="a">The first number.</param>
        /// <param name="b">The second number.</param>
        /// <returns>The least common multiple of the two numbers.</returns>
        public static int LeastCommonMultiple(int a, int b)
        {
            return (a * b) / GreatestCommonDivisor(a, b);
        }

        /// <summary>
        /// Determines the least common multiple of a collection of numbers.
        /// </summary>
        /// <param name="values">The numbers.</param>
        /// <returns>The least common multiple of all the numbers.</returns>
        public static int LeastCommonMultiple(IEnumerable<int> values)
        {
            return values.Aggregate((lcm, arg) => LeastCommonMultiple(lcm, arg));
        }

        /// <summary>
        /// Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <remarks>
        /// See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
        /// http: //fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/
        /// </remarks>
        /// <param name="from">Value to interpolate from.</param>
        /// <param name="to">Value to interpolate to.</param>
        /// <param name="amount">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        public static float Lerp(float from, float to, float amount)
        {
            return (1 - amount) * from + amount * to;
        }

        /// <summary>
        /// Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <remarks>
        /// See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
        /// http: //fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/
        /// </remarks>
        /// <param name="from">Value to interpolate from.</param>
        /// <param name="to">Value to interpolate to.</param>
        /// <param name="amount">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        public static double Lerp(double from, double to, double amount)
        {
            return (1 - amount) * from + amount * to;
        }

        /// <summary>
        /// Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <remarks>
        /// See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
        /// http: //fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/
        /// </remarks>
        /// <param name="from">Value to interpolate from.</param>
        /// <param name="to">Value to interpolate to.</param>
        /// <param name="amount">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        public static byte Lerp(byte from, byte to, float amount)
        {
            return (byte)Lerp((float)from, (float)to, amount);
        }

        /// <summary>
        /// Rounds the value up to the next power of two.
        /// </summary>
        /// <param name="x">The value to round up.</param>
        /// <returns>The value's next power of two.</returns>
        [CLSCompliant(false)]
        public static uint NextPowerOfTwo(uint x)
        {
            // See http://acius2.blogspot.de/2007/11/calculating-next-power-of-2.html

            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x++;

            return x;
        }

        /// <summary>
        /// Rounds the value up to the next power of two.
        /// </summary>
        /// <param name="x">The value to round up.</param>
        /// <returns>The value's next power of two.</returns>
        [CLSCompliant(false)]
        public static ulong NextPowerOfTwo(ulong x)
        {
            // See http://acius2.blogspot.de/2007/11/calculating-next-power-of-2.html

            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x |= x >> 32;
            x++;

            return x;
        }

        /// <summary>
        /// Gets the power of two of the given exponent.
        /// </summary>
        /// <param name="n">The exponent.</param>
        /// <returns>Two to the power of the exponent.</returns>
        public static double PowerOfTwo(double n)
        {
            return Math.Pow(2, n);
        }

        /// <summary>
        /// Converts the specified angle in radians into degrees.
        /// </summary>
        /// <param name="value">The value in radians to convert.</param>
        /// <returns>The input value in degrees.</returns>
        public static float RadiansToDegrees(float value)
        {
            return (float)(value * (180 / Math.PI));
        }

        /// <summary>
        /// Converts the specified angle in radians into degrees.
        /// </summary>
        /// <param name="value">The value in radians to convert.</param>
        /// <returns>The input value in degrees.</returns>
        public static double RadiansToDegrees(double value)
        {
            return value * (180 / Math.PI);
        }

        /// <summary>
        /// Rounds the specified <paramref name="number"/> up to a multiple of <paramref name="factor"/>.
        /// </summary>
        /// <param name="number">The number to round up.</param>
        /// <param name="factor">The factor to round up to a multiple of to.</param>
        /// <returns>The <paramref name="number"/> rounded up to a multiple of <paramref name="factor"/>.</returns>
        /// <example>
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             <code>RoundUp(1, 10) = 10</code>
        ///         </description> 
        ///     </item> 
        ///     <item>
        ///         <description>
        ///             <code>RoundUp(11, 10) = 20</code>
        ///         </description> 
        ///     </item> 
        /// </list>
        /// </example>
        public static int RoundToMultiple(int number, int factor)
        {
            Contract.Requires<ArgumentOutOfRangeException>(factor >= 0);

            return number + factor - 1 - (number - 1) % factor;
        }

        /// <summary>
        /// Rounds the specified <paramref name="number"/> up to a multiple of <paramref name="factor"/>.
        /// </summary>
        /// <param name="number">The number to round up.</param>
        /// <param name="factor">The factor to round up to a multiple of to.</param>
        /// <returns>The <paramref name="number"/> rounded up to a multiple of <paramref name="factor"/>.</returns>
        /// <example>
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             <code>RoundUp(1, 10) = 10</code>
        ///         </description> 
        ///     </item> 
        ///     <item>
        ///         <description>
        ///             <code>RoundUp(11, 10) = 20</code>
        ///         </description> 
        ///     </item> 
        /// </list>
        /// </example>
        public static long RoundToMultiple(long number, long factor)
        {
            Contract.Requires<ArgumentOutOfRangeException>(factor >= 0);

            return number + factor - 1 - (number - 1) % factor;
        }

        /// <summary>
        /// Performs smooth (cubic Hermite) interpolation between 0 and 1.
        /// </summary>
        /// <remarks>See https://en.wikipedia.org/wiki/Smoothstep</remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        [ContractVerification(false)]
        public static float SmoothStep(float amount)
        {
            return (amount <= 0.0f) ?
                0.0f :
                (amount >= 1.0f) ?
                    1.0f :
                    amount * amount * (3 - (2 * amount));
        }

        /// <summary>
        /// Performs smooth (cubic Hermite) interpolation between 0 and 1.
        /// </summary>
        /// <remarks>See https://en.wikipedia.org/wiki/Smoothstep</remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        [ContractVerification(false)]
        public static double SmoothStep(double amount)
        {
            return (amount <= 0.0) ?
                0.0 :
                (amount >= 1.0) ?
                    1.0 :
                    amount * amount * (3 - (2 * amount));
        }

        /// <summary>
        /// Performs a smooth(er) interpolation between 0 and 1 with 1st and 2nd order derivatives of zero at endpoints.
        /// </summary>
        /// <remarks>See https://en.wikipedia.org/wiki/Smoothstep</remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        [ContractVerification(false)]
        public static float SmootherStep(float amount)
        {
            return (amount <= 0.0f) ?
                0.0f :
                (amount >= 1.0f) ?
                    1.0f :
                    amount * amount * amount * (amount * ((amount * 6) - 15) + 10);
        }

        /// <summary>
        /// Performs a smooth(er) interpolation between 0 and 1 with 1st and 2nd order derivatives of zero at endpoints.
        /// </summary>
        /// <remarks>See https://en.wikipedia.org/wiki/Smoothstep</remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        [ContractVerification(false)]
        public static double SmootherStep(double amount)
        {
            return (amount <= 0.0) ?
                0.0 :
                (amount >= 1.0) ?
                    1.0 :
                    amount * amount * amount * (amount * ((amount * 6) - 15) + 10);
        }

        /// <summary>
        /// Class used to convert bytes into hex strings.
        /// </summary>
        /// <remarks>This class is thread-safe.</remarks>
        [Pure]
        public static class HexTable
        {
            /// <summary>
            /// Backing field.
            /// </summary>
            private static readonly string[] hexData = Enumerable.Range(0, 256).Select(i => i.ToString("X2")).ToArray();

            /// <summary>
            /// Gets the hexadecimal representation of all byte values.
            /// </summary>
            public static string[] GetHexData()
            {
                return hexData.ToArray();
            }

            /// <summary>
            /// Gets the specified byte as hexadecimal string.
            /// </summary>
            /// <param name="index">The byte to obtain as hex string.</param>
            /// <returns>The byte's representation as hex string.</returns>
            public static string GetHexData(byte index)
            {
                return hexData[index];
            }

            /// <summary>
            /// Contains Contract.Invariant definitions.
            /// </summary>
            [ContractInvariantMethod]
            private static void ObjectInvariant()
            {
                Contract.Invariant(hexData.Length > byte.MaxValue);
            }
        }
    }
}