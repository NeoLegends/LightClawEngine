using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

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
        public static Transform Zero
        {
            get
            {
                return new Transform();
            }
        }

        /// <summary>
        /// Notifies about changes in the children collection.
        /// </summary>
        public event NotifyCollectionChangedEventHandler ChildrenChanged
        {
            add
            {
                this.CollectionChanged += value;
            }
            remove
            {
                this.CollectionChanged -= value;
            }
        }

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

        private DirtyFlags _DirtyFlags = DirtyFlags.All; // Transform values need to be calculated on start, even if we're not technically dirty.

        /// <summary>
        /// Indicates whether the <see cref="Transform"/> has changed and the matrices need to be recalculated.
        /// </summary>
        protected DirtyFlags Dirty
        {
            get
            {
                return _DirtyFlags;
            }
            set
            {
                this.SetProperty(ref _DirtyFlags, value);
            }
        }

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
                this.SetProperty(ref _Parent, value, this.ParentChanged);
                this.Dirty = DirtyFlags.All;
            }
        }

        private Vector3 _LocalPosition = Vector3.Zero;

        /// <summary>
        /// The <see cref="GameObject"/>s local position in relation to the parent's position.
        /// </summary>
        [DataMember]
        [Newtonsoft.Json.JsonConverter(typeof(MathConverter))]
        public Vector3 LocalPosition
        {
            get
            {
                return _LocalPosition;
            }
            set
            {
                this.SetProperty(ref _LocalPosition, value, this.LocalPositionChanged);
                this.Dirty |= DirtyFlags.Position;
            }
        }

        /// <summary>
        /// The <see cref="GameObject"/>s absolute position in world space.
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
                Vector3 newValue = value - this.GetParentPosition();

                this.SetProperty(ref _LocalPosition, newValue, this.PositionChanged);
            }
        }

        private Matrix4 _PositionMatrix;

        /// <summary>
        /// The absolute position as translation <see cref="Matrix"/>.
        /// </summary>
        [IgnoreDataMember]
        public Matrix4 PositionMatrix
        {
            get
            {
                if (!this.Dirty.HasFlag(DirtyFlags.Position))
                {
                    return _PositionMatrix;
                }
                else
                {
                    Matrix4 result = _PositionMatrix = Matrix4.CreateTranslation(this.Position);
                    this.Dirty &= ~DirtyFlags.Position;
                    return result;
                }
            }
        }

        private Quaternion _LocalRotation = Quaternion.Identity;

        /// <summary>
        /// The <see cref="GameObject"/>s local rotation in relation to the parent's rotation.
        /// </summary>
        [DataMember]
        [Newtonsoft.Json.JsonConverter(typeof(MathConverter))]
        public Quaternion LocalRotation
        {
            get
            {
                return _LocalRotation;
            }
            set
            {
                this.SetProperty(ref _LocalRotation, value, this.LocalRotationChanged);
                this.Dirty |= DirtyFlags.Rotation;
            }
        }

        /// <summary>
        /// The <see cref="GameObject"/>s absolute rotation in world space.
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
                Quaternion newValue = Quaternion.Conjugate(this.GetParentRotation()) * value;

                this.SetProperty(ref _LocalRotation, newValue, this.RotationChanged);
            }
        }

        private Matrix4 _RotationMatrix;

        /// <summary>
        /// The absolute rotation in world space as rotation <see cref="Matrix"/>.
        /// </summary>
        [IgnoreDataMember]
        public Matrix4 RotationMatrix
        {
            get
            {
                if (!this.Dirty.HasFlag(DirtyFlags.Rotation))
                {
                    return _RotationMatrix;
                }
                else
                {
                    Matrix4 result = _RotationMatrix = Matrix4.CreateFromQuaternion(this.Rotation);
                    this.Dirty &= ~DirtyFlags.Rotation;
                    return result;
                }
            }
        }

        private Vector3 _LocalScaling = Vector3.One;

        /// <summary>
        /// The <see cref="GameObject"/>s scaling relative to the parent's scaling.
        /// </summary>
        [DataMember]
        [Newtonsoft.Json.JsonConverter(typeof(MathConverter))]
        public Vector3 LocalScaling
        {
            get
            {
                return _LocalScaling;
            }
            set
            {
                this.SetProperty(ref _LocalScaling, value, this.LocalScalingChanged);
                this.Dirty |= DirtyFlags.Scaling;
            }
        }

        /// <summary>
        /// The <see cref="GameObject"/>s absolute scaling in world space.
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
                Vector3 newValue = Vector3.One;

                this.SetProperty(ref _LocalScaling, newValue, this.ScalingChanged);
            }
        }

        private Matrix4 _ScalingMatrix;

        /// <summary>
        /// The absolute scaling in world space as scaling <see cref="Matrix"/>.
        /// </summary>
        [IgnoreDataMember]
        public Matrix4 ScalingMatrix
        {
            get
            {
                if (!this.Dirty.HasFlag(DirtyFlags.Scaling))
                {
                    return _ScalingMatrix;
                }
                else
                {
                    Matrix4 scalingMatrix = _ScalingMatrix = Matrix4.CreateScale(this.Scaling);
                    this.Dirty &= ~DirtyFlags.Scaling;
                    return scalingMatrix;
                }
            }
        }

        private Matrix4 _ModelMatrix;

        /// <summary>
        /// The position-, rotation, and scaling matrices combined as model / world-<see cref="Matrix"/>.
        /// </summary>
        /// <remarks>
        /// <list type="number">
        ///     <listheader>
        ///         <term>Order of Operations</term> 
        ///         <description>
        ///             Contains the order in which the transformation matrices are applied to form the final model / world-matrix.
        ///         </description> 
        ///     </listheader>
        ///     <item>
        ///         <description>Scaling</description> 
        ///     </item> 
        ///     <item>
        ///         <description>Rotation</description> 
        ///     </item>
        ///     <item>
        ///         <description>Translation</description> 
        ///     </item> 
        /// </list>
        /// </remarks>
        [IgnoreDataMember]
        public Matrix4 ModelMatrix
        {
            get
            {
                if (this.Dirty == DirtyFlags.None)
                {
                    return _ModelMatrix;
                }
                else
                {
                    Matrix4 result = _ModelMatrix = this.PositionMatrix * this.RotationMatrix * this.ScalingMatrix;
                    this.Dirty &= ~(DirtyFlags.Position | DirtyFlags.Rotation | DirtyFlags.Scaling);
                    return result;
                }
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

                NotifyCollectionChangedEventHandler handler = this.CollectionChanged;
                if (handler != null)
                {
                    handler(s, e);
                }
            };
        }

        /// <summary>
        /// Initializes a new <see cref="Transform"/> setting position, rotation and scaling.
        /// </summary>
        /// <param name="localPosition">
        /// The <see cref="GameObject"/>s local position in relation to the parent's position.
        /// </param>
        /// <param name="localRotation">
        /// The <see cref="GameObject"/>s local rotation in relation to the parent's rotation.
        /// </param>
        /// <param name="localScale">The <see cref="GameObject"/>s scaling relative to the parent's scaling.</param>
        public Transform(Vector3 localPosition, Quaternion localRotation, Vector3 localScale) : this(null, localPosition, localRotation, localScale) { }

        /// <summary>
        /// Initializes a new <see cref="Transform"/> the parent, position, rotation and scaling.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="localPosition">
        /// The <see cref="GameObject"/>s local position in relation to the parent's position.
        /// </param>
        /// <param name="localRotation">
        /// The <see cref="GameObject"/>s local rotation in relation to the parent's rotation.
        /// </param>
        /// <param name="localScale">The <see cref="GameObject"/>s scaling relative to the parent's scaling.</param>
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
            this.Rotation *= amount;
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
        /// Safely (thread-safe / null-check) gets the parent's rotation and returns <see cref="Quaternion.Identity"/>
        /// if it could not be obtained.
        /// </summary>
        /// <returns>
        /// <see cref="Quaternion.Identity"/> if <see cref="P:Parent"/> was null, otherwise <see cref="P:Parent"/>s rotation.
        /// </returns>
        protected Quaternion GetParentRotation()
        {
            Transform parent = this.Parent;
            return (parent != null) ? parent.Rotation : Quaternion.Identity;
        }

        /// <summary>
        /// Safely (thread-safe / null-check) gets the parent's position and returns <see cref="Vector3.Zero"/> if it
        /// could not be obtained.
        /// </summary>
        /// <returns>
        /// <see cref="Vector3.Zero"/> if <see cref="P:Parent"/> was null, otherwise <see cref="P:Parent"/>s position.
        /// </returns>
        protected Vector3 GetParentPosition()
        {
            Transform parent = this.Parent;
            return (parent != null) ? parent.Position : Vector3.Zero;
        }

        /// <summary>
        /// Safely (thread-safe / null-check) gets the parent's scaling and returns <see cref="Vector3.One"/> if it
        /// could not be obtained.
        /// </summary>
        /// <returns>
        /// <see cref="Vector3.One"/> if <see cref="P:Parent"/> was null, otherwise <see cref="P:Parent"/>s scaling.
        /// </returns>
        protected Vector3 GetParentScale()
        {
            Transform parent = this.Parent;
            return (parent != null) ? parent.Scaling : Vector3.One;
        }

        /// <summary>
        /// Represents the <see cref="Transform"/>s dirty state.
        /// </summary>
        [Flags]
        protected enum DirtyFlags
        { 
            /// <summary>
            /// No properties are dirty and need to be recalculated.
            /// </summary>
            None = 0,

            /// <summary>
            /// Position is dirty.
            /// </summary>
            Position = 1,

            /// <summary>
            /// Rotation is dirty.
            /// </summary>
            Rotation = 2,

            /// <summary>
            /// Scaling is dirty.
            /// </summary>
            Scaling = 4,
            
            /// <summary>
            /// Everything is dirty.
            /// </summary>
            All = Position | Rotation | Scaling
        }
    }
}
