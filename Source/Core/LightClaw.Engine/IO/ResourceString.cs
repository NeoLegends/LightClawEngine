using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using Newtonsoft.Json;

namespace LightClaw.Engine.IO
{
    [DataContract]
    [JsonConverter(typeof(ResourceStringConverter))]
    public struct ResourceString : ICloneable, IEquatable<ResourceString>
    {
        [IgnoreDataMember]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Path);
            }
        }

        [DataMember]
        public string Path { get; private set; }

        public ResourceString(string path)
            : this()
        {
            this.Path = path;
        }

        public object Clone()
        {
            return new ResourceString(this.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is ResourceString) ? this.Equals((ResourceString)obj) : false;
        }

        public bool Equals(ResourceString other)
        {
            return (this.Path == other.Path);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Path);
        }

        public static implicit operator string(ResourceString resourceString)
        {
            return resourceString.Path;
        }

        public static implicit operator ResourceString(string resourceString)
        {
            return new ResourceString(resourceString);
        }

        public static bool operator ==(ResourceString left, ResourceString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResourceString left, ResourceString right)
        {
            return !(left == right);
        }

        public class ResourceStringConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(ResourceString));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return serializer.Deserialize<string>(reader);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((ResourceString)value).Path);
            }
        }
    }
}
