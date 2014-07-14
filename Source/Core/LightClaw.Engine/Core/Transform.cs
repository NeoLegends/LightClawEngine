using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract(AsReferenceDefault = true)]
    [NonRemovable, Solitary(typeof(Transform), "An object logically cannot have more than one transform defined.")]
    public class Transform : Component
    {
        public event EventHandler<ValueChangedEventArgs<Transform>> ParentChanged;

        private ObservableCollection<Transform> _Childs = new ObservableCollection<Transform>();

        [ProtoMember(1)]
        public ObservableCollection<Transform> Childs
        {
            get
            {
                return _Childs;
            }
            set
            {
                this.SetProperty(ref _Childs, value);
            }
        }

        private Transform _Parent;

        [ProtoMember(2)]
        public Transform Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                Transform previous = this.Parent;
                this.SetProperty(ref _Parent, value);
                this.RaiseParentChanged(value, previous);
            }
        }

        private Vector3d _Position;

        [ProtoMember(3)]
        public Vector3d Position
        {
            get
            {
                return _Position;
            }
            set
            {
                this.SetProperty(ref _Position, value);
            }
        }

        //[ProtoMember(4)]
        //private Quaterniond _Rotation;

        //public Quaterniond Rotation
        //{
        //    get
        //    {
        //        return _Rotation;
        //    }
        //    set
        //    {
        //        this.SetProperty(ref _Rotation, value);
        //    }
        //}

        private Vector3d _Scale;

        [ProtoMember(5)]
        public Vector3d Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                this.SetProperty(ref _Scale, value);
            }
        }

        protected override void OnReset()
        {
            this.Position = Vector3d.Zero;
            //this.Rotation = Quaterniond.Zero;
            this.Scale = Vector3d.One;
        }

        private void RaiseParentChanged(Transform newParent, Transform oldParent)
        {
            EventHandler<ValueChangedEventArgs<Transform>> handler = this.ParentChanged;
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<Transform>(newParent, oldParent));
            }
        }
    }
}
