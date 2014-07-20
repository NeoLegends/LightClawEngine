using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Munq;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Manager))]
    public abstract class Entity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [CLSCompliant(false)]
        [IgnoreDataMember, ProtoIgnore]
        public IocContainer IocC { get; protected set; }

        protected Entity()
        {
            this.IocC = LightClawEngine.DefaultIocContainer;
        }

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
    }
}
