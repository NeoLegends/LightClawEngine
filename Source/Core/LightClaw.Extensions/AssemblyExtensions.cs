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
        [Pure]
        public static IEnumerable<Type> GetTypesByBase<T>(this Assembly assembly, bool includeNonPublic)
            where T : class
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            Contract.Ensures(Contract.Result<IEnumerable<Type>>().All(type => typeof(T).IsAssignableFrom(type)));

            return GetTypesByBase(assembly, typeof(T), includeNonPublic);
        }

        [Pure]
        public static IEnumerable<Type> GetTypesByBase(this Assembly assembly, Type baseType, bool includeNonPublic)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            Contract.Requires<ArgumentNullException>(baseType != null);
            Contract.Ensures(Contract.Result<IEnumerable<Type>>().All(type => baseType.IsAssignableFrom(type)));

            return from searchResult in (includeNonPublic ? assembly.GetTypes() : assembly.GetExportedTypes())
                   where (searchResult.IsClass &&
                               (baseType.IsClass && (baseType.IsAssignableFrom(searchResult))) ||
                               (baseType.IsInterface && searchResult.GetInterfaces().Contains(baseType))) ||
                           (searchResult.IsValueType &&
                               (baseType.IsInterface && searchResult.GetInterfaces().Contains(baseType)))
                   select searchResult;
        }

        [Pure]
        public static IEnumerable<Type> GetTypesByAttribute<T>(this Assembly assembly)
            where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            
            return GetTypesByAttribute(assembly, typeof(T));
        }

        [Pure]
        public static IEnumerable<Type> GetTypesByAttribute(this Assembly assembly, Type attributeType)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            Contract.Requires<ArgumentNullException>(attributeType != null);
            Contract.Requires<ArgumentException>(typeof(Attribute).IsAssignableFrom(attributeType));

            return from searchResult in assembly.GetTypes()
                   where searchResult.GetCustomAttributes().Any(attribute => attributeType.IsAssignableFrom(attribute.GetType()))
                   select searchResult;
        }
    }
}
