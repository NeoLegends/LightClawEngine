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
        public static string Format(this string formattableString, params object[] args)
        {
            Contract.Requires<ArgumentNullException>(formattableString != null);

            return string.Format(formattableString, args);
        }
    }
}
