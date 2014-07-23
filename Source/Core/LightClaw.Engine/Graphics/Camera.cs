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
                this.SetProperty(ref _FoV, value);
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
                this.SetProperty(ref _Zoom, value);
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
                this.SetProperty(ref _Iso, value);
            }
        }
    }
}
