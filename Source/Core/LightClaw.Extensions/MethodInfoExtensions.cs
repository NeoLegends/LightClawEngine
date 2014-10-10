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
    /// Contains extension methods to <see cref="MethodInfo"/>.
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Creates a delegate of the specified <see cref="Type"/> from the method.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of delegate to create.</typeparam>
        /// <param name="mInfo">The <see cref="MethodInfo"/> to create a delegate from.</param>
        /// <returns>The created delegate.</returns>
        [Pure]
        public static T CreateDelegate<T>(this MethodInfo mInfo)
        {
            Contract.Requires<ArgumentNullException>(mInfo != null);
            Contract.Requires<ArgumentException>(typeof(Delegate).IsAssignableFrom(typeof(T)));

            return (T)((object)mInfo.CreateDelegate(typeof(T)));
        }

        /// <summary>
        /// Creates a delegate of the specified <see cref="Type"/> from the method.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of delegate to create.</typeparam>
        /// <param name="mInfo">The <see cref="MethodInfo"/> to create a delegate from.</param>
        /// <param name="target">The object instance on which to execute the method, if it isn't static.</param>
        /// <returns>The created delegate.</returns>
        [Pure]
        public static T CreateDelegate<T>(this MethodInfo mInfo, object target)
        {
            Contract.Requires<ArgumentNullException>(mInfo != null);
            Contract.Requires<ArgumentException>(typeof(Delegate).IsAssignableFrom(typeof(T)));

            return (T)((object)mInfo.CreateDelegate(typeof(T), target));
        }
    }
}
