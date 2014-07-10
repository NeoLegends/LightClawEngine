using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SolitaryAttribute : AttachmentValidatorAttribute
    {
        public SolitaryAttribute(Type componentType)
            : base(componentType)
        {
            Contract.Requires<ArgumentNullException>(componentType != null);
        }

        public override bool Validate(GameObject gameObjectToAttachTo, IEnumerable<Component> componentsToAttach)
        {
            return !gameObjectToAttachTo.Concat(componentsToAttach)
                                        .Distinct()
                                        .Any(component => this.ComponentType.IsAssignableFrom(component.GetType()));
        }
    }
}
