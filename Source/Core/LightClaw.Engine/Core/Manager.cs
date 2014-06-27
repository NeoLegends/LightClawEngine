using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract]
    public abstract class Manager : IControllable, INameable, INotifyPropertyChanged
    {
        private object loadedStateLock = new object();

        public event EventHandler<ControllableEventArgs> Loaded;

        public event EventHandler<ControllableEventArgs> Updated;

        public event EventHandler<ControllableEventArgs> Unloaded;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsLoaded = false;

        [ProtoMember(2)]
        public virtual bool IsLoaded
        {
            get
            {
                return _IsLoaded;
            }
            private set
            {
                this.SetProperty(ref _IsLoaded, value);
                if (value)
                {
                    EventHandler<ControllableEventArgs> handler = this.Loaded;
                    if (handler != null)
                    {
                        handler(this, new ControllableEventArgs());
                    }
                }
                else
                {
                    EventHandler<ControllableEventArgs> handler = this.Unloaded;
                    if (handler != null)
                    {
                        handler(this, new ControllableEventArgs());
                    }
                }
            }
        }

        private string _Name;

        [ProtoMember(1)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                this.SetProperty(ref _Name, value);
            }
        }

        protected Manager()
        {
            this.Name = this.GetType().FullName;
        }

        protected Manager(String name)
        {
            this.Name = name;
        }

        ~Manager()
        {
            this.Dispose(false);
        }

        public void Load()
        {
            lock (this.loadedStateLock)
            {
                if (!this.IsLoaded)
                {
                    this.OnLoad();
                    this.IsLoaded = true;
                }
            }
        }

        public void Update()
        {
            lock (this.loadedStateLock)
            {
                if (this.IsLoaded)
                {
                    this.OnUpdate();
                    EventHandler<ControllableEventArgs> handler = this.Updated;
                    if (handler != null)
                    {
                        handler(this, new ControllableEventArgs());
                    }
                }
            }
        }

        public void Unload()
        {
            lock (this.loadedStateLock)
            {
                if (this.IsLoaded)
                {
                    this.OnShutdown();
                    this.IsLoaded = false;
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
            this.Unload();
        }

        protected abstract void OnLoad();

        protected abstract void OnUpdate();

        protected abstract void OnShutdown();

        protected void SetProperty<T>(ref T location, T newValue, [CallerMemberName] String propertyName = null)
        {
            location = newValue;
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
