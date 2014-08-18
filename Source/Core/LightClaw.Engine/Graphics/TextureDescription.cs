﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [DataContract, ProtoContract]
    public struct TextureDescription : ICloneable, IEquatable<TextureDescription>
    {
        [DataMember, ProtoMember(1)]
        public int Width { get; private set; }

        [DataMember, ProtoMember(2)]
        public int Height { get; private set; }

        [DataMember, ProtoMember(3)]
        public int Depth { get; private set; }
        
        [DataMember, ProtoMember(4)]
        public int TextureLevels { get; private set; }

        [DataMember, ProtoMember(5)]
        public int MultisamplingLevels { get; private set; }

        [DataMember, ProtoMember(6)]
        public PixelInternalFormat PixelInternalFormat { get; private set; }

        [DataMember, ProtoMember(7)]
        public TextureTarget Target { get; private set; }

        public TextureDescription(int width, int texLevels, int msLevels, TextureTarget target, PixelInternalFormat pixelInternalFormat)
            : this(width, texLevels, 0, texLevels, msLevels, target, pixelInternalFormat)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentOutOfRangeException>(texLevels > 0);
            Contract.Requires<ArgumentOutOfRangeException>(msLevels > 0);
        }

        public TextureDescription(int width, int height, int texLevels, int msLevels, TextureTarget target, PixelInternalFormat pixelInternalFormat)
            : this(width, height, 0, texLevels, msLevels, target, pixelInternalFormat) 
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentOutOfRangeException>(texLevels > 0);
            Contract.Requires<ArgumentOutOfRangeException>(msLevels > 0);
        }

        public TextureDescription(int width, int height, int depth, int texLevels, int msLevels, TextureTarget target, PixelInternalFormat pixelInternalFormat)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth >= 0);
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)width));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)height));
            Contract.Requires<ArgumentException>(MathF.IsPowerOfTwo((uint)depth));
            Contract.Requires<ArgumentOutOfRangeException>(texLevels > 0);
            Contract.Requires<ArgumentOutOfRangeException>(msLevels > 0);

            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.MultisamplingLevels = msLevels;
            this.Target = target;
            this.TextureLevels = texLevels;
            this.PixelInternalFormat = pixelInternalFormat;
        }

        public object Clone()
        {
            return new TextureDescription(this.Width, this.Height, this.Depth, this.TextureLevels, this.MultisamplingLevels, this.Target, this.PixelInternalFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is TextureDescription) ? this.Equals((TextureDescription)obj) : false;
        }

        public bool Equals(TextureDescription other)
        {
            return (this.Depth == other.Depth) && (this.Height == other.Height) &&
                   (this.PixelInternalFormat == other.PixelInternalFormat) && (this.TextureLevels == other.TextureLevels) &&
                   (this.MultisamplingLevels == other.MultisamplingLevels) &&
                   (this.Target == other.Target) && (this.Width == other.Width);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(
                HashF.GetHashCode(this.Width, this.Height, this.Depth, this.TextureLevels),
                HashF.GetHashCode(this.MultisamplingLevels, this.PixelInternalFormat, this.Target)
            );
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Width >= 0);
            Contract.Invariant(this.Height >= 0);
            Contract.Invariant(this.Depth >= 0);
            //Contract.Invariant(this.Levels > 0);
        }

        public static bool operator ==(TextureDescription left, TextureDescription right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextureDescription left, TextureDescription right)
        {
            return !(left == right);
        }

        public static int GetMaxTextureLevels(int width)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Ensures(Contract.Result<int>() > 0);

            return Math.Max((int)Math.Log(width, 2), 1);
        }

        public static int GetMaxTextureLevels(int width, int height)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Ensures(Contract.Result<int>() > 0);

            return Math.Max((int)Math.Min(Math.Log(width, 2), Math.Log(height, 2)) + 1, 1);
        }

        public static int GetMaxTextureLevels(int width, int height, int depth)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(height >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(depth >= 0);
            Contract.Ensures(Contract.Result<int>() > 0);

            return Math.Max((int)Math.Min(Math.Min(Math.Log(width, 2), Math.Log(height, 2)), Math.Log(depth, 2)) + 1, 1);
        }
    }
}
