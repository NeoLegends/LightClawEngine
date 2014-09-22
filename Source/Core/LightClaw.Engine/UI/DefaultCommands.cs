using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.UI
{
    public static class DefaultCommands
    {
        public static string GetSettingsValue(string[] args)
        {
            Contract.Requires<ArgumentNullException>(args != null);
            Contract.Requires<ArgumentException>(args.Length >= 1);

            throw new NotImplementedException();
        }

        public static void SetSettingsValue(string[] args)
        {
            Contract.Requires<ArgumentNullException>(args != null);
            Contract.Requires<ArgumentException>(args.Length >= 2);

            throw new NotImplementedException();
        }
    }
}
