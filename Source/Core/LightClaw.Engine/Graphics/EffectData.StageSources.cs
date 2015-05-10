using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using Newtonsoft.Json;

namespace LightClaw.Engine.Graphics
{
    public sealed partial class EffectData
    {
        [DataContract]
        public sealed class StageSources : ICloneable, IEquatable<StageSources>
        {
            [DataMember]
            public StageData Fragment { get; private set; }

            [DataMember]
            public StageData TessControl { get; private set; }

            [DataMember]
            public StageData TessEval { get; private set; }

            [DataMember]
            public StageData Geometry { get; private set; }

            [DataMember]
            public StageData Vertex { get; private set; }

            [ContractVerification(false)]
            private StageSources() { }

            public StageSources(StageData vertex, StageData fragment)
                : this(vertex, null, null, null, fragment)
            {
                Contract.Requires<ArgumentNullException>(vertex != null);
                Contract.Requires<ArgumentNullException>(fragment != null);
            }

            public StageSources(
                    StageData vertex,
                    StageData tessControl,
                    StageData tessEval,
                    StageData geometry,
                    StageData fragment
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
                return new StageSources(this.Vertex, this.TessControl, this.TessEval, this.Geometry, this.Fragment);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;
                if (ReferenceEquals(obj, this))
                    return true;

                return this.Equals(obj as StageSources);
            }

            public bool Equals(StageSources other)
            {
                if (ReferenceEquals(other, null))
                    return false;
                if (ReferenceEquals(other, this))
                    return true;

                return (this.Fragment == other.Fragment) && (this.Vertex == other.Vertex) &&
                       (this.TessControl == other.TessControl) && (this.TessEval == other.TessEval) &&
                       (this.Geometry == other.Geometry);
            }

            public override int GetHashCode()
            {
                return HashF.GetHashCode(this.Fragment, this.Vertex, this.TessControl, this.TessEval, this.Geometry);
            }

            public static bool operator ==(StageSources left, StageSources right)
            {
                if (ReferenceEquals(left, right))
                    return true;
                if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                    return false;

                return left.Equals(right);
            }

            public static bool operator !=(StageSources left, StageSources right)
            {
                return !(left == right);
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.Vertex != null);
                Contract.Invariant(this.Fragment != null);
            }
        }
    }
}
