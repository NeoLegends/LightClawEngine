using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents an attachment validator that allows to declare dependencies of the <see cref="Component"/> on other
    /// <see cref="Component"/>s.
    /// </summary>
    /// <remarks>
    /// Dependency on <see cref="Transform"/> and <see cref="Coroutines.CoroutineController"/> do not (!) have to be
    /// declared explicitly, they will be managed by the <see cref="GameObject"/> and automatically added.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class DependsOnAttribute : AttachmentValidatorAttribute
    {
        /// <summary>
        /// The dependencies of the <see cref="Component"/>.
        /// </summary>
        public IEnumerable<Type> Dependencies { get; private set; }

        /// <summary>
        /// Initialies a new <see cref="DependsOnAttribute"/> setting the dependencies.
        /// </summary>
        /// <param name="dependencies">The dependencies of the <see cref="Component"/>.</param>
        public DependsOnAttribute(params Type[] dependencies)
        {
            Contract.Requires<ArgumentNullException>(dependencies != null);
            Contract.Requires<ArgumentException>(dependencies.All(dependency => dependency != null));
            Contract.Requires<ArgumentException>(dependencies.All(dependency => typeof(Component).IsAssignableFrom(dependency)));

            this.Dependencies = dependencies.Distinct();
        }

        /// <summary>
        /// Validates the attachment process checking whether the dependencies are met.
        /// </summary>
        /// <param name="gameObjectToAttachTo">
        /// The <see cref="GameObject"/> the <see cref="Component"/> is about to be attached to.
        /// </param>
        /// <param name="componentsToAttach">
        /// Other <see cref="Component"/>s (except the current one being attached!) that are supposed to be attached in
        /// the same transaction as the current object. Used to provide support for attaching multiple interdependent
        /// <see cref="Component"/>s.
        /// </param>
        /// <returns><c>true</c> if the attachment can be done, otherwise <c>false</c> .</returns>
        public override bool Validate(GameObject gameObjectToAttachTo, IEnumerable<Component> componentsToAttach)
        {
            IEnumerable<Component> totalComponents = gameObjectToAttachTo.Concat(componentsToAttach).Distinct();

            return this.Dependencies.All(requiredComponent => totalComponents.Any(component => requiredComponent.IsAssignableFrom(component.GetType())));
        }
    }
}
