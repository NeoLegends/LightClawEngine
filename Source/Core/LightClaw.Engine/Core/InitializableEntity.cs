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
    /// Represents an <see cref="DisposableEntity"/> that can be initialized lazily.
    /// </summary>
    [DataContract]
    public abstract class InitializableEntity : DisposableEntity, IInitializable
    {
        /// <summary>
        /// Used to lock access to the initialization method.
        /// </summary>
        private readonly object initializationLock = new object();

        /// <summary>
        /// Backing field.
        /// </summary>
        private volatile bool _IsInitialized = false;

        /// <summary>
        /// Indicates whether the instance has already been initialized or not.
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            //private set
            //{
            //    this.SetProperty(ref _IsInitialized, value);
            //}
        }
        
        /// <summary>
        /// Initializes a new <see cref="InitializableEntity"/>.
        /// </summary>
        protected InitializableEntity() { }

        /// <summary>
        /// Initializes a new <see cref="InitializableEntity"/> and sets the name.
        /// </summary>
        protected InitializableEntity(string name) : base(name) { }

        /// <summary>
        /// Initializes the entity in a thread-safe manner.
        /// </summary>
        public void Initialize()
        {
            if (!this._IsInitialized) // Use backing field for volatility
            {
                lock (this.initializationLock)
                {
                    if (!this._IsInitialized)
                    {
                        this.OnInitialize();
                        this._IsInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Callback doing the actual initialization work.
        /// </summary>
        protected abstract void OnInitialize();
    }
}
