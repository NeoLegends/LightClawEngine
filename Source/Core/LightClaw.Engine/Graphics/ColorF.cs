using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    [StructureInformation(4, 4, true)]
    public struct ColorF
    {
        [DataMember]
        public float R { get; private set; }

        [DataMember]
        public float G { get; private set; }

        [DataMember]
        public float B { get; private set; }

        [DataMember]
        public float A { get; private set; }

        public ColorF(float value)
            : this(value, value, value, value)
        {
            Contract.Requires<ArgumentOutOfRangeException>(value >= 0.0f && value <= 1.0f);
        }

        public ColorF(Color color) : this(color.R, color.G, color.B, color.A) { }

        public ColorF(byte r, byte g, byte b, byte a) : this(Color.ToFloat(r), Color.ToFloat(g), Color.ToFloat(b), Color.ToFloat(a)) { }

        public ColorF(float r, float g, float b, float a)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(r >= 0.0f && r <= 1.0f);
            Contract.Requires<ArgumentOutOfRangeException>(g >= 0.0f && g <= 1.0f);
            Contract.Requires<ArgumentOutOfRangeException>(b >= 0.0f && b <= 1.0f);
            Contract.Requires<ArgumentOutOfRangeException>(a >= 0.0f && a <= 1.0f);

            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public static implicit operator ColorF(Color color)
        {
            return new ColorF(color);
        }
    }
}
