using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Reference implementation of <see cref="IControllable"/> and <see cref="IDrawable"/>.
    /// </summary>
    [ThreadMode(true)]
    [DataContract(IsReference = true)]
    public abstract class Manager : DisposableEntity, IDrawable, IControllable, INameable
    {
        /// <summary>
        /// Object used to synchronize access to the state-mutating methods.
        /// </summary>
        private readonly object stateLock = new object();

        /// <summary>
        /// Notifies about the start of the enabling process.
        /// </summary>
        /// <remarks>Raised before any enabling operations.</remarks>
        public event EventHandler<ParameterEventArgs> Enabling;

        /// <summary>
        /// Notifies about the end of the enabling process.
        /// </summary>
        /// <remarks>Raised after any enabling operations.</remarks>
        public event EventHandler<ParameterEventArgs> Enabled;

        /// <summary>
        /// Notifies about the start of the disabling process.
        /// </summary>
        /// <remarks>Raised before any disabling operations.</remarks>
        public event EventHandler<ParameterEventArgs> Disabling;

        /// <summary>
        /// Notifies about the end of the disabling process.
        /// </summary>
        /// <remarks>Raised after any disabling operations.</remarks>
        public event EventHandler<ParameterEventArgs> Disabled;

        /// <summary>
        /// Notifies about the start of the drawing process.
        /// </summary>
        /// <remarks>Raised before any binding / drawing occurs.</remarks>
        public event EventHandler<ParameterEventArgs> Drawing;

        /// <summary>
        /// Notifies about the finish of the drawing process.
        /// </summary>
        /// <remarks>Raised after any binding / drawing operations.</remarks>
        public event EventHandler<ParameterEventArgs> Drawn;

        /// <summary>
        /// Notifies about the start of the loading process.
        /// </summary>
        /// <remarks>Raised before any loading operations.</remarks>
        public event EventHandler<ParameterEventArgs> Loading;

        /// <summary>
        /// Notifies about the end of the loading process.
        /// </summary>
        /// <remarks>Raised after any loading operations.</remarks>
        public event EventHandler<ParameterEventArgs> Loaded;

        /// <summary>
        /// Notifies about the start of the resetting process.
        /// </summary>
        /// <remarks>Raised before any resetting operations.</remarks>
        public event EventHandler<ParameterEventArgs> Resetting;

        /// <summary>
        /// Notifies about the end of the resetting process.
        /// </summary>
        /// <remarks>Raised after any resetting operations.</remarks>
        public event EventHandler<ParameterEventArgs> Resetted;

        /// <summary>
        /// Notifies about the start of the updating process.
        /// </summary>
        /// <remarks>Raised before any updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> Updating;

        /// <summary>
        /// Notifies about the finsih of the updating process.
        /// </summary>
        /// <remarks>Raised after any updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> Updated;

        /// <summary>
        /// Notifies about the start of the late updating process.
        /// </summary>
        /// <remarks>Raised before any late updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> LateUpdating;

        /// <summary>
        /// Notifies about the finsih of the late updating process.
        /// </summary>
        /// <remarks>Raised after any late updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> LateUpdated;

        /// <summary>
        /// Backing field.
        /// </summary>
        private volatile bool _IsEnabled = false;

        /// <summary>
        /// Indicates whether the instance is enabled or not.
        /// </summary>
        [DataMember]
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            private set
            {
                _IsEnabled = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private volatile bool _IsLoaded = false;

        /// <summary>
        /// Indicates whether the instance is loaded or not.
        /// </summary>
        [IgnoreDataMember]
        public bool IsLoaded
        {
            get
            {
                return _IsLoaded;
            }
            private set
            {
                _IsLoaded = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Manager"/>.
        /// </summary>
        protected Manager()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="Manager"/> and sets the name.
        /// </summary>
        /// <param name="name">The instance's name.</param>
        protected Manager(string name)
            : base(name)
        {
        }

        // Need volatile backing field for double-checked locking to work

        /// <summary>
        /// Enables the instance.
        /// </summary>
        /// <remarks>Calls to <see cref="M:Enable"/> will be ignored if the instance is already enabled.</remarks>
        public void Enable()
        {
            if (this.IsLoaded && !this.IsEnabled)
            {
                lock (this.stateLock)
                {
                    if (this.IsLoaded && !this.IsEnabled)
                    {
                        using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Enabling, this.Enabled))
                        {
                            this.OnEnable();
                            this.IsEnabled = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disables the instance
        /// </summary>
        /// <remarks>Calls to <see cref="M:Disable"/> will be ignored if the instance is already disabled.</remarks>
        public void Disable()
        {
            if (this.IsLoaded && this.IsEnabled)
            {
                lock (this.stateLock)
                {
                    if (this.IsLoaded && this.IsEnabled)
                    {
                        using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Disabling, this.Disabled))
                        {
                            this.OnDisable();
                            this.IsEnabled = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Triggers binding and drawing of the element.
        /// </summary>
        public void Draw()
        {
            if (this.IsLoaded && this.IsEnabled)
            {
                lock (this.stateLock)
                {
                    if (this.IsLoaded && this.IsEnabled)
                    {
                        using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
                        {
                            this.OnDraw();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the instance.
        /// </summary>
        /// <remarks>Calls to <see cref="M:Load"/> will be ignored if the instance is already loaded.</remarks>
        public void Load()
        {
            if (!this.IsLoaded)
            {
                lock (this.stateLock)
                {
                    if (!this.IsLoaded)
                    {
                        using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Loading, this.Loaded))
                        {
                            this.OnLoad();
                            this.IsLoaded = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the instance to the default values.
        /// </summary>
        /// <example>
        /// <see cref="Transform"/> resets its local position / rotation / scaling to zero / identity / one.
        /// </example>
        public void Reset()
        {
            lock (this.stateLock)
            {
                using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Resetting, this.Resetted))
                {
                    this.OnReset();
                }
            }
        }

        /// <summary>
        /// Updates the instance with the specified <see cref="GameTime"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        public void Update(GameTime gameTime)
        {
            if (this.IsLoaded && this.IsEnabled)
            {
                lock (this.stateLock)
                {
                    if (this.IsLoaded && this.IsEnabled)
                    {
                        using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
                        {
                            this.OnUpdate(gameTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the instance.
        /// </summary>
        public void LateUpdate()
        {
            if (this.IsLoaded && this.IsEnabled)
            {
                lock (this.stateLock)
                {
                    if (this.IsLoaded && this.IsEnabled)
                    {
                        using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
                        {
                            this.OnLateUpdate();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disposes the instance releasing all unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            this.IsEnabled = false;
            this.IsLoaded = false;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Callback called during <see cref="M:Enable"/> to add custom functionality.
        /// </summary>
        protected abstract void OnEnable();

        /// <summary>
        /// Callback called during <see cref="M:Disable"/> to add custom functionality.
        /// </summary>
        protected abstract void OnDisable();

        /// <summary>
        /// Callback called during <see cref="M:Draw"/> to add custom functionality.
        /// </summary>
        protected abstract void OnDraw();

        /// <summary>
        /// Callback called during <see cref="M:Load"/> to add custom functionality.
        /// </summary>
        protected abstract void OnLoad();

        /// <summary>
        /// Callback called during <see cref="M:Reset"/> to add custom functionality.
        /// </summary>
        protected abstract void OnReset();

        /// <summary>
        /// Callback called during <see cref="M:Update"/> to add custom functionality.
        /// </summary>
        protected abstract void OnUpdate(GameTime gameTime);

        /// <summary>
        /// Callback called during <see cref="M:LateUpdate"/> to add custom functionality.
        /// </summary>
        protected abstract void OnLateUpdate();
    }
}
