using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        public event NotifyCollectionChangedEventHandler ChildrenChanged;

        public event EventHandler<ValueChangedEventArgs<Transform>> ParentChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> PositionChanged;

        public event EventHandler<ValueChangedEventArgs<Quaternion>> RotationChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> ScalingChanged;

        private readonly ObservableCollection<Transform> _Childs = new ObservableCollection<Transform>();

        [ProtoMember(1)]
        public ObservableCollection<Transform> Childs
        {
            get
            {
                return _Childs;
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

        private Vector3 _Position = Vector3.Zero;

        [ProtoMember(3)]
        public Vector3 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                Vector3 previous = this.Position;
                this.SetProperty(ref _Position, value);
                this.RaisePositionChanged(value, previous);
            }
        }

        private Quaternion _Rotation = Quaternion.Zero;

        [ProtoMember(4)]
        public Quaternion Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                Quaternion previous = this.Rotation;
                this.SetProperty(ref _Rotation, value);
                this.RaiseRotationChanged(value, previous);
            }
        }

        private Vector3 _Scale = Vector3.Zero;

        [ProtoMember(5)]
        public Vector3 Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                Vector3 previous = this.Scale;
                this.SetProperty(ref _Scale, value);
                this.RaiseScalingChanged(value, previous);
            }
        }

        public Transform() 
        {
            this.Childs.CollectionChanged += (s, e) => this.RaiseChildrenChanged(s, e);
        }

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
            : this()
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
        }

        public Transform(Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
            : this(position, rotation, scale)
        {
            this.Parent = parent;
        }

        protected override void OnReset()
        {
            this.Position = Vector3.Zero;
            this.Rotation = Quaternion.Zero;
            this.Scale = Vector3.One;
        }

        private void RaiseChildrenChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler handler = this.ChildrenChanged;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        private void RaiseParentChanged(Transform newParent, Transform oldParent)
        {
            EventHandler<ValueChangedEventArgs<Transform>> handler = this.ParentChanged;
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<Transform>(newParent, oldParent));
            }
        }

        private void RaisePositionChanged(Vector3 newPosition, Vector3 oldPosition)
        {
            EventHandler<ValueChangedEventArgs<Vector3>> handler = this.PositionChanged;
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<Vector3>(newPosition, oldPosition));
            }
        }

        private void RaiseRotationChanged(Quaternion newRotation, Quaternion oldRotation)
        {
            EventHandler<ValueChangedEventArgs<Quaternion>> handler = this.RotationChanged;
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<Quaternion>(newRotation, oldRotation));
            }
        }

        private void RaiseScalingChanged(Vector3 newScaling, Vector3 oldScaling)
        {
            EventHandler<ValueChangedEventArgs<Vector3>> handler = this.ScalingChanged;
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<Vector3>(newScaling, oldScaling));
            }
        }
    }
}
