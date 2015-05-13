using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Threading;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents an entity that is associated with a <see cref="Dispatcher"/>.
    /// </summary>
    public class DispatcherEntity : DisposableEntity, IDispatcherObject
    {
        private Dispatcher _Dispatcher;

        /// <summary>
        /// The <see cref="Dispatcher"/> used to dispatch graphics events.
        /// </summary>
        [IgnoreDataMember]
        public Dispatcher Dispatcher
        {
            get
            {
                return _Dispatcher;
            }
            protected set
            {
                _Dispatcher = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="DispatcherEntity"/>.
        /// </summary>
        public DispatcherEntity() : this(null) { }

        /// <summary>
        /// Initializes a new <see cref="DispatcherEntity"/> and sets its name.
        /// </summary>
        /// <param name="name">The name.</param>
        public DispatcherEntity(string name) 
            : base(name) 
        {
            this.Dispatcher = this.IocC.Resolve<Dispatcher>();
        }
    }
}
