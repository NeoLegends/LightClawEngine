using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public sealed partial class EffectData : ICloneable, IEquatable<EffectData>
    {
        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public StageSources Sources { get; private set; }

        [DataMember]
        public ImmutableArray<string> Uniforms { get; private set; }

        private EffectData() { }

        public EffectData(StageSources sources, IEnumerable<string> uniforms)
            : this(null, sources, uniforms)
        {
            Contract.Requires<ArgumentNullException>(sources != null);
            Contract.Requires<ArgumentNullException>(uniforms != null);
        }

        public EffectData(string name, StageSources sources, IEnumerable<string> uniforms)
        {
            Contract.Requires<ArgumentNullException>(sources != null);
            Contract.Requires<ArgumentNullException>(uniforms != null);

            this.Name = name;
            this.Sources = sources;
            this.Uniforms = uniforms.ToImmutableArray();
        }

        public object Clone()
        {
            return new EffectData(this.Name, this.Sources, this.Uniforms);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (ReferenceEquals(obj, this))
                return true;

            EffectData data = obj as EffectData;
            return (data != null) ? this.Equals(data) : false;
        }

        public bool Equals(EffectData other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(other, this))
                return true;

            return (this.Name == other.Name) && (this.Sources == other.Sources) && (this.Uniforms.SequenceEqual(other.Uniforms));
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(
                this.Sources,
                this.Name,
                HashF.GetHashCode(this.Uniforms)
            );
        }

        public static bool operator ==(EffectData left, EffectData right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(EffectData left, EffectData right)
        {
            return !(left == right);
        }
        
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Sources != null);
            Contract.Invariant(this.Uniforms != null);
        }
    }
}
