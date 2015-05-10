using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using Newtonsoft.Json;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a 32-bit boolean.
    /// </summary>
    [Serializable, DataContract]
    [JsonConverter(typeof(GLBoolConverter))]
    public struct GLBool : ICloneable, IEquatable<GLBool>
    {
        /// <summary>
        /// Value false.
        /// </summary>
        public static GLBool False
        {
            get
            {
                return new GLBool(false);
            }
        }

        /// <summary>
        /// Value true.
        /// </summary>
        public static GLBool True
        {
            get
            {
                return new GLBool(true);
            }
        }

        /// <summary>
        /// The <see cref="GLBool"/> as native .NET boolean.
        /// </summary>
        public bool BoolValue
        {
            get
            {
                return (this.Value != 0);
            }
        }

        /// <summary>
        /// The value. If this is non-zero, the <see cref="GLBool"/> represents <c>true</c>, otherwise <c>false</c>.
        /// </summary>
        [DataMember]
        public int Value { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="GLBool"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public GLBool(bool value) : this(value ? 1 : 0) { }

        /// <summary>
        /// Initializes a new <see cref="GLBool"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public GLBool(int value)
            : this()
        {
            this.Value = value;
        }

        /// <summary>
        /// Clones the <see cref="GLBool"/>.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new GLBool(this.Value);
        }

        /// <summary>
        /// Checks whether the <see cref="GLBool"/> equals the specified object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is GLBool) ? this.Equals((GLBool)obj) : false;
        }

        /// <summary>
        /// Checks whether the current instance is equal to the specified <see cref="GLBool"/>.
        /// </summary>
        /// <param name="other">The <see cref="GLBool"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="GLBool"/>s are equal, otherwise <c>false</c>.</returns>
        public bool Equals(GLBool other)
        {
            return (this.Value == other.Value);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.BoolValue);
        }

        public static bool operator !(GLBool glBool)
        {
            return !glBool.BoolValue;
        }

        public static bool operator ==(GLBool left, GLBool right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GLBool left, GLBool right)
        {
            return !(left == right);
        }

        public static bool operator ==(GLBool left, bool right)
        {
            return (left.BoolValue == right);
        }

        public static bool operator !=(GLBool left, bool right)
        {
            return !(left == right);
        }

        public static bool operator ==(bool left, GLBool right)
        {
            return (left == right.BoolValue);
        }

        public static bool operator !=(bool left, GLBool right)
        {
            return !(left == right);
        }

        public static implicit operator bool(GLBool glBool)
        {
            return glBool.BoolValue;
        }

        public static implicit operator GLBool(bool boolean)
        {
            return new GLBool(boolean);
        }

        public static implicit operator int(GLBool glBool)
        {
            return glBool.Value;
        }

        public static implicit operator GLBool(int integer)
        {
            return new GLBool(integer);
        }

        public class GLBoolConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(GLBool));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return new GLBool(serializer.Deserialize<bool>(reader));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((GLBool)value).BoolValue);
            }
        }
    }
}
