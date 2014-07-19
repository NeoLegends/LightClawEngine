using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    public static class ContentManagerExtensions
    {
        public static void Register(this IContentManager manager, IEnumerable<IContentReader> readers)
        {
            Contract.Requires<ArgumentNullException>(manager != null);
            Contract.Requires<ArgumentNullException>(readers != null);

            foreach (IContentReader reader in readers)
            {
                manager.Register(reader);
            }
        }

        public static void Register(this IContentManager manager, IEnumerable<IContentResolver> resolvers)
        {
            Contract.Requires<ArgumentNullException>(manager != null);
            Contract.Requires<ArgumentNullException>(resolvers != null);

            foreach (IContentResolver resolver in resolvers)
            {
                manager.Register(resolver);
            }
        }
    }
}
