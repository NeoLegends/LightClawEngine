using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [ProtoContract]
    [GameComponent]
    public class Camera : Component
    {
        private double _FoV;

        [ProtoMember(1)]
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

        [ProtoMember(2)]
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

        [ProtoMember(3)]
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
