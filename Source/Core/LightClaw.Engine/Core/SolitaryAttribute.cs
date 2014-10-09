using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents an attachment validator that allows only one <see cref="Component"/> of the specified
    /// <see cref="Type"/> at the same time on a <see cref="GameObject"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SolitaryAttribute : AttachmentValidatorAttribute
    {
        /// <summary>
        /// Initializes a new <see cref="SolitaryAttribute"/> setting the type of the <see cref="Component"/>.
        /// </summary>
        /// <param name="componentType">The <see cref="Type"/> of <see cref="Component"/> that is solitary.</param>
        public SolitaryAttribute(Type componentType)
            : base(componentType)
        {
            Contract.Requires<ArgumentNullException>(componentType != null);
        }

        /// <summary>
        /// Initializes a new <see cref="SolitaryAttribute"/> setting the type of the <see cref="Component"/> and the reason.
        /// </summary>
        /// <param name="componentType">The <see cref="Type"/> of <see cref="Component"/> that is solitary.</param>
        /// <param name="reason">The reason why the <see cref="Component"/> is solitary.</param>
        public SolitaryAttribute(Type componentType, string reason)
            : base(componentType, reason)
        {
            Contract.Requires<ArgumentNullException>(componentType != null);
        }

        /// <summary>
        /// Validates attachment of the <see cref="Component"/> to the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gameObjectToAttachTo">
        /// The <see cref="GameObject"/> the <see cref="Component"/> is about to be attached to.
        /// </param>
        /// <param name="componentsToAttach">
        /// Other <see cref="Component"/>s (except the current one being attached!) that are supposed to be attached in
        /// the same transaction as the current object. Used to provide support for attaching multiple interdependent
        /// <see cref="Component"/>s.
        /// </param>
        /// <returns><c>true</c> if the attachment can be done, otherwise <c>false</c>.</returns>
        public override bool Validate(GameObject gameObjectToAttachTo, IEnumerable<Component> componentsToAttach)
        {
            return !gameObjectToAttachTo.Concat(componentsToAttach)
                                        .Distinct()
                                        .Any(component => this.ComponentType.IsAssignableFrom(component.GetType()));
        }
    }
}
