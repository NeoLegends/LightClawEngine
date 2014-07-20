using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract]
    public class Entity : IControllable, INotifyPropertyChanged
    {
        public event EventHandler<ControllableEventArgs> Loaded;

        public event EventHandler<ControllableEventArgs> Updated;

        public event EventHandler<ControllableEventArgs> ShutDown;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsLoaded = false;

        [ProtoMember(2)]
        public bool IsLoaded
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
                    EventHandler<ControllableEventArgs> handler = this.ShutDown;
                    if (handler != null)
                    {
                        handler(this, new ControllableEventArgs());
                    }
                }
            }
        }

        private String _Name;

        [ProtoMember(1)]
        public String Name
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

        public Entity()
        {
            this.Name = this.GetType().FullName;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public Task Load()
        {
            return this.OnLoad().ContinueWith(t => this.IsLoaded = true, TaskContinuationOptions.ExecuteSynchronously);
        }

        public Task Update()
        {
            return this.OnUpdate().ContinueWith(t =>
            {
                EventHandler<ControllableEventArgs> handler = this.Updated;
                if (handler != null)
                {
                    handler(this, new ControllableEventArgs());
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public Task Shutdown()
        {
            return this.OnShutdown().ContinueWith(t => this.IsLoaded = false, TaskContinuationOptions.ExecuteSynchronously);
        }

        protected abstract Task OnLoad();

        protected abstract Task OnUpdate();

        protected abstract Task OnShutdown();

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
