using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a camera.
    /// </summary>
    [DataContract]
    public class Camera : Component
    {
        /// <summary>
        /// Notifies about changes in the <see cref="Camera"/>'s field of view.
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
        /// Notifies about changes in the <see cref="Camera"/>'s zoom level.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<double>> ZoomChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private float _FoV = 90.0f;

        /// <summary>
        /// The <see cref="Camera"/>'s horizontal field of view (in degrees).
        /// </summary>
        [DataMember]
        public float FoV
        {
            get
            {
                return _FoV;
            }
            set
            {
                float previous = this.FoV;
                value = value % 360.0f;
                this.SetProperty(ref _FoV, value);
                this.Raise(this.FoVChanged, value, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private int _Height;

        /// <summary>
        /// The height of the rendered frame.
        /// </summary>
        [DataMember]
        public int Height
        {
            get
            {
                return _Height;
            }
            set
            {
                int previous = this.Width;
                this.SetProperty(ref _Height, value);
                this.Raise(this.WidthChanged, value, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private int _Width;

        /// <summary>
        /// The width of the rendered frame.
        /// </summary>
        [DataMember]
        public int Width
        {
            get
            {
                return _Width;
            }
            set
            {
                int previous = this.Width;
                this.SetProperty(ref _Width, value);
                this.Raise(this.WidthChanged, value, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private double _Zoom;

        /// <summary>
        /// Gets the <see cref="Camera"/>'s zoom level.
        /// </summary>
        [DataMember]
        public double Zoom
        {
            get
            {
                return _Zoom;
            }
            set
            {
                double previous = this.Zoom;
                this.SetProperty(ref _Zoom, value);
                this.Raise(this.ZoomChanged, value, previous);
            }
        }
    }
}
