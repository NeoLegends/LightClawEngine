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
        public Type ComponentType { get; private set; }

        protected ComponentValidatorAttribute(Type componentType)
        {
            this.ComponentType = componentType;
        }

        public abstract bool Validate(GameObject gameObject);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class ComponentAttachmentValidatorAttribute : ComponentValidatorAttribute 
    { 
        protected ComponentAttachmentValidatorAttribute(Type componentType) : base(componentType) { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class ComponentRemovalValidatorAttribute : ComponentValidatorAttribute 
    { 
        protected ComponentRemovalValidatorAttribute(Type componentType) : base(componentType) { }
    }
}
