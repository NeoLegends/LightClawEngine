﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    [Serializable, DataContract]
    [DebuggerDisplay("{Name}: {Location}")]
    public struct VertexAttributeDescription : ICloneable, IEquatable<VertexAttributeDescription>
    {
        public static VertexAttributeDescription[] Empty
        {
            get
            {
                Contract.Ensures(Contract.Result<VertexAttributeDescription[]>() != null);

                return new VertexAttributeDescription[] { };
            }
        }

        [DataMember]
        public VertexAttributeLocation Location { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        public VertexAttributeDescription(string name, VertexAttributeLocation location)
            : this()
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
            Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

            this.Location = location;
            this.Name = name;
        }

        public void ApplyIn(int shaderProgram)
        {
            GL.BindAttribLocation(shaderProgram, this.Location, this.Name);
        }

        public object Clone()
        {
            return new VertexAttributeDescription(this.Name, this.Location);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is VertexAttributeDescription) ? this.Equals((VertexAttributeDescription)obj) : false;
        }

        public bool Equals(VertexAttributeDescription other)
        {
            return (this.Location == other.Location) && (this.Name == other.Name);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Location, this.Name);
        }

        public static bool operator ==(VertexAttributeDescription left, VertexAttributeDescription right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexAttributeDescription left, VertexAttributeDescription right)
        {
            return !(left == right);
        }
    }
}
