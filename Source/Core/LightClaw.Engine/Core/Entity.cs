using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Munq;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [DataContract]
    public abstract class Entity : INameable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _Name;

        [DataMember]
        public virtual string Name
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

        private IocContainer _IocC = LightClawEngine.DefaultIocContainer;

        [IgnoreDataMember]
        public IocContainer IocC
        {
            get
            {
                return _IocC;
            }
            protected set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                _IocC = value;
            }
        }

        protected Entity() { }

        protected void SetProperty<T>(ref T location, T newValue, [CallerMemberName] string propertyName = null)
        {
            location = newValue;
            this.RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void Raise(EventHandler<ParameterEventArgs> handler, ParameterEventArgs args = null)
        {
            if (handler != null)
            {
                handler(this, args ?? ParameterEventArgs.Default);
            }
        }
    }
}
