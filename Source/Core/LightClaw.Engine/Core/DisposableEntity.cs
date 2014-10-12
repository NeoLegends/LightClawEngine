using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
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
        private int _IsDisposed = 0;

        /// <summary>
        /// Indicates whether the instance has already been disposed or not.
        /// </summary>
        [IgnoreDataMember]
        public bool IsDisposed
        {
            get
            {
                return (_IsDisposed == 1);
            }
            private set
            {
                this.SetProperty(ref _IsDisposed, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="DisposableEntity"/>.
        /// </summary>
        public DisposableEntity()
        {
        }

        /// <summary>
        /// Initializing a new <see cref="DisposableEntity"/> setting the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="Entity"/>s name.</param>
        public DisposableEntity(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Finalizes the <see cref="DisposableEntity"/> releasing all allocated resources.
        /// </summary>
        ~DisposableEntity()
        {
            this.DisposeInternal(false);
        }

        /// <summary>
        /// Disposes the <see cref="DisposableEntity"/> releasing all allocated resources.
        /// </summary>
        public void Dispose()
        {
            this.DisposeInternal(true);
        }

        /// <summary>
        /// Disposes the <see cref="DisposableEntity"/> releasing all unmanaged and, if specified, all managed resources. See remarks.
        /// </summary>
        /// <remarks>
        /// When overriding this method, DO NOT check <see cref="P:IsDisposed"/> and do nothing if it is true. <see cref="P:IsDisposed"/>
        /// will be set to true with a thread-safe operation BEFORE this method will be called.
        /// </remarks>
        /// <param name="disposing">Indicates whether to dispose of managed resources as well.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.Raise(this.Disposed, this.GetDisposedArgument());
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the event arguments for <see cref="E:Disposed"/>.
        /// </summary>
        /// <returns>The event argument parameter.</returns>
        protected virtual object GetDisposedArgument()
        {
            return null;
        }

        /// <summary>
        /// Callback for <see cref="M:Dispose()"/> and <see cref="M:Finalize"/>
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose of managed resources as well.</param>
        private void DisposeInternal(bool disposing)
        {
            if (Interlocked.CompareExchange(ref _IsDisposed, 1, 0) == 0)
            {
                this.Dispose(disposing);
            }
        }
    }
}
