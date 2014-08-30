using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a disposable entity.
    /// </summary>
    [DataContract]
    public abstract class DisposableEntity : Entity, IDisposable
    {
        /// <summary>
        /// Occurs when the <see cref="DisposableEntity"/> was disposed.
        /// </summary>
        public event EventHandler<ParameterEventArgs> Disposed;

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _IsDisposed = false;

        /// <summary>
        /// Indicates whether the instance has already been disposed or not.
        /// </summary>
        [IgnoreDataMember]
        public bool IsDisposed
        {
            get
            {
                return _IsDisposed;
            }
            private set
            {
                this.SetProperty(ref _IsDisposed, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="DisposableEntity"/>.
        /// </summary>
        public DisposableEntity() { }

        /// <summary>
        /// Initializing a new <see cref="DisposableEntity"/> setting the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="Entity"/>'s name.</param>
        public DisposableEntity(string name) : base(name) { }

        /// <summary>
        /// Finalizes the <see cref="DisposableEntity"/> releasing all allocated resources.
        /// </summary>
        ~DisposableEntity()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Disposes the <see cref="DisposableEntity"/> releasing all allocated resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes the <see cref="DisposableEntity"/> releasing all unmanaged and, if specified, all managed resources.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose of managed resources as well.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                this.Raise(this.Disposed, this.GetDisposedArgument());

                this.IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Gets the event arguments for <see cref="E:Disposed"/>.
        /// </summary>
        /// <returns>The event argument parameter.</returns>
        protected virtual object GetDisposedArgument()
        {
            return null;
        }
    }
}
