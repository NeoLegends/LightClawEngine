using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Marks a <see cref="Component"/> as non-removable from its <see cref="GameObject"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal sealed class NonRemovableAttribute : RemovalValidatorAttribute
    {
        /// <summary>
        /// Initializes a new <see cref="NonRemovableAttribute"/>.
        /// </summary>
        public NonRemovableAttribute() { }

        /// <summary>
        /// Initializes a new <see cref="NonRemovableAttribute"/> and sets the reason.
        /// </summary>
        /// <param name="reason">The reason why the <see cref="Component"/> is not removable.</param>
        public NonRemovableAttribute(string reason) : base(reason) { }

        /// <summary>
        /// Validates removal from the specified <paramref name="gameObjectToRemoveFrom"/>.
        /// </summary>
        /// <param name="gameObjectToRemoveFrom">
        /// The <see cref="GameObject"/> the <see cref="Component"/> is about to be removed from.
        /// </param>
        /// <returns>
        /// <c>false</c>, as <see cref="NonRemovableAttribute"/> makes a <see cref="Component"/> non-removable.
        /// </returns>
        public override bool Validate(GameObject gameObjectToRemoveFrom)
        {
            return false;
        }
    }
}
