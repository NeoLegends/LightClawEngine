using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// A format descriptor for texture data.
    /// </summary>
    [DataContract]
    public struct TextureDescription : ICloneable, IEquatable<TextureDescription>
    {
        /// <summary>
        /// The width of the texture.
        /// </summary>
        [DataMember]
        public int Width { get; private set; }

        /// <summary>
        /// The height of the texture.
        /// </summary>
        [DataMember]
        public int Height { get; private set; }

        /// <summary>
        /// The depth of the texture.
        /// </summary>
        [DataMember]
        public int Depth { get; private set; }

        /// <summary>
        /// The amount of mipmap levels.
        /// </summary>
        [DataMember]
        public int Levels { get; private set; }

        /// <summary>
        /// The amount of multisampling levels.
        /// </summary>
        [DataMember]
        public int MultisamplingLevels { get; private set; }

        /// <summary>
        /// The format of the pixels.
        /// </summary>
        [DataMember]
        public PixelInternalFormat PixelInternalFormat { get; private set; }

        /// <summary>
        /// The <see cref="TextureTarget"/> the <see cref="Texture"/> will be bound to.
        /// </summary>
        [DataMember]
        public TextureTarget Target { get; private set; }

        /// <summary>
        /// Initializes a new one-dimensional <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="texLevels">The amount of mipmap levels.</param>
        /// <param name="msLevels">The amount of multisampling levels.</param>
        /// <param name="target">The <see cref="TextureTarget"/> the <see cref="Texture"/> will be bound to.</param>
        /// <param name="pixelInternalFormat">The format of the pixels.</param>
        public TextureDescription(int width, int texLevels, int msLevels, TextureTarget1d target, PixelInternalFormat pixelInternalFormat)
            : this(width, 0, texLevels, msLevels, (TextureTarget2d)target, pixelInternalFormat)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentException>(width == 0 || MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentOutOfRangeException>(texLevels > 0);
            Contract.Requires<ArgumentOutOfRangeException>(msLevels >= 0);
        }

        /// <summary>
        /// Initializes a new two-dimensional <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="texLevels">The amount of mipmap levels.</param>
        /// <param name="msLevels">The amount of multisampling levels.</param>
        /// <param name="target">The <see cref="TextureTarget"/> the <see cref="Texture"/> will be bound to.</param>
        /// <param name="pixelInternalFormat">The format of the pixels.</param>
        public TextureDescription(int width, int height, int texLevels, int msLevels, TextureTarget2d target, PixelInternalFormat pixelInternalFormat)
            : this(width, height, 0, texLevels, msLevels, (TextureTarget3d)target, pixelInternalFormat)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Requires<ArgumentException>(width == 0 || MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(height == 0 || MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentOutOfRangeException>(texLevels > 0);
            Contract.Requires<ArgumentOutOfRangeException>(msLevels >= 0);
        }

        /// <summary>
        /// Initializes a new two-dimensional <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="depth">The depth of the texture.</param>
        /// <param name="texLevels">The amount of mipmap levels.</param>
        /// <param name="msLevels">The amount of multisampling levels.</param>
        /// <param name="target">The <see cref="TextureTarget"/> the <see cref="Texture"/> will be bound to.</param>
        /// <param name="pixelInternalFormat">The format of the pixels.</param>
        public TextureDescription(int width, int height, int depth, int texLevels, int msLevels, TextureTarget3d target, PixelInternalFormat pixelInternalFormat)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth >= 0);
            Contract.Requires<ArgumentException>(width == 0 || MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(height == 0 || MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(depth == 0 || MathF.IsPowerOfTwo((uint)depth));
            Contract.Requires<ArgumentOutOfRangeException>(texLevels > 0);
            Contract.Requires<ArgumentOutOfRangeException>(msLevels >= 0);

            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.MultisamplingLevels = msLevels;
            this.Target = (TextureTarget)target;
            this.Levels = texLevels;
            this.PixelInternalFormat = pixelInternalFormat;
        }

        /// <summary>
        /// Clones the <see cref="TextureDescription"/>.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new TextureDescription(
                this.Width,
                this.Height,
                this.Depth,
                this.Levels,
                this.MultisamplingLevels,
                (TextureTarget3d)this.Target,
                this.PixelInternalFormat
            );
        }

        /// <summary>
        /// Checks whether the <see cref="TextureDescription"/> equals the specified object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is TextureDescription) ? this.Equals((TextureDescription)obj) : false;
        }

        /// <summary>
        /// Checks whether the current instance is equal to the specified <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="other">The <see cref="TextureDescription"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="TextureDescription"/>s are equal, otherwise <c>false</c>.</returns>
        public bool Equals(TextureDescription other)
        {
            return (this.Depth == other.Depth) && (this.Height == other.Height) &&
                   (this.PixelInternalFormat == other.PixelInternalFormat) && (this.Levels == other.Levels) &&
                   (this.MultisamplingLevels == other.MultisamplingLevels) &&
                   (this.Target == other.Target) && (this.Width == other.Width);
        }

        /// <summary>
        /// Gets the <see cref="BufferRange"/>'s hash code.
        /// </summary>
        /// <returns>The <see cref="BufferRange"/>'s hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(
                HashF.GetHashCode(this.Width, this.Height, this.Depth, this.Levels),
                HashF.GetHashCode(this.MultisamplingLevels, this.PixelInternalFormat, this.Target)
            );
        }

        /// <summary>
        /// Gets the maximum amount of mipmap levels given a texture's width.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <returns>The maximum amount of mipmap levels.</returns>
        public static int GetMaxTextureLevels(int width)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Ensures(Contract.Result<int>() > 0);

            return Math.Max((int)Math.Log(width, 2), 1);
        }

        /// <summary>
        /// Gets the maximum amount of mipmap levels given a texture's width and height.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <returns>The maximum amount of mipmap levels.</returns>
        public static int GetMaxTextureLevels(int width, int height)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Ensures(Contract.Result<int>() > 0);

            return Math.Max((int)Math.Min(Math.Log(width, 2), Math.Log(height, 2)) + 1, 1);
        }

        /// <summary>
        /// Gets the maximum amount of mipmap levels given a texture's width, height and depth.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="depth">The depth of the texture.</param>
        /// <returns>The maximum amount of mipmap levels.</returns>
        public static int GetMaxTextureLevels(int width, int height, int depth)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth >= 0);
            Contract.Ensures(Contract.Result<int>() > 0);

            return Math.Max((int)Math.Min(Math.Min(Math.Log(width, 2), Math.Log(height, 2)), Math.Log(depth, 2)) + 1, 1);
        }

        /// <summary>
        /// Checks whether two <see cref="TextureDescription"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="TextureDescription"/>s are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(TextureDescription left, TextureDescription right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="TextureDescription"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="TextureDescription"/>s are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(TextureDescription left, TextureDescription right)
        {
            return !(left == right);
        }
    }
}
