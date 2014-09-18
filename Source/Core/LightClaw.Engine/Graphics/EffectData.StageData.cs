using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;

namespace LightClaw.Engine.Graphics
{
    public sealed partial class EffectData
    {
        [DataContract]
        public sealed class StageData : ICloneable, IEquatable<StageData>
        {
            [DataMember]
            public ResourceString Source { get; private set; }

            [DataMember]
            public string VertexAttribute { get; private set; }

            [DataMember]
            public string TexCoordAttribute { get; private set; }

            [DataMember]
            public string NormalAttribute { get; private set; }

            [DataMember]
            public string TangentAttribute { get; private set; }

            [DataMember]
            public string BinormalAttribute { get; private set; }

            [DataMember]
            public string ColorAttribute { get; private set; }

            private StageData() { }

            public StageData(ResourceString source)
                : this(source, null, null, null, null, null, null)
            {
                Contract.Requires<ArgumentNullException>(source != null);
            }

            public StageData(
                    ResourceString source,
                    string vertexAttributeName,
                    string texCoordAttributeName,
                    string normalAttributeName,
                    string binormalAttributeName,
                    string tangentAttributeName,
                    string colorAttributeName
                )
            {
                Contract.Requires<ArgumentNullException>(source != null);

                this.Source = source;
                this.VertexAttribute = vertexAttributeName;
                this.TexCoordAttribute = texCoordAttributeName;
                this.NormalAttribute = normalAttributeName;
                this.TangentAttribute = tangentAttributeName;
                this.BinormalAttribute = binormalAttributeName;
                this.ColorAttribute = colorAttributeName;
            }

            public object Clone()
            {
                return new StageData(
                    this.Source,
                    this.VertexAttribute,
                    this.TexCoordAttribute,
                    this.NormalAttribute,
                    this.BinormalAttribute,
                    this.TangentAttribute,
                    this.ColorAttribute
                );
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;
                if (ReferenceEquals(obj, this))
                    return true;

                StageData data = obj as StageData;
                return (data != null) ? this.Equals(data) : false;
            }

            public bool Equals(StageData other)
            {
                if (ReferenceEquals(other, null))
                    return false;
                if (ReferenceEquals(other, this))
                    return true;

                return (this.Source == other.Source) && (this.VertexAttribute == other.VertexAttribute) &&
                       (this.TexCoordAttribute == other.TexCoordAttribute) && (this.NormalAttribute == other.NormalAttribute) &&
                       (this.TangentAttribute == other.TangentAttribute) && (this.BinormalAttribute == other.BinormalAttribute) &&
                       (this.ColorAttribute == other.ColorAttribute);
            }

            public override int GetHashCode()
            {
                return HashF.GetHashCode(
                    this.Source,
                    HashF.GetHashCode(
                        this.VertexAttribute,
                        this.TexCoordAttribute,
                        this.NormalAttribute,
                        this.TangentAttribute,
                        this.BinormalAttribute,
                        this.ColorAttribute
                    )
                );
            }

            public static bool operator ==(StageData left, StageData right)
            {
                if (ReferenceEquals(left, right))
                    return true;
                if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                    return false;

                return left.Equals(right);
            }

            public static bool operator !=(StageData left, StageData right)
            {
                return !(left == right);
            }
        }
    }
}
