using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class RequiresAttribute : AttachmentValidatorAttribute
    {
        public IEnumerable<Type> Requirements { get; private set; }

        public RequiresAttribute(params Type[] requiredComponents)
        {
            Contract.Requires<ArgumentNullException>(requiredComponents != null);
            Contract.Requires<ArgumentException>(requiredComponents.All(reqComp => reqComp != null));
            Contract.Requires<ArgumentException>(requiredComponents.All(reqComp => typeof(Component).IsAssignableFrom(reqComp)));

            this.Requirements = requiredComponents.Distinct();
        }

        public override bool Validate(GameObject gameObjectToAttachTo, IEnumerable<Component> componentsToAttach)
        {
            IEnumerable<Component> totalComponents = gameObjectToAttachTo.Concat(componentsToAttach).Distinct();

            return this.Requirements.All(requiredComponent => totalComponents.Any(component => requiredComponent.IsAssignableFrom(component.GetType())));
        }
    }
}
