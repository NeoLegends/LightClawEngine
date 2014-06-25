using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class ComponentValidatorAttribute : Attribute
    {
        public abstract bool Validate(GameObject gameObject);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class ComponentAttachmentValidatorAttribute : ComponentValidatorAttribute { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class ComponentRemovalValidatorAttribute : ComponentValidatorAttribute { }
}
