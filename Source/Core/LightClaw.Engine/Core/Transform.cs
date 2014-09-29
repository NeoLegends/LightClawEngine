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
    /// <summary>
    /// Contains an object's position, rotation and scaling relative to its parent and in world space.
    /// </summary>
    [DataContract(IsReference = true)]
    [NonRemovable, Solitary(typeof(Transform), "A GameObject cannot have multiple transformations at the same time.")]
    public class Transform : Component, INotifyCollectionChanged
    {
        /// <summary>
        /// Gets a <see cref="Transform"/> with default values.
        /// </summary>
        public static Transform Null
        {
            get
            {
                return new Transform();
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="Transform"/> has changed and the matrices need to be recalculated.
        /// </summary>
        private bool isDirty = true; // Transform values need to be calculated on start, even if we're not technically dirty.

        /// <summary>
        /// Notifies about changes in the children collection.
        /// </summary>
        public event NotifyCollectionChangedEventHandler ChildrenChanged;

        /// <summary>
        /// Notifies about changes in the children collection.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Notifies about changes in the local position.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Vector3>> LocalPositionChanged;

        /// <summary>
        /// Notifies about changes in the local rotation.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Quaternion>> LocalRotationChanged;

        /// <summary>
        /// Notifies about changes in the local scaling.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Vector3>> LocalScalingChanged;

        /// <summary>
        /// Notifies about changes in the absolute position.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Vector3>> PositionChanged;

        /// <summary>
        /// Notifies about changes of the parent.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Transform>> ParentChanged;

        /// <summary>
        /// Notifies about changes in the absolute rotation.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Quaternion>> RotationChanged;

        /// <summary>
        /// Notifies about changes in the absolute scaling.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Vector3>> ScalingChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private ObservableCollection<Transform> _Childs = new ObservableCollection<Transform>();
        
        /// <summary>
        /// The logically attached childs.
        /// </summary>
        [DataMember]
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

        /// <summary>
        /// Backing field.
        /// </summary>
        private Transform _Parent;

        /// <summary>
        /// The parent (if any).
        /// </summary>
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
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private Vector3 _LocalPosition = Vector3.Zero;

        /// <summary>
        /// The <see cref="GameObject"/>'s local position in relation to the parent's position.
        /// </summary>
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
                this.isDirty = true;
            }
        }

        /// <summary>
        /// The <see cref="GameObject"/>'s absolute position in world space.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 Position
        {
            get
            {
                return this.GetParentPosition() + this.LocalPosition;
            }
            set
            {
                Vector3 previous = this.Position;
                Vector3 newValue = value - this.GetParentPosition();

                this.SetProperty(ref _LocalPosition, newValue);
                this.Raise(this.PositionChanged, newValue, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private Matrix _PositionMatrix;

        /// <summary>
        /// The absolute position as translation <see cref="Matrix"/>.
        /// </summary>
        [IgnoreDataMember]
        public Matrix PositionMatrix
        {
            get
            {
                return (this.isDirty) ? (_PositionMatrix = Matrix.Translation(this.Position)) : _PositionMatrix;
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private Quaternion _LocalRotation = Quaternion.Identity;

        /// <summary>
        /// The <see cref="GameObject"/>'s local rotation in relation to the parent's rotation.
        /// </summary>
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
                this.isDirty = true;
            }
        }

        /// <summary>
        /// The <see cref="GameObject"/>'s absolute rotation in world space.
        /// </summary>
        [IgnoreDataMember]
        public Quaternion Rotation
        {
            get
            {
                return this.GetParentRotation() * this.LocalRotation;
            }
            set
            {
                Quaternion previous = this.Rotation;
                Quaternion newValue = Quaternion.Conjugate(this.GetParentRotation()) * value;

                this.SetProperty(ref _LocalRotation, newValue);
                this.Raise(this.RotationChanged, newValue, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private Matrix _RotationMatrix;

        /// <summary>
        /// The absolute rotation in world space as rotation <see cref="Matrix"/>.
        /// </summary>
        [IgnoreDataMember]
        public Matrix RotationMatrix
        {
            get
            {
                return (this.isDirty) ? (_RotationMatrix = Matrix.RotationQuaternion(this.Rotation)) : _RotationMatrix; ;
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private Vector3 _LocalScaling = Vector3.One;

        /// <summary>
        /// The <see cref="GameObject"/>'s scaling relative to the parent's scaling.
        /// </summary>
        [DataMember]
        public Vector3 LocalScaling
        {
            get
            {
                return _LocalScaling;
            }
            set
            {
                Vector3 previous = this.LocalScaling;
                this.SetProperty(ref _LocalScaling, value);
                this.Raise(this.LocalScalingChanged, previous, previous);
                this.isDirty = true;
            }
        }

        /// <summary>
        /// The <see cref="GameObject"/>'s absolute scaling in world space.
        /// </summary>
        [IgnoreDataMember]
        public Vector3 Scaling
        {
            get
            {
                return this.GetParentScale() * this.LocalScaling;
            }
            set
            {
                Vector3 previous = this.Scaling;
                Vector3 newValue = Vector3.One;

                this.SetProperty(ref _LocalScaling, newValue);
                this.Raise(this.ScalingChanged, value, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private Matrix _ScalingMatrix;

        /// <summary>
        /// The absolute scaling in world space as scaling <see cref="Matrix"/>.
        /// </summary>
        [IgnoreDataMember]
        public Matrix ScalingMatrix
        {
            get
            {
                return (this.isDirty) ? (_ScalingMatrix = Matrix.Scaling(this.Scaling)) : _ScalingMatrix;
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private Matrix _ModelMatrix;

        /// <summary>
        /// The position-, rotation, and scaling matrices combined as model / world-<see cref="Matrix"/> (up to you how you call it ;)).
        /// </summary>
        /// <remarks>
        /// <list type="number">
        ///     <listheader>
        ///         <term>Order of Operations</term>
        ///         <description>Contains the order in which the transformation matrices are applied to form the final model / world-matrix.</description>
        ///     </listheader>
        ///     <item>
        ///         <description>Scaling</description>
        ///     </item>
        ///     <item>
        ///         <description>Rotation</description>
        ///     </item>
        ///     <item>
        ///         <description>Position</description>
        ///     </item>
        /// </list>
        /// </remarks>
        [IgnoreDataMember]
        public Matrix ModelMatrix
        {
            get
            {
                return (this.isDirty) ? (_ModelMatrix = this.PositionMatrix * this.RotationMatrix * this.ScalingMatrix) : _ModelMatrix;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Transform"/>.
        /// </summary>
        public Transform()
        {
            this.Childs.CollectionChanged += (s, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Move)
                {
                    foreach (Transform t in e.OldItems)
                    {
                        t.Parent = null;
                    }
                    foreach (Transform t in e.NewItems)
                    {
                        t.Parent = this;
                    }
                }

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

        /// <summary>
        /// Initializes a new <see cref="Transform"/> setting position, rotation and scaling.
        /// </summary>
        /// <param name="localPosition">The <see cref="GameObject"/>'s local position in relation to the parent's position.</param>
        /// <param name="localRotation">The <see cref="GameObject"/>'s local rotation in relation to the parent's rotation.</param>
        /// <param name="localScale">The <see cref="GameObject"/>'s scaling relative to the parent's scaling.</param>
        public Transform(Vector3 localPosition, Quaternion localRotation, Vector3 localScale) : this(null, localPosition, localRotation, localScale) { }

        /// <summary>
        /// Initializes a new <see cref="Transform"/> the parent, position, rotation and scaling.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="localPosition">The <see cref="GameObject"/>'s local position in relation to the parent's position.</param>
        /// <param name="localRotation">The <see cref="GameObject"/>'s local rotation in relation to the parent's rotation.</param>
        /// <param name="localScale">The <see cref="GameObject"/>'s scaling relative to the parent's scaling.</param>
        public Transform(Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
            : this()
        {
            this.LocalPosition = localPosition;
            this.LocalRotation = localRotation;
            this.LocalScaling = localScale;
            this.Parent = parent;
        }

        /// <summary>
        /// Rotates the <see cref="GameObject"/> by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to rotate by.</param>
        public void Rotate(Quaternion amount)
        {
            this.Rotation = amount * this.Rotation;
        }

        /// <summary>
        /// Scales the <see cref="GameObject"/> by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to scale by.</param>
        public void Scale(Vector3 amount)
        {
            this.Scaling *= amount;
        }

        /// <summary>
        /// Translates the <see cref="GameObject"/> by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to translate by.</param>
        public void Translate(Vector3 amount)
        {
            this.Position += amount;
        }

        /// <summary>
        /// Resets local position, scaling and rotation.
        /// </summary>
        protected override void OnReset()
        {
            this.LocalPosition = Vector3.Zero;
            this.LocalRotation = Quaternion.Identity;
            this.LocalScaling = Vector3.One;
        }

        /// <summary>
        /// Safely (thread-safe / null-check) gets the parent's rotation and returns <see cref="Quaternion.Identity"/> if it could not be obtained.
        /// </summary>
        /// <returns><see cref="Quaternion.Identity"/> if <see cref="P:Parent"/> was null, otherwise <see cref="P:Parent"/>'s rotation.</returns>
        protected Quaternion GetParentRotation()
        {
            Transform parent = this.Parent;
            return (parent != null) ? parent.Rotation : Quaternion.Identity;
        }

        /// <summary>
        /// Safely (thread-safe / null-check) gets the parent's position and returns <see cref="Vector3.Zero"/> if it could not be obtained.
        /// </summary>
        /// <returns><see cref="Vector3.Zero"/> if <see cref="P:Parent"/> was null, otherwise <see cref="P:Parent"/>'s position.</returns>
        protected Vector3 GetParentPosition()
        {
            Transform parent = this.Parent;
            return (parent != null) ? parent.Position : Vector3.Zero;
        }

        /// <summary>
        /// Safely (thread-safe / null-check) gets the parent's scaling and returns <see cref="Vector3.One"/> if it could not be obtained.
        /// </summary>
        /// <returns><see cref="Vector3.One"/> if <see cref="P:Parent"/> was null, otherwise <see cref="P:Parent"/>'s scaling.</returns>
        protected Vector3 GetParentScale()
        {
            Transform parent = this.Parent;
            return (parent != null) ? parent.Scaling : Vector3.One;
        }
    }
}
