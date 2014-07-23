using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class DependsOnAttribute : AttachmentValidatorAttribute
    {
        public IEnumerable<Type> Dependencies { get; private set; }

        public DependsOnAttribute(params Type[] dependencies)
        {
            Contract.Requires<ArgumentNullException>(dependencies != null);
            Contract.Requires<ArgumentException>(dependencies.All(reqComp => reqComp != null));
            Contract.Requires<ArgumentException>(dependencies.All(reqComp => typeof(Component).IsAssignableFrom(reqComp)));

            this.Dependencies = dependencies.Distinct();
        }

        public override bool Validate(GameObject gameObjectToAttachTo, IEnumerable<Component> componentsToAttach)
        {
            IEnumerable<Component> totalComponents = gameObjectToAttachTo.Concat(componentsToAttach).Distinct();

            return this.Dependencies.All(requiredComponent => totalComponents.Any(component => requiredComponent.IsAssignableFrom(component.GetType())));
        }
    }
}
