using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using ProtoBuf;

using GLTextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;

namespace LightClaw.Engine.Graphics
{
    [DataContract, ProtoContract]
    public struct TextureUnit : ICloneable, IDisposable, IEquatable<TextureUnit>, IEquatable<int>
    {
        public static readonly TextureUnit Texture0 = new TextureUnit(0);

        public static readonly TextureUnit Texture1 = new TextureUnit(1);

        public static readonly TextureUnit Texture2 = new TextureUnit(2);

        public static readonly TextureUnit Texture3 = new TextureUnit(3);

        public static readonly TextureUnit Texture4 = new TextureUnit(4);

        public static readonly TextureUnit Texture5 = new TextureUnit(5);

        public static readonly TextureUnit Texture6 = new TextureUnit(6);

        public static readonly TextureUnit Texture7 = new TextureUnit(7);

        public static readonly TextureUnit Texture8 = new TextureUnit(8);

        public static readonly TextureUnit Texture9 = new TextureUnit(9);

        public static readonly TextureUnit Texture10 = new TextureUnit(10);

        public static readonly TextureUnit Texture11 = new TextureUnit(11);

        public static readonly TextureUnit Texture12 = new TextureUnit(12);

        public static readonly TextureUnit Texture13 = new TextureUnit(13);

        public static readonly TextureUnit Texture14 = new TextureUnit(14);

        public static readonly TextureUnit Texture15 = new TextureUnit(15);

        public static readonly TextureUnit Texture16 = new TextureUnit(16);

        public static readonly TextureUnit Texture17 = new TextureUnit(17);

        public static readonly TextureUnit Texture18 = new TextureUnit(18);

        public static readonly TextureUnit Texture19 = new TextureUnit(19);

        public static readonly TextureUnit Texture20 = new TextureUnit(20);

        public static readonly TextureUnit Texture21 = new TextureUnit(21);

        public static readonly TextureUnit Texture22 = new TextureUnit(22);

        public static readonly TextureUnit Texture23 = new TextureUnit(23);

        public static readonly TextureUnit Texture24 = new TextureUnit(24);

        public static readonly TextureUnit Texture25 = new TextureUnit(25);

        public static readonly TextureUnit Texture26 = new TextureUnit(26);

        public static readonly TextureUnit Texture27 = new TextureUnit(27);

        public static readonly TextureUnit Texture28 = new TextureUnit(28);

        public static readonly TextureUnit Texture29 = new TextureUnit(29);

        public static readonly TextureUnit Texture30 = new TextureUnit(30);

        public static readonly TextureUnit Texture31 = new TextureUnit(31);

        public event EventHandler<ParameterEventArgs> Disposed;

        [DataMember, ProtoMember(1)]
        public int Unit { get; private set; }

        public TextureUnit(int unit)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(unit >= 0);

            this.Unit = unit;
        }

        public TextureUnit(GLTextureUnit unit) : this(unit - GLTextureUnit.Texture0) { }

        public object Clone()
        {
            return new TextureUnit(this.Unit);
        }

        public void Dispose()
        {
            EventHandler<ParameterEventArgs> handler = this.Disposed;
            if (handler != null)
            {
                handler(this, new ParameterEventArgs(this.Unit));
            }

            this.Unit = -1;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is TextureUnit || obj is int) ? this.Equals((int)obj) : false;
        }

        public bool Equals(TextureUnit other)
        {
            return this.Equals(other.Unit);
        }

        public bool Equals(int other)
        {
            return (this.Unit == other);
        }

        public override int GetHashCode()
        {
            return this.Unit.GetHashCode();
        }

        public override string ToString()
        {
            return this.Unit.ToString();
        }

        public static TextureUnit Parse(string s)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(s));

            return new TextureUnit(int.Parse(s));
        }

        public static bool TryParse(string s, out TextureUnit textureUnit)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(s));

            int unit;
            bool result = int.TryParse(s, out unit);
            textureUnit = result ? new TextureUnit(unit) : default(TextureUnit);
            return result;
        }

        public static TextureUnit operator +(TextureUnit left, TextureUnit right)
        {
            return new TextureUnit(left.Unit + right.Unit);
        }

        public static bool operator ==(TextureUnit left, TextureUnit right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextureUnit left, TextureUnit right)
        {
            return !(left == right);
        }

        public static implicit operator int(TextureUnit unit)
        {
            return unit.Unit;
        }

        public static implicit operator TextureUnit(int unit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(unit >= 0);

            return new TextureUnit(unit);
        }

        public static implicit operator GLTextureUnit(TextureUnit unit)
        {
            return (GLTextureUnit)(GLTextureUnit.Texture0 + unit);
        }

        public static implicit operator TextureUnit(GLTextureUnit unit)
        {
            return new TextureUnit(unit - GLTextureUnit.Texture0);
        }
    }
}
