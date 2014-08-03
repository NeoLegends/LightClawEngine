using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
    {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class ComponentValidatorAttribute : Attribute
    {
        public Type ComponentType { get; protected set; }

        protected ComponentValidatorAttribute() { }

        protected ComponentValidatorAttribute(Type componentType)
        {
            this.ComponentType = componentType;
        }
    }

    [ContractClass(typeof(AttachmentValidatorAttributeContracts))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class AttachmentValidatorAttribute : ComponentValidatorAttribute
    {
        public AttachmentValidatorAttribute() { }

        public AttachmentValidatorAttribute(Type componentType) : base(componentType) { }

        public bool Validate(GameObject gameObjectToAttachTo)
        {
            Contract.Requires<ArgumentNullException>(gameObjectToAttachTo != null);

            return this.Validate(gameObjectToAttachTo, Enumerable.Empty<Component>());
        }

        public abstract bool Validate(GameObject gameObjectToAttachTo, IEnumerable<Component> componentsToAttach);
    }
    
    [ContractClass(typeof(RemovalValidatorAttributeContracts))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class RemovalValidatorAttribute : ComponentValidatorAttribute
    {
        public RemovalValidatorAttribute() { }

        public RemovalValidatorAttribute(Type componentType) : base(componentType) { }

        public abstract bool Validate(GameObject gameObjectToRemoveFrom);
    }

    [ContractClassFor(typeof(AttachmentValidatorAttribute))]
    abstract class AttachmentValidatorAttributeContracts : AttachmentValidatorAttribute
    {
        public override bool Validate(GameObject gameObjectToAttachTo, IEnumerable<Component> componentsToAttach)
        {
            Contract.Requires<ArgumentNullException>(gameObjectToAttachTo != null);
            Contract.Requires<ArgumentNullException>(componentsToAttach != null);

            return false;
        }
    }

    [ContractClassFor(typeof(RemovalValidatorAttribute))]
    abstract class RemovalValidatorAttributeContracts : RemovalValidatorAttribute
    {
        public override bool Validate(GameObject gameObjectToRemoveFrom)
        {
            Contract.Requires<ArgumentNullException>(gameObjectToRemoveFrom != null);

            return false;
        }
    }
}
