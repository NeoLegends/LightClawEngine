using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using Newtonsoft.Json;

namespace LightClaw.Engine.Graphics.OpenGL
{
    [Serializable, DataContract]
    [JsonConverter(typeof(VertexAttributeLocationConverter))]
    public struct VertexAttributeLocation : ICloneable, IComparable<VertexAttributeLocation>, IEquatable<int>, IEquatable<VertexAttributeLocation>
    {
        public static readonly VertexAttributeLocation Position = new VertexAttributeLocation(0);

        public static readonly VertexAttributeLocation Normals = new VertexAttributeLocation(1);

        public static readonly VertexAttributeLocation Binormals = new VertexAttributeLocation(2);

        public static readonly VertexAttributeLocation Tangent = new VertexAttributeLocation(3);

        public static readonly VertexAttributeLocation TexCoords = new VertexAttributeLocation(4);

        public static readonly VertexAttributeLocation Color = new VertexAttributeLocation(5);

        public static readonly VertexAttributeLocation BoneWeight0 = new VertexAttributeLocation(6);

        public static readonly VertexAttributeLocation BoneWeight1 = new VertexAttributeLocation(7);

        public static readonly VertexAttributeLocation BoneWeight2 = new VertexAttributeLocation(8);

        public static readonly VertexAttributeLocation BoneWeight3 = new VertexAttributeLocation(9);

        [DataMember]
        public int Location { get; private set; }

        public VertexAttributeLocation(int location)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

            this.Location = location;
        }

        public object Clone()
        {
            return new VertexAttributeLocation(this.Location);
        }

        public int CompareTo(VertexAttributeLocation other)
        {
            return this.Location.CompareTo(other.Location);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is VertexAttributeLocation) ? this.Equals((VertexAttributeLocation)obj) : false;
        }

        public bool Equals(int other)
        {
            return (this.Location == other);
        }

        public bool Equals(VertexAttributeLocation other)
        {
            return this.Equals(other.Location);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Location);
        }

        public override string ToString()
        {
            return this.Location.ToString();
        }

        public static bool operator <(VertexAttributeLocation left, VertexAttributeLocation right)
        {
            return (left.Location < right.Location);
        }

        public static bool operator >(VertexAttributeLocation left, VertexAttributeLocation right)
        {
            return (left.Location > right.Location);
        }

        public static bool operator <=(VertexAttributeLocation left, VertexAttributeLocation right)
        {
            return (left.Location <= right.Location);
        }

        public static bool operator >=(VertexAttributeLocation left, VertexAttributeLocation right)
        {
            return (left.Location >= right.Location);
        }

        public static bool operator ==(VertexAttributeLocation left, VertexAttributeLocation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexAttributeLocation left, VertexAttributeLocation right)
        {
            return !(left == right);
        }

        public static implicit operator VertexAttributeLocation(int location)
        {
            Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

            return new VertexAttributeLocation(location);
        }

        public static implicit operator int(VertexAttributeLocation location)
        {
            return location.Location;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Location >= 0);
        }

        public class VertexAttributeLocationConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(VertexAttributeLocation));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                int location = serializer.Deserialize<int>(reader);
                if (location < 0)
                {
                    throw new InvalidOperationException("The deserialized vertex attribute location was smaller than zero.");
                }
                return new VertexAttributeLocation(location);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((VertexAttributeLocation)value).Location);
            }
        }
    }
}
