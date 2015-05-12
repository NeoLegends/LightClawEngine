using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using Newtonsoft.Json;
using GLTextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a texture unit. A target an OpenGL texture can be bound to.
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(TextureUnitConverter))]
    public struct TextureUnit : ICloneable, IEquatable<int>, IEquatable<TextureUnit>
    {
        #region Predefined Units

        /// <summary>
        /// Texture unit 0.
        /// </summary>
        public static readonly TextureUnit Texture0 = new TextureUnit(0);

        /// <summary>
        /// Texture unit 1.
        /// </summary>
        public static readonly TextureUnit Texture1 = new TextureUnit(1);

        /// <summary>
        /// Texture unit 2.
        /// </summary>
        public static readonly TextureUnit Texture2 = new TextureUnit(2);

        /// <summary>
        /// Texture unit 3.
        /// </summary>
        public static readonly TextureUnit Texture3 = new TextureUnit(3);

        /// <summary>
        /// Texture unit 4.
        /// </summary>
        public static readonly TextureUnit Texture4 = new TextureUnit(4);

        /// <summary>
        /// Texture unit 5.
        /// </summary>
        public static readonly TextureUnit Texture5 = new TextureUnit(5);

        /// <summary>
        /// Texture unit 6.
        /// </summary>
        public static readonly TextureUnit Texture6 = new TextureUnit(6);

        /// <summary>
        /// Texture unit 7.
        /// </summary>
        public static readonly TextureUnit Texture7 = new TextureUnit(7);

        /// <summary>
        /// Texture unit 8.
        /// </summary>
        public static readonly TextureUnit Texture8 = new TextureUnit(8);

        /// <summary>
        /// Texture unit 9.
        /// </summary>
        public static readonly TextureUnit Texture9 = new TextureUnit(9);

        /// <summary>
        /// Texture unit 10.
        /// </summary>
        public static readonly TextureUnit Texture10 = new TextureUnit(10);

        /// <summary>
        /// Texture unit 11.
        /// </summary>
        public static readonly TextureUnit Texture11 = new TextureUnit(11);

        /// <summary>
        /// Texture unit 12.
        /// </summary>
        public static readonly TextureUnit Texture12 = new TextureUnit(12);

        /// <summary>
        /// Texture unit 13.
        /// </summary>
        public static readonly TextureUnit Texture13 = new TextureUnit(13);

        /// <summary>
        /// Texture unit 14.
        /// </summary>
        public static readonly TextureUnit Texture14 = new TextureUnit(14);

        /// <summary>
        /// Texture unit 15.
        /// </summary>
        public static readonly TextureUnit Texture15 = new TextureUnit(15);

        /// <summary>
        /// Texture unit 16.
        /// </summary>
        public static readonly TextureUnit Texture16 = new TextureUnit(16);

        /// <summary>
        /// Texture unit 17.
        /// </summary>
        public static readonly TextureUnit Texture17 = new TextureUnit(17);

        /// <summary>
        /// Texture unit 18.
        /// </summary>
        public static readonly TextureUnit Texture18 = new TextureUnit(18);

        /// <summary>
        /// Texture unit 19.
        /// </summary>
        public static readonly TextureUnit Texture19 = new TextureUnit(19);

        /// <summary>
        /// Texture unit 20.
        /// </summary>
        public static readonly TextureUnit Texture20 = new TextureUnit(20);

        /// <summary>
        /// Texture unit 21.
        /// </summary>
        public static readonly TextureUnit Texture21 = new TextureUnit(21);

        /// <summary>
        /// Texture unit 22.
        /// </summary>
        public static readonly TextureUnit Texture22 = new TextureUnit(22);

        /// <summary>
        /// Texture unit 23.
        /// </summary>
        public static readonly TextureUnit Texture23 = new TextureUnit(23);

        /// <summary>
        /// Texture unit 24.
        /// </summary>
        public static readonly TextureUnit Texture24 = new TextureUnit(24);

        /// <summary>
        /// Texture unit 25.
        /// </summary>
        public static readonly TextureUnit Texture25 = new TextureUnit(25);

        /// <summary>
        /// Texture unit 26.
        /// </summary>
        public static readonly TextureUnit Texture26 = new TextureUnit(26);

        /// <summary>
        /// Texture unit 27.
        /// </summary>
        public static readonly TextureUnit Texture27 = new TextureUnit(27);

        /// <summary>
        /// Texture unit 28.
        /// </summary>
        public static readonly TextureUnit Texture28 = new TextureUnit(28);

        /// <summary>
        /// Texture unit 29.
        /// </summary>
        public static readonly TextureUnit Texture29 = new TextureUnit(29);

        /// <summary>
        /// Texture unit 30.
        /// </summary>
        public static readonly TextureUnit Texture30 = new TextureUnit(30);

        /// <summary>
        /// Texture unit 31.
        /// </summary>
        public static readonly TextureUnit Texture31 = new TextureUnit(31);

        #endregion

        /// <summary>
        /// The index of the texture unit.
        /// </summary>
        [DataMember]
        public int Unit { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="TextureUnit"/> from the specified <see cref="GLTextureUnit"/>.
        /// </summary>
        /// <param name="unit">The unit to initialize from.</param>
        public TextureUnit(GLTextureUnit unit) : this(unit - GLTextureUnit.Texture0) { }

        /// <summary>
        /// Initializes a new <see cref="TextureUnit"/>.
        /// </summary>
        /// <param name="unit">The unit to initialize from.</param>
        public TextureUnit(int unit)
            : this()
        {
            this.Unit = unit;
        }

        /// <summary>
        /// Clones the <see cref="TextureUnit"/>.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new TextureUnit(this.Unit);
        }

        /// <summary>
        /// Checks whether the <see cref="TextureUnit"/> equals the specified object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is TextureUnit || obj is int) ? this.Equals((int)obj) : false;
        }

        /// <summary>
        /// Checks whether the current instance is equal to the specified <see cref="TextureUnit"/>.
        /// </summary>
        /// <param name="other">The <see cref="TextureUnit"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="TextureUnit"/>s are equal, otherwise <c>false</c>.</returns>
        public bool Equals(TextureUnit other)
        {
            return this.Equals(other.Unit);
        }

        /// <summary>
        /// Checks whether the current instance is equal to the specified <see cref="TextureUnit"/>.
        /// </summary>
        /// <param name="other">The <see cref="TextureUnit"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="TextureUnit"/>s are equal, otherwise <c>false</c>.</returns>
        public bool Equals(int other)
        {
            return (this.Unit == other);
        }

        /// <summary>
        /// Gets the <see cref="BufferRange"/>'s hash code.
        /// </summary>
        /// <returns>The <see cref="BufferRange"/>'s hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Unit);
        }

        /// <summary>
        /// Converts the <see cref="TextureUnit"/> into a <see cref="String"/>.
        /// </summary>
        /// <returns>The <see cref="TextureUnit"/> as <see cref="String"/>.</returns>
        public override string ToString()
        {
            return this.Unit.ToString();
        }

        /// <summary>
        /// Compares two <see cref="TextureUnit"/>s.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c>, if the left operand is smaller than the right operand.</returns>
        public static bool operator <(TextureUnit left, TextureUnit right)
        {
            return (left.Unit < right.Unit);
        }

        /// <summary>
        /// Compares two <see cref="TextureUnit"/>s.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c>, if the left operand is greater than the right operand.</returns>
        public static bool operator >(TextureUnit left, TextureUnit right)
        {
            return (left.Unit > right.Unit);
        }

        /// <summary>
        /// Compares two <see cref="TextureUnit"/>s.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c>, if the left operand is smaller than or equal to the right operand.</returns>
        public static bool operator <=(TextureUnit left, TextureUnit right)
        {
            return (left.Unit <= right.Unit);
        }

        /// <summary>
        /// Compares two <see cref="TextureUnit"/>s.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c>, if the left operand is greater than or equal to the right operand.</returns>
        public static bool operator >=(TextureUnit left, TextureUnit right)
        {
            return (left.Unit >= right.Unit);
        }

        /// <summary>
        /// Checks whether two <see cref="TextureUnit"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="TextureUnit"/>s are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(TextureUnit left, TextureUnit right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="TextureUnit"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="TextureUnit"/>s are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(TextureUnit left, TextureUnit right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implicitly converts the <see cref="TextureUnit"/> into an <see cref="Int32"/>.
        /// </summary>
        /// <param name="unit">The <see cref="TextureUnit"/> to convert.</param>
        /// <returns>The conversion result.</returns>
        public static implicit operator int(TextureUnit unit)
        {
            return unit.Unit;
        }

        /// <summary>
        /// Implicitly converts the <see cref="Int32"/> into a <see cref="TextureUnit"/>.
        /// </summary>
        /// <param name="unit">The <see cref="Int32"/> to convert.</param>
        /// <returns>The conversion result.</returns>
        public static implicit operator TextureUnit(int unit)
        {
            return new TextureUnit(unit);
        }

        /// <summary>
        /// Implicitly converts the <see cref="TextureUnit"/> into an <see cref="GLTextureUnit"/>.
        /// </summary>
        /// <param name="unit">The <see cref="TextureUnit"/> to convert.</param>
        /// <returns>The conversion result.</returns>
        public static implicit operator GLTextureUnit(TextureUnit unit)
        {
            return (GLTextureUnit)(GLTextureUnit.Texture0 + unit);
        }

        /// <summary>
        /// Implicitly converts the <see cref="GLTextureUnit"/> into a <see cref="TextureUnit"/>.
        /// </summary>
        /// <param name="unit">The <see cref="GLTextureUnit"/> to convert.</param>
        /// <returns>The conversion result.</returns>
        public static implicit operator TextureUnit(GLTextureUnit unit)
        {
            return new TextureUnit(unit - GLTextureUnit.Texture0);
        }

        /// <summary>
        /// A <see cref="JsonConverter"/> that converts <see cref="TextureUnit"/>s.
        /// </summary>
        public class TextureUnitConverter : JsonConverter
        {
            public override bool CanRead
            {
                get
                {
                    return true;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return true;
                }
            }

            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(TextureUnit));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                int unit = serializer.Deserialize<int>(reader);
                if (unit >= 0)
                {
                    throw new InvalidOperationException("The value to deserialize into a TextureUnit was smaller than zero.");
                }
                return new TextureUnit(unit);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((TextureUnit)value).Unit);
            }
        }
    }
}
