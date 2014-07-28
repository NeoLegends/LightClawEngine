using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    public static class StringExtensions
    {
        [Pure]
        public static string FormatWith(this string formattableString, params object[] args)
        {
            Contract.Requires<ArgumentNullException>(formattableString != null);
            Contract.Requires<ArgumentNullException>(args != null);

            return string.Format(formattableString, args);
        }
    }
}
