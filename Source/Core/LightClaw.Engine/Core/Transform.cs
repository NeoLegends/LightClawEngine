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
    [NonRemovable, Solitary(typeof(Transform), "An object cannot be transformed by multiple components.")]
    public class Transform : Component, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler ChildrenChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> LocalPositionChanged;

        public event EventHandler<ValueChangedEventArgs<Quaternion>> LocalRotationChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> LocalScalingChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> PositionChanged;

        public event EventHandler<ValueChangedEventArgs<Transform>> ParentChanged;

        public event EventHandler<ValueChangedEventArgs<Quaternion>> RotationChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> ScalingChanged;

        [ProtoMember(1)]
        private ObservableCollection<Transform> _Childs = new ObservableCollection<Transform>();

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
                this.Raise(this.ParentChanged, value, previous);
            }
        }

        private Vector3 _LocalPosition;

        [ProtoMember(3)]
        public Vector3 LocalPosition
        {
            get
            {
                return _LocalPosition;
            }
            set
            {
                this.SetProperty(ref _LocalPosition, value);
            }
        }

        private Vector3 _Position = Vector3.Zero;

        public Vector3 Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                Vector3 previous = this.Position;
                this.SetProperty(ref _Position, value);
                this.Raise(this.PositionChanged, value, previous);
            }
        }

        private Quaternion _LocalRotation;

        [ProtoMember(4)]
        public Quaternion LocalRotation
        {
            get
            {
                return _LocalRotation;
            }
            set
            {
                this.SetProperty(ref _LocalRotation, value);
            }
        }

        private Quaternion _Rotation = Quaternion.Zero;

        public Quaternion Rotation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                Quaternion previous = this.Rotation;
                this.SetProperty(ref _Rotation, value);
                this.Raise(this.RotationChanged, value, previous);
            }
        }

        private Vector3 _LocalScale;

        [ProtoMember(5)]
        public Vector3 LocalScale
        {
            get
            {
                return _LocalScale;
            }
            set
            {
                this.SetProperty(ref _LocalScale, value);
            }
        }

        private Vector3 _Scale = Vector3.Zero;

        public Vector3 Scale
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                Vector3 previous = this.Scale;
                this.SetProperty(ref _Scale, value);
                this.Raise(this.ScalingChanged, value, previous);
            }
        }

        public Transform() 
        {
            this.Childs.CollectionChanged += (s, e) =>
            {
                NotifyCollectionChangedEventHandler handler = this.ChildrenChanged;
                if (handler != null)
                {
                    handler(s, e);
                }
                handler = this.CollectionChanged;
                if (handler != null)
                {
                    handler(s, e);
                }
            };
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

        private void Raise<T>(EventHandler<ValueChangedEventArgs<T>> handler, T newValue, T oldValue)
        {
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<T>(newValue, oldValue));
            }
        }

        [Tag("Congrats, you are a true C# zen master and found this hidden method. Might allow polan into space.")]
        private void Randomize()
        {
            this.Position = Vector3.Random;
            this.Rotation = Quaternion.Random;
            this.Scale = Vector3.Random;
        }
    }
}
