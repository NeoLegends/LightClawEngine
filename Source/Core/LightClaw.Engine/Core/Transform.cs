using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace LightClaw.Engine.Core
{
    [DataContract]
    [Description("Contains an object's position, rotation and scaling relativ to it's parent and in world space.")]
    [NonRemovable, Solitary(typeof(Transform), "An object cannot be transformed by multiple components.")]
    public class Transform : Component, INotifyCollectionChanged
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Transform));

        public event NotifyCollectionChangedEventHandler ChildrenChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> LocalPositionChanged;

        public event EventHandler<ValueChangedEventArgs<Quaternion>> LocalRotationChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> LocalScalingChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> PositionChanged;

        public event EventHandler<ValueChangedEventArgs<Transform>> ParentChanged;

        public event EventHandler<ValueChangedEventArgs<Quaternion>> RotationChanged;

        public event EventHandler<ValueChangedEventArgs<Vector3>> ScalingChanged;

        private ObservableCollection<Transform> _Childs = new ObservableCollection<Transform>();

        public ObservableCollection<Transform> Childs
        {
            get
            {
                return _Childs;
            }
            private set
            {
                this.SetProperty(ref _Childs, value);
            }
        }

        private Transform _Parent;

        [DataMember]
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

        private Vector3 _LocalPosition = Vector3.Zero;

        [DataMember]
        public Vector3 LocalPosition
        {
            get
            {
                return _LocalPosition;
            }
            set
            {
                Vector3 previous = this.LocalPosition;
                this.SetProperty(ref _LocalPosition, value);
                this.Raise(this.LocalPositionChanged, value, previous);
            }
        }

        [IgnoreDataMember]
        public Vector3 Position
        {
            get
            {
                Transform parent = this.Parent;
                return (parent != null) ? parent.Position + this.LocalPosition : this.LocalPosition;
            }
            set
            {
                Vector3 previous = this.Position;
                //throw new NotImplementedException();
                //this.SetProperty(ref _Position, value);
                this.Raise(this.PositionChanged, value, previous);
            }
        }

        public Matrix PositionMatrix
        {
            get
            {
                return Matrix.Translation(this.Position);
            }
        }

        private Quaternion _LocalRotation = Quaternion.Identity;

        [DataMember]
        public Quaternion LocalRotation
        {
            get
            {
                return _LocalRotation;
            }
            set
            {
                Quaternion previous = this.LocalRotation;
                this.SetProperty(ref _LocalRotation, value);
                this.Raise(this.LocalRotationChanged, value, previous);
            }
        }

        [IgnoreDataMember]
        public Quaternion Rotation
        {
            get
            {
                Transform parent = this.Parent;
                return (parent != null) ? parent.Rotation * this.LocalRotation : this.LocalRotation;
            }
            set
            {
                Quaternion previous = this.Rotation;
                //throw new NotImplementedException();
                //this.SetProperty(ref _Rotation, value);
                this.Raise(this.RotationChanged, value, previous);
            }
        }

        public Matrix RotationMatrix
        {
            get
            {
                return Matrix.RotationQuaternion(this.Rotation);
            }
        }

        private Vector3 _LocalScale = Vector3.One;

        [DataMember]
        public Vector3 LocalScale
        {
            get
            {
                return _LocalScale;
            }
            set
            {
                Vector3 previous = this.LocalScale;
                this.SetProperty(ref _LocalScale, value);
                this.Raise(this.LocalScalingChanged, previous, previous);
            }
        }

        [IgnoreDataMember]
        public Vector3 Scale
        {
            get
            {
                Transform parent = this.Parent;
                return (parent != null) ? parent.Scale * this.LocalScale : this.LocalScale;
            }
            set
            {
                Vector3 previous = this.Scale;
                //throw new NotImplementedException();
                //this.SetProperty(ref _Scale, value);
                this.Raise(this.ScalingChanged, value, previous);
            }
        }

        public Matrix ScaleMatrix
        {
            get
            {
                return Matrix.Scaling(this.Scale);
            }
        }

        public Matrix ModelMatrix
        {
            get
            {
                return this.PositionMatrix * this.RotationMatrix * this.ScaleMatrix;
            }
        }

        public Transform()
        {
            logger.Debug("Initializing a new transform.");

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

        public Transform(Vector3 localPosition, Quaternion localRotation, Vector3 localScale) : this(null, localPosition, localRotation, localScale) { }

        public Transform(Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
            : this()
        {
            this.LocalPosition = localPosition;
            this.LocalRotation = localRotation;
            this.LocalScale = localScale;
            this.Parent = parent;
        }

        protected override void OnReset()
        {
            this.LocalPosition = Vector3.Zero;
            this.LocalRotation = Quaternion.Identity;
            this.LocalScale = Vector3.One;
        }
    }
}
