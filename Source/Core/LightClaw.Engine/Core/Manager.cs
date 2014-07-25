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
    [DataContract]
    public abstract class Manager : Entity, IDrawable, IControllable, INameable
    {
        private object stateLock = new object();

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

        private string _Name;

        [DataMember]
        public string Name
        {
            get
            {
                return _Name ?? (Name = this.GetType().FullName);
            }
            set
            {
                this.SetProperty(ref _Name, value);
            }
        }

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
                    this.Raise(this.Enabling);
                    this.OnEnable();
                    this.IsEnabled = true;
                    this.Raise(this.Enabled);
                }
            }
        }

        public void Disable()
        {
            lock (this.stateLock)
            {
                if (this.IsLoaded && this.IsEnabled)
                {
                    this.Raise(this.Disabling);
                    this.OnDisable();
                    this.IsEnabled = false;
                    this.Raise(this.Disabled);
                }
            }
        }

        public void Draw()
        {
            lock (this.stateLock)
            {
                if (this.IsLoaded && this.IsEnabled)
                {
                    this.Raise(this.Drawing);
                    this.OnDraw();
                    this.Raise(this.Drawn);
                }
            }
        }

        public void Load()
        {
            lock (this.stateLock)
            {
                if (!this.IsLoaded)
                {
                    this.Raise(this.Loading);
                    this.OnLoad();
                    this.IsLoaded = true;
                    this.Raise(this.Loaded);
                }
            }
        }

        public void Reset()
        {
            lock (this.stateLock)
            {
                this.Raise(this.Resetting);
                this.OnReset();
                this.Raise(this.Resetted);
            }
        }

        public void Update(GameTime gameTime)
        {
            lock (this.stateLock)
            {
                if (this.IsEnabled)
                {
                    this.Raise(this.Updating);
                    this.OnUpdate(gameTime);
                    this.Raise(this.Updated);
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

        private void Raise(EventHandler<ParameterEventArgs> handler, ParameterEventArgs args = null)
        {
            if (handler != null)
            {
                handler(this, args ?? new ParameterEventArgs());
            }
        }
    }
}
