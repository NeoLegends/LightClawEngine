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
        /// Backing field.
        /// </summary>
        private static readonly VertexAttributePointer[] _Interleaved = new[]
        {
            new VertexAttributePointer(VertexAttributeLocation.Position, 3, VertexAttribPointerType.Float, false, SizeInBytes, 0),
            new VertexAttributePointer(VertexAttributeLocation.Normals, 3, VertexAttribPointerType.Float, false, SizeInBytes, 12),
            new VertexAttributePointer(VertexAttributeLocation.TexCoords, 2, VertexAttribPointerType.Float, false, SizeInBytes, 24),
            new VertexAttributePointer(VertexAttributeLocation.Color, 4, VertexAttribPointerType.UnsignedByte, false, SizeInBytes, 32),
            new VertexAttributePointer(VertexAttributeLocation.BoneWeight0, 2, VertexAttribPointerType.UnsignedByte, false, SizeInBytes, 36),
            new VertexAttributePointer(VertexAttributeLocation.BoneWeight1, 2, VertexAttribPointerType.UnsignedByte, false, SizeInBytes, 44),
            new VertexAttributePointer(VertexAttributeLocation.BoneWeight2, 2, VertexAttribPointerType.UnsignedByte, false, SizeInBytes, 52),
            new VertexAttributePointer(VertexAttributeLocation.BoneWeight3, 2, VertexAttribPointerType.UnsignedByte, false, SizeInBytes, 60),
        };

        /// <summary> 
        /// Gets a default <see cref="VertexAttributePointer"/>-configuration for the <see
        /// cref="Vertex"/>. The configuration can be used for interleaved vertex data. 
        /// </summary>
        /// <remarks> 
        /// <para> 
        /// Attribute location in GLSL will be assumed as follows: 
        /// <list type="bullet"> 
        ///     <item> 
        ///         <description> 
        ///             Vertex Position (3 float): <see cref="VertexAttributeLocation.Position"/>
        ///         </description> 
        ///     </item> 
        ///     <item>
        ///         <description> 
        ///             Vertex Normal (3 float): <see cref="VertexAttributeLocation.Normals"/>
        ///         </description> 
        ///     </item> 
        ///     <item>
        ///         <description> 
        ///             Texture Coordinates (2 float): <see cref="VertexAttributeLocation.TexCoords"/>
        ///         </description> 
        ///     </item> 
        ///     <item>
        ///         <description>
        ///             Color (4 unsigned byte): <see cref="VertexAttributeLocation.Color"/> 
        ///         </description> 
        ///     </item> 
        /// </list> 
        /// </para> 
        /// </remarks>
        public static VertexAttributePointer[] Interleaved
        {
            get
            {
                Contract.Ensures(Contract.Result<VertexAttributePointer[]>() != null);
                Contract.Ensures(Contract.Result<VertexAttributePointer[]>().Length == 3);

                return _Interleaved.ToArray();
            }
        }

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
        /// The first bone weighting.
        /// </summary>
        [DataMember]
        public BoneWeighting BoneWeight0;

        /// <summary>
        /// The second bone weighting.
        /// </summary>
        [DataMember]
        public BoneWeighting BoneWeight1;

        /// <summary>
        /// The third bone weighting.
        /// </summary>
        [DataMember]
        public BoneWeighting BoneWeight2;

        /// <summary>
        /// The fourth bone weighting.
        /// </summary>
        [DataMember]
        public BoneWeighting BoneWeight3;

        /// <summary>
        /// Initializes a new <see cref="Vertex"/> from a position, a normal, a color and a texture coordinate.
        /// </summary>
        /// <param name="position">The <see cref="Vertex"/>'s position.</param>
        /// <param name="normal">The <see cref="Vertex"/>'s normal.</param>
        /// <param name="texCoord">The <see cref="Vertex"/>'s texture coordinates.</param>
        /// <param name="color">The <see cref="Vertex"/>'s color.</param>
        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord, Color color)
            : this()
        {
            this.Normal = normal;
            this.Position = position;
            this.TexCoord = texCoord;
            this.Color = color;
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
                   (this.TexCoord == other.TexCoord) && (this.Color == other.Color) &&
                   (this.BoneWeight0 == other.BoneWeight0) && (this.BoneWeight1 == other.BoneWeight1) &&
                   (this.BoneWeight2 == other.BoneWeight2) && (this.BoneWeight3 == other.BoneWeight3);
        }

        /// <summary>
        /// Gets the <see cref="Vertex"/>'s hash code.
        /// </summary>
        /// <returns>The <see cref="Vertex"/>'s hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(
                this.Position, this.Normal, this.TexCoord, this.Color,
                HashF.GetHashCode(this.BoneWeight0, this.BoneWeight1, this.BoneWeight2, this.BoneWeight3)
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
    }
}
