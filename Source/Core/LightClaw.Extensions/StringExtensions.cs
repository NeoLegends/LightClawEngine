using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    /// <summary>
    /// Contains extension methods to <see cref="String"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces the format element inside the <paramref name="formattableString"/> with the specified format arguments.
        /// </summary>
        /// <param name="formattableString">The <see cref="String"/> to format.</param>
        /// <param name="args">Format arguments.</param>
        /// <returns>The formatted <see cref="String"/>.</returns>
        /// <example>
        /// Console.WriteLine("Hello, my name is {0}.".FormatWith("Bert"));
        /// </example>
        [Pure]
        public static string FormatWith(this string formattableString, params object[] args)
        {
            Contract.Requires<ArgumentNullException>(formattableString != null);
            Contract.Requires<ArgumentNullException>(args != null);

            return string.Format(formattableString, args);
        }
    }
}
