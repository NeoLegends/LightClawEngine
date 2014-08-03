using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using log4net;
using Munq;

namespace LightClaw.Engine.Core
{
    [DataContract(IsReference = true)]
    public abstract class Manager : Entity, IDrawable, IControllable, INameable
    {
        private readonly object stateLock = new object();

        public event EventHandler<ParameterEventArgs> Enabling;

        public event EventHandler<ParameterEventArgs> Enabled;

        public event EventHandler<ParameterEventArgs> Disabling;

        public event EventHandler<ParameterEventArgs> Disabled;

        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public event EventHandler<ParameterEventArgs> Loading;

        public event EventHandler<ParameterEventArgs> Loaded;

        public event EventHandler<ParameterEventArgs> Resetting;

        public event EventHandler<ParameterEventArgs> Resetted;

        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        private bool _IsEnabled = false;

        [DataMember]
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            private set
            {
                this.SetProperty(ref _IsEnabled, value);
            }
        }

        private bool _IsLoaded = false;

        [IgnoreDataMember]
        public bool IsLoaded
        {
            get
            {
                return _IsLoaded;
            }
            private set
            {
                this.SetProperty(ref _IsLoaded, value);
            }
        }

        protected Manager()
        {
            this.IocC = LightClawEngine.DefaultIocContainer;
        }

        protected Manager(string name)
            : this()
        {
            this.Name = name;
        }

        ~Manager()
        {
            this.Dispose(false);
        }

        public void Enable()
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

        public void Disable()
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

        public void Draw()
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

        public void Load()
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

        public void Update(GameTime gameTime)
        {
            lock (this.stateLock)
            {
                if (this.IsEnabled)
                {
                    using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
                    {
                        this.OnUpdate(gameTime);
                    }
                }
            }
        }

        public void LateUpdate()
        {
            lock (this.stateLock)
            {
                if (this.IsEnabled)
                {
                    using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
                    {
                        this.OnLateUpdate();
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public override string ToString()
        {
            return this.Name ?? base.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            this.IsEnabled = false;
            this.IsLoaded = false;
            GC.SuppressFinalize(this);
        }

        protected abstract void OnEnable();

        protected abstract void OnDisable();

        protected abstract void OnDraw();

        protected abstract void OnLoad();

        protected abstract void OnReset();

        protected abstract void OnUpdate(GameTime gameTime);

        protected abstract void OnLateUpdate();
    }
}
