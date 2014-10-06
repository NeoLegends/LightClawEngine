using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Abstract base class for all component validators.
    /// </summary>
    /// <remarks>
    /// A component validator is an attribute that validates attachment to or removal from a specified
    /// <see cref="GameObject"/>. It can deny the process resulting in no change in the scene tree.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class ComponentValidatorAttribute : Attribute
    {
        /// <summary>
        /// The <see cref="Type"/> of component that is involved in the validation.
        /// </summary>
        public Type ComponentType { get; protected set; }

        /// <summary>
        /// The reason why the validator works the way it works.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ComponentValidatorAttribute"/>.
        /// </summary>
        protected ComponentValidatorAttribute()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ComponentValidatorAttribute"/> setting the type of the component.
        /// </summary>
        /// <param name="componentType">The <see cref="Type"/> of component to validate.</param>
        protected ComponentValidatorAttribute(Type componentType)
            : this(componentType, null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ComponentValidatorAttribute"/> setting the reason.
        /// </summary>
        /// <param name="reason">The reason why the validator works the way it works.</param>
        protected ComponentValidatorAttribute(string reason)
            : this(null, reason)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ComponentValidatorAttribute"/> setting the reason and the type of the component.
        /// </summary>
        /// <param name="reason">The reason why the validator works the way it works.</param>
        /// <param name="componentType">The <see cref="Type"/> of component to validate.</param>
        protected ComponentValidatorAttribute(Type componentType, string reason)
        {
            this.ComponentType = componentType;
            this.Reason = reason;
        }
    }

    /// <summary>
    /// Validates the attachment of a <see cref="Component"/> to a <see cref="GameObject"/>.
    /// </summary>
    [ContractClass(typeof(AttachmentValidatorAttributeContracts))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class AttachmentValidatorAttribute : ComponentValidatorAttribute
    {
        /// <summary>
        /// Initializes a new <see cref="AttachmentValidatorAttribute"/>.
        /// </summary>
        public AttachmentValidatorAttribute()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="AttachmentValidatorAttribute"/>.
        /// </summary>
        /// <param name="reason">The reason why the validator works the way it works.</param>
        public AttachmentValidatorAttribute(string reason)
            : base(reason)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="AttachmentValidatorAttribute"/> setting the <see cref="Type"/> of component to
        /// be validated.
        /// </summary>
        /// <param name="componentType">The <see cref="Type"/> of component to validate.</param>
        public AttachmentValidatorAttribute(Type componentType)
            : base(componentType)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="AttachmentValidatorAttribute"/>.
        /// </summary>
        /// <param name="reason">The reason why the validator works the way it works.</param>
        /// <param name="componentType">The <see cref="Type"/> of component to validate.</param>
        public AttachmentValidatorAttribute(Type componentType, string reason)
            : base(componentType, reason)
        {
        }

        /// <summary>
        /// Validates attachment of the <see cref="Component"/> to the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gameObjectToAttachTo">
        /// The <see cref="GameObject"/> the <see cref="Component"/> is about to be attached to.
        /// </param>
        /// <returns><c>true</c> if the attachment can be done, otherwise <c>false</c> .</returns>
        public bool Validate(GameObject gameObjectToAttachTo)
        {
            Contract.Requires<ArgumentNullException>(gameObjectToAttachTo != null);

            return this.Validate(gameObjectToAttachTo, Enumerable.Empty<Component>());
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
        /// <returns><c>true</c> if the attachment can be done, otherwise <c>false</c> .</returns>
        public abstract bool Validate(GameObject gameObjectToAttachTo, IEnumerable<Component> componentsToAttach);
    }

    /// <summary>
    /// Validates the removal of a <see cref="Component"/> from a <see cref="GameObject"/>.
    /// </summary>
    [ContractClass(typeof(RemovalValidatorAttributeContracts))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class RemovalValidatorAttribute : ComponentValidatorAttribute
    {
        /// <summary>
        /// Initializes a new <see cref="RemovalValidatorAttribute"/>.
        /// </summary>
        public RemovalValidatorAttribute()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="RemovalValidatorAttribute"/> setting the <see cref="Type"/> of component to be validated.
        /// </summary>
        /// <param name="componentType">The <see cref="Type"/> of component to validate.</param>
        public RemovalValidatorAttribute(Type componentType)
            : base(componentType)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="RemovalValidatorAttribute"/>.
        /// </summary>
        /// <param name="reason">The reason why the validator works the way it works.</param>
        public RemovalValidatorAttribute(string reason)
            : base(reason)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="RemovalValidatorAttribute"/>.
        /// </summary>
        /// <param name="reason">The reason why the validator works the way it works.</param>
        /// <param name="componentType">The <see cref="Type"/> of component to validate.</param>
        public RemovalValidatorAttribute(Type componentType, string reason)
            : base(componentType, reason)
        {
        }

        /// <summary>
        /// Validates a removal process from the specified <paramref name="gameObjectToRemoveFrom"/>.
        /// </summary>
        /// <param name="gameObjectToRemoveFrom">
        /// The <see cref="GameObject"/> the <see cref="Component"/> is about to be removed from.
        /// </param>
        /// <returns><c>true</c> if the <see cref="Component"/> can be safely removed, otherwise <c>false</c> .</returns>
        public abstract bool Validate(GameObject gameObjectToRemoveFrom);
    }

    [ContractClassFor(typeof(AttachmentValidatorAttribute))]
    internal abstract class AttachmentValidatorAttributeContracts : AttachmentValidatorAttribute
    {
        public override bool Validate(GameObject gameObjectToAttachTo, IEnumerable<Component> componentsToAttach)
        {
            Contract.Requires<ArgumentNullException>(gameObjectToAttachTo != null);
            Contract.Requires<ArgumentNullException>(componentsToAttach != null);

            return false;
        }
    }

    [ContractClassFor(typeof(RemovalValidatorAttribute))]
    internal abstract class RemovalValidatorAttributeContracts : RemovalValidatorAttribute
    {
        public override bool Validate(GameObject gameObjectToRemoveFrom)
        {
            Contract.Requires<ArgumentNullException>(gameObjectToRemoveFrom != null);

            return false;
        }
    }
}
