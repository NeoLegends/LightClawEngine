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
    public sealed class EffectStageSources : ICloneable, IEquatable<EffectStageSources>
    {
        [DataMember]
        public EffectStageData Fragment { get; private set; }

        [DataMember]
        public EffectStageData TessControl { get; private set; }

        [DataMember]
        public EffectStageData TessEval { get; private set; }

        [DataMember]
        public EffectStageData Geometry { get; private set; }

        [DataMember]
        public EffectStageData Vertex { get; private set; }

        private EffectStageSources() { }

        public EffectStageSources(EffectStageData vertex, EffectStageData fragment)
            : this(vertex, null, null, null, fragment)
        {
            Contract.Requires<ArgumentNullException>(vertex != null);
            Contract.Requires<ArgumentNullException>(fragment != null);
        }

        public EffectStageSources(
                EffectStageData vertex, 
                EffectStageData tessControl, 
                EffectStageData tessEval, 
                EffectStageData geometry, 
                EffectStageData fragment
            )
        {
            Contract.Requires<ArgumentNullException>(vertex != null);
            Contract.Requires<ArgumentNullException>(fragment != null);

            this.Fragment = fragment;
            this.Vertex = vertex;
            this.TessControl = tessControl;
            this.TessEval = tessEval;
            this.Geometry = geometry;
        }

        public object Clone()
        {
            return new EffectStageSources(this.Vertex, this.TessControl, this.TessEval, this.Geometry, this.Fragment);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (ReferenceEquals(obj, this))
                return true;

            EffectStageSources sources = obj as EffectStageSources;
            return (sources != null) ? this.Equals(sources) : false;
        }

        public bool Equals(EffectStageSources other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return (this.Fragment == other.Fragment) && (this.Vertex == other.Vertex) &&
                   (this.TessControl == other.TessControl) && (this.TessEval == other.TessEval) &&
                   (this.Geometry == other.Geometry);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Fragment, this.Vertex, this.TessControl, this.TessEval, this.Geometry);
        }

        public static bool operator ==(EffectStageSources left, EffectStageSources right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(EffectStageSources left, EffectStageSources right)
        {
            return !(left == right);
        }
    }
}
