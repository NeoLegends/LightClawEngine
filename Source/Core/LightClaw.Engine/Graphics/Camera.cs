using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public class Camera : Component
    {
        public event EventHandler<ValueChangedEventArgs<double>> FoVChanged;

        public event EventHandler<ValueChangedEventArgs<int>> IsoChanged;

        public event EventHandler<ValueChangedEventArgs<double>> ZoomChanged;

        private double _FoV;

        [DataMember]
        public double FoV
        {
            get
            {
                return _FoV;
            }
            set
            {
                double previous = this.FoV;
                this.SetProperty(ref _FoV, value);
                this.Raise(this.FoVChanged, value, previous);
            }
        }

        private int _Iso;

        [DataMember]
        public int Iso
        {
            get
            {
                return _Iso;
            }
            set
            {
                int previous = this.Iso;
                this.SetProperty(ref _Iso, value);
                this.Raise(this.IsoChanged, value, previous);
            }
        }

        private double _Zoom;

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
