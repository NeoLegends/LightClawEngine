using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    [Serializable, DataContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct BoneWeighting : ICloneable, IEquatable<BoneWeighting>
    {
        [DataMember]
        public float BoneId { get; private set; }

        [DataMember]
        public float Weighting { get; private set; }

        public BoneWeighting(float boneId, float weighting)
            : this()
        {
            this.BoneId = boneId;
            this.Weighting = weighting;
        }

        public object Clone()
        {
            return new BoneWeighting(this.BoneId, this.Weighting);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is BoneWeighting) ? this.Equals((BoneWeighting)obj) : false;
        }

        public bool Equals(BoneWeighting other)
        {
            return (this.BoneId == other.BoneId) && (this.Weighting == other.Weighting);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.BoneId, this.Weighting);
        }

        public static bool operator ==(BoneWeighting left, BoneWeighting right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BoneWeighting left, BoneWeighting right)
        {
            return !(left == right);
        }
    }
}
