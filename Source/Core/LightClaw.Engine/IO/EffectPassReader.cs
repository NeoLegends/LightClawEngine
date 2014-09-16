using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;

namespace LightClaw.Engine.IO
{
    public class EffectPassReader : IContentReader
    {
        public Task<object> ReadAsync(IContentManager contentManager, ResourceString resourceString, Stream assetStream, Type assetType, object parameter)
        {
            if (typeof(Effect).IsAssignableFrom(assetType))
            {
                return Task.FromResult((object)null);
            }
            else
            {
                return Task.FromResult((object)null);
            }
        }

        [DataContract]
        public struct EffectData : ICloneable, IEquatable<EffectData>
        {
            private EffectAttributeDescription[] _AttributeDescriptions;

            [DataMember]
            public EffectAttributeDescription[] AttributeDescriptions
            {
                get
                {
                    return (_AttributeDescriptions ?? new EffectAttributeDescription[] { }).ToArray();
                }
                private set
                {
                    Contract.Requires<ArgumentNullException>(value != null);

                    _AttributeDescriptions = value;
                }
            }

            private DataEffectUniformDescription[] _DataUniformDescriptions;

            [DataMember]
            public DataEffectUniformDescription[] DataUniformDescriptions
            {
                get
                {
                    return (_DataUniformDescriptions ?? new DataEffectUniformDescription[] { }).ToArray();
                }
                private set
                {
                    Contract.Requires<ArgumentNullException>(value != null);

                    _DataUniformDescriptions = value;
                }
            }

            [DataMember]
            public string Name { get; private set; }


            private SamplerEffectUniformDescription[] _SamplerDescriptions;

            [DataMember]
            public SamplerEffectUniformDescription[] SamplerDescriptions
            {
                get
                {
                    return (_SamplerDescriptions ?? new SamplerEffectUniformDescription[] { }).ToArray();
                }
                private set
                {
                    Contract.Requires<ArgumentNullException>(value != null);

                    _SamplerDescriptions = value;
                }
            }

            [DataMember]
            public EffectSource Source { get; private set; }

            public EffectData(
                    string third, 
                    IEnumerable<EffectAttributeDescription> attributeDescriptions, 
                    IEnumerable<DataEffectUniformDescription> dataUniformDescriptions,
                    IEnumerable<SamplerEffectUniformDescription> samplerDescriptions, 
                    EffectSource source
                )
                : this()
            {
                Contract.Requires<ArgumentNullException>(attributeDescriptions != null);
                Contract.Requires<ArgumentNullException>(dataUniformDescriptions != null);
                Contract.Requires<ArgumentNullException>(samplerDescriptions != null);

                this.AttributeDescriptions = attributeDescriptions.ToArray();
                this.DataUniformDescriptions = dataUniformDescriptions.ToArray();
                this.Name = third;
                this.SamplerDescriptions = samplerDescriptions.ToArray();
                this.Source = source;
            }

            public object Clone()
            {
                return new EffectData(this.Name, this.AttributeDescriptions, this.DataUniformDescriptions, this.SamplerDescriptions, this.Source);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;

                return (obj is EffectData) ? this.Equals((EffectData)obj) : false;
            }

            public override bool Equals(EffectData other)
            {
                return (this.AttributeDescriptions == other.AttributeDescriptions) && (this.DataUniformDescriptions == other.DataUniformDescriptions) &&
                       (this.Name == other.Name) && (this.SamplerDescriptions == other.SamplerDescriptions) &&
                       (this.Source == other.Source);
            }

            public override int GetHashCode()
            {
                return HashF.GetHashCode(
                    HashF.GetHashCode(this.AttributeDescriptions),
                    HashF.GetHashCode(this.DataUniformDescriptions),
                    HashF.GetHashCode(this.SamplerDescriptions), 
                    this.Name, 
                    this.Source
                );
            }

            public static bool operator ==(EffectData left, EffectData right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(EffectData left, EffectData right)
            {
                return !(left == right);
            }
        }

        [DataContract]
        public struct EffectAttributeDescription : ICloneable, IEquatable<EffectAttributeDescription>
        {
            [DataMember]
            public int Location { get; private set; }

            [DataMember]
            public string Name { get; private set; }

            public EffectAttributeDescription(string name, int location)
                : this()
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
                Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

                this.Location = location;
                this.Name = name;
            }

            public object Clone()
            {
                return new EffectAttributeDescription(this.Name, this.Location);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;

                return (obj is EffectAttributeDescription) ? this.Equals((EffectAttributeDescription)obj) : false;
            }

            public override bool Equals(EffectAttributeDescription other)
            {
                return (this.Location == other.Location) && (this.Name == other.Name);
            }

            public override int GetHashCode()
            {
                return HashF.GetHashCode(this.Location, this.Name);
            }

            public static bool operator ==(EffectAttributeDescription left, EffectAttributeDescription right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(EffectAttributeDescription left, EffectAttributeDescription right)
            {
                return !(left == right);
            }
        }

        [DataContract]
        public struct EffectSource : ICloneable, IEquatable<EffectSource>
        {
            [DataMember]
            public ResourceString Fragment { get; private set; }

            [DataMember]
            public ResourceString Geometry { get; private set; }

            [DataMember]
            public ResourceString TessControl { get; private set; }

            [DataMember]
            public ResourceString TessEval { get; private set; }

            [DataMember]
            public ResourceString Vertex { get; private set; }

            public EffectSource(ResourceString fragment, ResourceString vertex)
                : this(fragment, null, null, null, vertex)
            {
                Contract.Requires<ArgumentNullException>(fragment != null);
                Contract.Requires<ArgumentNullException>(vertex != null);
            }

            public EffectSource(ResourceString fragment, ResourceString geometry, ResourceString tessControl, ResourceString tessEval, ResourceString vertex)
                : this()
            {
                Contract.Requires<ArgumentNullException>(fragment != null);
                Contract.Requires<ArgumentNullException>(vertex != null);

                this.Fragment = fragment;
                this.Geometry = geometry;
                this.TessControl = tessControl;
                this.TessEval = tessEval;
                this.Vertex = vertex;
            }

            public object Clone()
            {
                return new EffectSource(this.Fragment, this.Geometry, this.TessControl, this.TessEval, this.Vertex);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;

                return (obj is EffectSource) ? this.Equals((EffectSource)obj) : false;
            }

            public override bool Equals(EffectSource other)
            {
                return (this.Fragment == other.Fragment) && (this.Geometry == other.Geometry) &&
                       (this.TessControl == other.TessControl) && (this.TessEval == other.TessEval) &&
                       (this.Vertex == other.Vertex);
            }

            public override int GetHashCode()
            {
                return HashF.GetHashCode(this.Fragment, this.Geometry, this.TessControl, this.TessEval, this.Vertex);
            }

            public static bool operator ==(EffectSource left, EffectSource right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(EffectSource left, EffectSource right)
            {
                return !(left == right);
            }
        }

        [DataContract]
        public struct DataEffectUniformDescription : ICloneable, IEquatable<DataEffectUniformDescription>
        {
            [DataMember]
            public int Location { get; private set; }

            [DataMember]
            public string Name { get; private set; }

            public DataEffectUniformDescription(string name, int location)
                : this()
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
                Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

                this.Name = name;
                this.Location = location;
            }

            public object Clone()
            {
                return new DataEffectUniformDescription(this.Name, this.Location);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;

                return (obj is DataEffectUniformDescription) ? this.Equals((DataEffectUniformDescription)obj) : false;
            }

            public override bool Equals(DataEffectUniformDescription other)
            {
                return (this.Name == other.Name) && (this.Location == other.Location);
            }

            public override int GetHashCode()
            {
                return HashF.GetHashCode(this.Name, this.Location);
            }

            public static bool operator ==(DataEffectUniformDescription left, DataEffectUniformDescription right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(DataEffectUniformDescription left, DataEffectUniformDescription right)
            {
                return !(left == right);
            }
        }

        [DataContract]
        public struct SamplerEffectUniformDescription : ICloneable, IEquatable<SamplerEffectUniformDescription>
        {
            [DataMember]
            public string Name { get; private set; }

            [DataMember]
            public int TextureUnit { get; private set; }

            public SamplerEffectUniformDescription(string name, int location)
                : this()
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
                Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

                this.Name = name;
                this.TextureUnit = location;
            }

            public object Clone()
            {
                return new SamplerEffectUniformDescription(this.Name, this.TextureUnit);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;

                return (obj is SamplerEffectUniformDescription) ? this.Equals((SamplerEffectUniformDescription)obj) : false;
            }

            public override bool Equals(SamplerEffectUniformDescription other)
            {
                return (this.Name == other.Name) && (this.TextureUnit == other.TextureUnit);
            }

            public override int GetHashCode()
            {
                return HashF.GetHashCode(this.Name, this.TextureUnit);
            }

            public static bool operator ==(SamplerEffectUniformDescription left, SamplerEffectUniformDescription right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SamplerEffectUniformDescription left, SamplerEffectUniformDescription right)
            {
                return !(left == right);
            }
        }
    }
}
