using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using log4net;
using Munq;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [DataContract(IsReference = true)]
    [Description("A lightweight component base class implementing essential services such as INotifyPropertyChanged and INameable.")]
    public abstract class Entity : INameable, INotifyPropertyChanged
    {
        protected static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
        [Description("An Ioc-Container used to obtain certain service instances such as IContentManager at runtime.")]
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
