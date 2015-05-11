using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a camera.
    /// </summary>
    [DataContract]
    public class Camera : Component
    {
        private volatile bool isDirty = true;

        /// <summary>
        /// Notifies about changes in the <see cref="Camera"/>s field of view.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<float>> FoVChanged;

        /// <summary>
        /// Notifies about changes in the height of the rendered frame.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<int>> HeightChanged;

        /// <summary>
        /// Notifies about changes in the width of the rendered frame.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<int>> WidthChanged;

        /// <summary>
        /// Notifies about changes in the <see cref="Camera"/>s zoom level.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<float>> ZoomChanged;

        private float _FoV = 90.0f;

        /// <summary>
        /// The <see cref="Camera"/>s vertical field of view (in degrees).
        /// </summary>
        [DataMember]
        public float FoV
        {
            get
            {
                Contract.Ensures(Contract.Result<float>() > 0.0f);

                return _FoV;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value > 0.0f);

                float previous = this.FoV;
                value = value % 360.0f;
                this.SetProperty(ref _FoV, value);
                this.Raise(this.FoVChanged, value, previous);
                this.isDirty = true;
            }
        }

        private int _Height;

        /// <summary>
        /// The height of the rendered frame.
        /// </summary>
        [DataMember]
        public int Height
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _Height;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                int previous = this.Width;
                this.SetProperty(ref _Height, value);
                this.Raise(this.HeightChanged, value, previous);
                this.isDirty = true;
            }
        }

        private Matrix4 _ProjectionMatrix;

        /// <summary>
        /// Gets a <see cref="Matrix4"/> that represents the projection matrix of the <see cref="Camera"/>.
        /// </summary>
        public Matrix4 ProjectionMatrix
        {
            get
            {
                if (!this.isDirty)
                {
                    return _ProjectionMatrix;
                }
                else
                {
                    Matrix4 result = _ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                        this.FoV,
                        (float)this.Width / (float)this.Height,
                        0.01f,
                        1000.0f
                    );
                    this.isDirty = false;
                    return result;
                }
            }
        }

        private int _Width;

        /// <summary>
        /// The width of the rendered frame.
        /// </summary>
        [DataMember]
        public int Width
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _Width;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                int previous = this.Width;
                this.SetProperty(ref _Width, value);
                this.Raise(this.WidthChanged, value, previous);
                this.isDirty = true;
            }
        }

        private float _Zoom;

        /// <summary>
        /// Gets the <see cref="Camera"/>s zoom level.
        /// </summary>
        [DataMember]
        public float Zoom
        {
            get
            {
                Contract.Ensures(Contract.Result<float>() > 0.0f);

                return _Zoom;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value > 0.0f);

                float previous = this.Zoom;
                this.SetProperty(ref _Zoom, value);
                this.Raise(this.ZoomChanged, value, previous);
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Camera"/>.
        /// </summary>
        public Camera() { }

        /// <summary>
        /// Initializes a new <see cref="Camera"/>.
        /// </summary>
        /// <param name="width">The width of the rendered frame.</param>
        /// <param name="height">The height of the rendered frame.</param>
        /// <param name="fov">The vertical field of view (in degrees).</param>
        public Camera(int width, int height, float fov) 
            : this(width, height, fov, 1.0f)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(fov > 0.0f);
        }

        /// <summary>
        /// Initializes a new <see cref="Camera"/>.
        /// </summary>
        /// <param name="width">The width of the rendered frame.</param>
        /// <param name="height">The height of the rendered frame.</param>
        /// <param name="fov">The vertical field of view (in degrees).</param>
        /// <param name="zoom">The <see cref="Camera"/>s zoom level.</param>
        public Camera(int width, int height, float fov, float zoom)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(fov > 0.0f);
            Contract.Requires<ArgumentOutOfRangeException>(zoom > 0.0f);

            this.FoV = fov;
            this.Height = height;
            this.Width = width;
            this.Zoom = zoom;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._FoV > 0.0f);
            Contract.Invariant(this._Height >= 0);
            Contract.Invariant(this._Width >= 0);
            Contract.Invariant(this._Zoom > 0.0f);
        }
    }
}
