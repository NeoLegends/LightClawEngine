using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    /// <summary>
    /// Contains extensions to <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets all <see cref="Type"/>s in the <see cref="Assembly"/> that inherit from the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The <see cref="Type"/> the <see cref="Type"/>s have to inherit from to be returned. This can be an interface or a class.
        /// </typeparam>
        /// <param name="assembly">The <see cref="Assembly"/> to load the <see cref="Type"/>s from.</param>
        /// <param name="includeNonPublic">
        /// Indicates whether to include non-public (internal, private) <see cref="Type"/>s in the search as well.
        /// </param>
        /// <returns>All <see cref="Type"/>s that inherit from the specified <see cref="Type"/>.</returns>
        [Pure]
        public static IEnumerable<Type> GetTypesByBase<T>(this Assembly assembly, bool includeNonPublic)
            where T : class
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<Type>>(), type => typeof(T).IsAssignableFrom(type)));

            return GetTypesByBase(assembly, typeof(T), includeNonPublic);
        }

        /// <summary>
        /// Gets all <see cref="Type"/>s in the <see cref="Assembly"/> that inherit from the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="baseType">
        /// The <see cref="Type"/> the <see cref="Type"/>s have to inherit from to be returned. This can be an interface or a class.
        /// </param>
        /// <param name="assembly">The <see cref="Assembly"/> to load the <see cref="Type"/>s from.</param>
        /// <param name="includeNonPublic">
        /// Indicates whether to include non-public (internal, private) <see cref="Type"/>s in the search as well.
        /// </param>
        /// <returns>All <see cref="Type"/>s that inherit from the specified <see cref="Type"/>.</returns>
        [Pure]
        public static IEnumerable<Type> GetTypesByBase(this Assembly assembly, Type baseType, bool includeNonPublic)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            Contract.Requires<ArgumentNullException>(baseType != null);
            Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<Type>>(), type => baseType.IsAssignableFrom(type)));

            return from searchResult in (includeNonPublic ? assembly.GetTypes() : assembly.GetExportedTypes())
                   where (searchResult.IsClass &&
                               (baseType.IsClass && (baseType.IsAssignableFrom(searchResult))) ||
                               (baseType.IsInterface && searchResult.GetInterfaces().Contains(baseType))) ||
                           (searchResult.IsValueType &&
                               (baseType.IsInterface && searchResult.GetInterfaces().Contains(baseType)))
                   select searchResult;
        }

        /// <summary>
        /// Gets all <see cref="Type"/>s that are decorated with a specific <see cref="Type"/> of <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see cref="Attribute"/> to search for.</typeparam>
        /// <param name="includeNonPublic">
        /// Indicates whether to include non-public (internal, private) <see cref="Type"/>s in the search as well.
        /// </param>
        /// <param name="assembly">The <see cref="Assembly"/> to load the <see cref="Type"/>s from.</param>
        /// <returns>All <see cref="Type"/>s that are decorated with the specified <see cref="Type"/> of <see cref="Attribute"/>.</returns>
        [Pure]
        public static IEnumerable<Type> GetTypesByAttribute<T>(this Assembly assembly, bool includeNonPublic)
            where T : Attribute
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);
            
            return GetTypesByAttribute(assembly, typeof(T), includeNonPublic);
        }

        /// <summary>
        /// Gets all <see cref="Type"/>s that are decorated with a specific <see cref="Type"/> of <see cref="Attribute"/>.
        /// </summary>
        /// <param name="attributeType">The <see cref="Type"/> of <see cref="Attribute"/> to search for.</param>
        /// <param name="includeNonPublic">
        /// Indicates whether to include non-public (internal, private) <see cref="Type"/>s in the search as well.
        /// </param>
        /// <param name="assembly">The <see cref="Assembly"/> to load the <see cref="Type"/>s from.</param>
        /// <returns>All <see cref="Type"/>s that are decorated with the specified <see cref="Type"/> of <see cref="Attribute"/>.</returns>
        [Pure]
        public static IEnumerable<Type> GetTypesByAttribute(this Assembly assembly, Type attributeType, bool includeNonPublic)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            Contract.Requires<ArgumentNullException>(attributeType != null);
            Contract.Requires<ArgumentException>(typeof(Attribute).IsAssignableFrom(attributeType));
            Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);

            return from searchResult in (includeNonPublic ? assembly.GetTypes() : assembly.GetExportedTypes())
                   where searchResult.GetCustomAttributes().Any(attribute => attributeType.IsAssignableFrom(attribute.GetType()))
                   select searchResult;
        }
    }
}
