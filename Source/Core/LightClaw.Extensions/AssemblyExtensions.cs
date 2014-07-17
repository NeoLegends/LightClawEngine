using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetTypesByBase<T>(this Assembly assembly, bool includeNonPublic)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);

            return GetTypesByBase(assembly, typeof(T), includeNonPublic);
        }

        public static IEnumerable<Type> GetTypesByBase(this Assembly assembly, Type baseType, bool includeNonPublic)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            Contract.Requires<ArgumentNullException>(baseType != null);

            return from searchResult in (includeNonPublic ? assembly.GetTypes() : assembly.GetExportedTypes())
                   where (searchResult.IsClass &&
                               (baseType.IsClass && (baseType.IsAssignableFrom(searchResult))) ||
                               (baseType.IsInterface && searchResult.GetInterfaces().Contains(baseType))) ||
                           (searchResult.IsValueType &&
                               (baseType.IsInterface && searchResult.GetInterfaces().Contains(baseType)))
                   select searchResult;
        }
    }
}
