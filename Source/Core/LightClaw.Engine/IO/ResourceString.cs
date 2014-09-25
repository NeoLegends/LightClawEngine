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
    /// <summary>
    /// Represents a unique ID of an asset.
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(ResourceStringConverter))]
    public struct ResourceString : ICloneable, IEquatable<ResourceString>
    {
        /// <summary>
        /// Indicates whether the <see cref="ResourceString"/> is valid, respectively <c>!string.IsNullOrWhiteSpace</c>.
        /// </summary>
        [IgnoreDataMember]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Path);
            }
        }

        /// <summary>
        /// The actual asset path.
        /// </summary>
        [DataMember]
        public string Path { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ResourceString"/> and sets the <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The actual asset path.</param>
        public ResourceString(string path)
            : this()
        {
            this.Path = path;
        }

        /// <summary>
        /// Creates a clone of the <see cref="ResourceString"/>.
        /// </summary>
        /// <returns>The cloning object.</returns>
        public object Clone()
        {
            return new ResourceString(this.Path);
        }

        /// <summary>
        /// Tests whether the <see cref="ResourceString"/> equals the specified object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is ResourceString) ? this.Equals((ResourceString)obj) : false;
        }

        /// <summary>
        /// Checks whether the <see cref="ResourceString"/> equals the <paramref name="other"/> one.
        /// </summary>
        /// <param name="obj">The <see cref="ResourceString"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="ResourceString"/> are equal, otherwise <c>false</c>.</returns>
        public bool Equals(ResourceString other)
        {
            return (this.Path == other.Path);
        }

        /// <summary>
        /// Gets the <see cref="ResourceString"/>s hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Path);
        }
        
        /// <summary>
        /// Implicitly converts the <see cref="ResourceString"/> into a <see cref="String"/>.
        /// </summary>
        /// <param name="resourceString">The <see cref="ResourceString"/> to convert.</param>
        /// <returns>The conversion result.</returns>
        public static implicit operator string(ResourceString resourceString)
        {
            return resourceString.Path;
        }

        /// <summary>
        /// Implicitly converts the <see cref="String"/> into a <see cref="ResourceString"/>.
        /// </summary>
        /// <param name="path">The <see cref="String"/> to convert.</param>
        /// <returns>The conversion result.</returns>
        public static implicit operator ResourceString(string path)
        {
            return new ResourceString(path);
        }

        /// <summary>
        /// Checks whether two <see cref="ResourceString"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="ResourceString"/> are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(ResourceString left, ResourceString right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="ResourceString"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="ResourceString"/> are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(ResourceString left, ResourceString right)
        {
            return !(left == right);
        }

        /// <summary>
        /// A custom <see cref="JsonConverter"/> used for more natural conversion of the <see cref="ResourceString"/> into its JSON representation.
        /// </summary>
        public class ResourceStringConverter : JsonConverter
        {
            /// <summary>
            /// Indicates whether the <see cref="ResourceStringConverter"/> can read JSON into a <see cref="ResourceString"/>.
            /// </summary>
            public override bool CanRead
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Indicates whether the <see cref="ResourceStringConverter"/> can write a <see cref="ResourceString"/> to JSON.
            /// </summary>
            public override bool CanWrite
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Checks whether the <see cref="ResourceStringConverter"/> can convert objects of the specified <see cref="Type"/> to JSON.
            /// </summary>
            /// <param name="objectType">The <see cref="Type"/> to test.</param>
            /// <returns><c>true</c> if objects of the specified <see cref="Type"/> can be converted to JSON, otherwise <c>false</c>.</returns>
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(ResourceString));
            }

            /// <summary>
            /// Reads JSON from the <see cref="JsonReader"/> into a <see cref="ResourceString"/>.
            /// </summary>
            /// <param name="reader">The <see cref="JsonReader"/> used to read the JSON.</param>
            /// <param name="objectType">The <see cref="Type"/> to be deserialized.</param>
            /// <param name="existingValue">An existing value.</param>
            /// <param name="serializer">The <see cref="JsonSerializer"/> that triggered the deserialization process.</param>
            /// <returns>The deserialized object.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return new ResourceString(serializer.Deserialize<string>(reader));
            }

            /// <summary>
            /// Writes the <paramref name="value"/> (which is a <see cref="ResourceString"/>) into the <paramref name="writer"/>.
            /// </summary>
            /// <param name="writer">The <see cref="JsonWriter"/> to write the data into.</param>
            /// <param name="value">The value to write.</param>
            /// <param name="serializer">The <see cref="JsonSerializer"/> that triggered the serialization process.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((ResourceString)value).Path);
            }
        }
    }
}
