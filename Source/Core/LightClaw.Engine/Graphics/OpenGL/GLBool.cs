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
    [Serializable, DataContract]
    [JsonConverter(typeof(GLBoolConverter))]
    public struct GLBool : ICloneable, IEquatable<GLBool>
    {
        public static GLBool False
        {
            get
            {
                return new GLBool(false);
            }
        }

        public static GLBool True
        {
            get
            {
                return new GLBool(true);
            }
        }

        public bool BoolValue
        {
            get
            {
                return (this.Value != 0);
            }
        }

        [DataMember]
        public int Value { get; private set; }

        public GLBool(bool value) : this(value ? 1 : 0) { }

        public GLBool(int value)
            : this()
        {
            this.Value = value;
        }

        public object Clone()
        {
            return new GLBool(this.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is GLBool) ? this.Equals((GLBool)obj) : false;
        }

        public bool Equals(GLBool other)
        {
            return (this.Value == other.Value);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Value);
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
