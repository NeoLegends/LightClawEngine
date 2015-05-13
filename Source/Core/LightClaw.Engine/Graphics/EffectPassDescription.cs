using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.IO;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public struct EffectPassDescription : ICloneable, IEquatable<EffectPassDescription>
    {
        [IgnoreDataMember]
        public ResourceString[] Array
        {
            get
            {
                return new[] { this.VertexShader, this.TessControlShader, this.TessEvalShader, this.GeometryShader, this.FragmentShader };
            }
        }

        [DataMember]
        public ResourceString VertexShader { get; private set; }

        [DataMember]
        public ResourceString TessControlShader { get; private set; }

        [DataMember]
        public ResourceString TessEvalShader { get; private set; }

        [DataMember]
        public ResourceString GeometryShader { get; private set; }

        [DataMember]
        public ResourceString FragmentShader { get; private set; }

        public EffectPassDescription(ResourceString vertex, ResourceString fragment) : this(vertex, null, null, null, fragment) { }

        public EffectPassDescription(ResourceString vertex, ResourceString tessControl, ResourceString tessEval, ResourceString geom, ResourceString fragment)
            : this()
        {
            this.VertexShader = vertex;
            this.TessControlShader = tessControl;
            this.TessEvalShader = tessEval;
            this.GeometryShader = geom;
            this.FragmentShader = fragment;
        }

        public object Clone()
        {
            return new EffectPassDescription(this.VertexShader, this.TessControlShader, this.TessEvalShader, this.GeometryShader, this.FragmentShader);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is EffectPassDescription) ? this.Equals((EffectPassDescription)obj) : false;
        }

        public bool Equals(EffectPassDescription other)
        {
            return (this.VertexShader == other.VertexShader) && (this.TessControlShader == other.TessControlShader) &&
                   (this.TessEvalShader == other.TessEvalShader) && (this.GeometryShader == other.GeometryShader) &&
                   (this.FragmentShader == other.FragmentShader);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 29;
                hash = hash * 486187739 + ((this.VertexShader != null) ? this.VertexShader.GetHashCode() : 0);
                hash = hash * 486187739 + ((this.TessControlShader != null) ? this.TessControlShader.GetHashCode() : 0);
                hash = hash * 486187739 + ((this.TessEvalShader != null) ? this.TessEvalShader.GetHashCode() : 0);
                hash = hash * 486187739 + ((this.GeometryShader != null) ? this.GeometryShader.GetHashCode() : 0);
                hash = hash * 486187739 + ((this.FragmentShader != null) ? this.FragmentShader.GetHashCode() : 0);
                return hash;
            }
        }

        public static bool operator ==(EffectPassDescription left, EffectPassDescription right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EffectPassDescription left, EffectPassDescription right)
        {
            return !(left == right);
        }
    }
}
