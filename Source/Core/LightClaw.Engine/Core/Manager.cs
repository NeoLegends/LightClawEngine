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

        public event EventHandler<EnabledChangedEventArgs> EnabledChanged;

        public event EventHandler<LoadedChangedEventArgs> LoadedChanged;

        public event EventHandler<ControllableEventArgs> Updated;

        public event PropertyChangedEventHandler PropertyChanged;

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

        private bool _IsEnabled = false;

        [ProtoMember(2)]
        public bool IsEnabled
        {
            get
            {
                return this.IsLoaded && _IsEnabled;
            }
            set
            {
                this.OnEnabledChanged(value);
                this.SetProperty(ref _IsEnabled, value);
                EventHandler<EnabledChangedEventArgs> handler = this.EnabledChanged;
                if (handler != null)
                {
                    handler(this, new EnabledChangedEventArgs(value));
                }
            }
        }

        private bool _IsLoaded = false;

        [ProtoMember(3)]
        public virtual bool IsLoaded
        {
            get
            {
                return _IsLoaded;
            }
            private set
            {
                this.SetProperty(ref _IsLoaded, value);
                EventHandler<LoadedChangedEventArgs> handler = this.LoadedChanged;
                if (handler != null)
                {
                    handler(this, new LoadedChangedEventArgs(value));
                }
            }
        }

        protected Manager()
        {
            this.Name = this.GetType().FullName;
        }

        protected Manager(string name)
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
                if (this.IsEnabled)
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

        protected abstract void OnEnabledChanged(bool isEnabled);

        protected abstract void OnLoad();

        protected abstract void OnUpdate();

        protected void SetProperty<T>(ref T location, T newValue, [CallerMemberName] string propertyName = null)
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
