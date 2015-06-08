using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a vertex storing position, normal and texture coordinates.
    /// </summary>
    [Serializable, DataContract]
    [StructLayout(LayoutKind.Sequential)] // Even though sequential is default, specify for clarity
    public struct Vertex
    {
        /// <summary>
        /// Gets the size in bytes of the structure.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vertex));

        /// <summary>
        /// The <see cref="Vertex"/>'s position.
        /// </summary>
        [DataMember]
        public Vector3 Position;

        /// <summary>
        /// The <see cref="Vertex"/>'s normal.
        /// </summary>
        [DataMember]
        public Vector3 Normal;

        /// <summary>
        /// The <see cref="Vertex"/>'s binormal.
        /// </summary>
        [DataMember]
        public Vector3 Binormal;

        /// <summary>
        /// The <see cref="Vertex"/>'s tangent.
        /// </summary>
        [DataMember]
        public Vector3 Tangent;

        /// <summary>
        /// The <see cref="Vertex"/>'s texture coordinates.
        /// </summary>
        [DataMember]
        public Vector2 TexCoord;

        /// <summary>
        /// The <see cref="Vertex"/>'s color.
        /// </summary>
        [DataMember]
        public Color Color;

        /// <summary>
        /// Initializes a new <see cref="Vertex"/> from a position, a normal, a color and a texture coordinate.
        /// </summary>
        /// <param name="position">The <see cref="Vertex"/>'s position.</param>
        /// <param name="normal">The <see cref="Vertex"/>'s normal.</param>
        /// <param name="texCoord">The <see cref="Vertex"/>'s texture coordinates.</param>
        /// <param name="color">The <see cref="Vertex"/>'s color.</param>
        public Vertex(Vector3 position, Vector3 normal, Vector3 binormal, Vector3 tangent, Vector2 texCoord, Color color)
            : this()
        {
            this.Binormal = binormal;
            this.Color = color;
            this.Normal = normal;
            this.Position = position;
            this.Tangent = tangent;
            this.TexCoord = texCoord;
        }

        /// <summary>
        /// Checks whether the <see cref="Vertex"/> equals the specified object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is Vertex) ? this.Equals((Vertex)obj) : false;
        }

        /// <summary>
        /// Checks whether the current instance is equal to the specified <see cref="Vertex"/>.
        /// </summary>
        /// <param name="other">The <see cref="Vertex"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="Vertex"/>s are equal, otherwise <c>false</c>.</returns>
        public bool Equals(Vertex other)
        {
            return (this.Position == other.Position) && (this.Normal == other.Normal) &&
                   (this.Binormal == other.Binormal) && (this.Tangent == other.Tangent) &&
                   (this.TexCoord == other.TexCoord) && (this.Color == other.Color);
        }

        /// <summary>
        /// Gets the <see cref="Vertex"/>'s hash code.
        /// </summary>
        /// <returns>The <see cref="Vertex"/>'s hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(
                this.Position, 
                HashF.GetHashCode(this.Normal, this.Binormal, this.Tangent),
                this.TexCoord, this.Color
            );
        }

        /// <summary>
        /// Checks whether two <see cref="Vertex"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="Vertex"/>s are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(Vertex left, Vertex right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="Vertex"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="Vertex"/>s are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(Vertex left, Vertex right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Gets <see cref="VertexAttributePointer"/>s for an interleaved vertex data format.
        /// </summary>
        /// <param name="positionIndex">The index of the position attribute.</param>
        /// <param name="normalIndex">The index of the normal attribute.</param>
        /// <param name="textureIndex">The index of the UV attribute.</param>
        /// <param name="colorIndex">The index of the color attribute.</param>
        /// <returns>Interleaved <see cref="VertexAttributePointer"/>s.</returns>
        public static VertexAttributePointer[] GetInterleavedVaps(
                VertexAttributeLocation positionIndex, 
                VertexAttributeLocation normalIndex, 
                VertexAttributeLocation binormalIndex, 
                VertexAttributeLocation tangentIndex,
                VertexAttributeLocation textureIndex, 
                VertexAttributeLocation colorIndex
            )
        {
            return new[] {
                new VertexAttributePointer(positionIndex, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0),
                new VertexAttributePointer(normalIndex, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 12),
                new VertexAttributePointer(binormalIndex, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 24),
                new VertexAttributePointer(tangentIndex, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 36),
                new VertexAttributePointer(textureIndex, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 48),
                new VertexAttributePointer(colorIndex, 4, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 56)
            };
        }
    }
}
