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

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a vertex storing position, normal and texture coordinates.
    /// </summary>
    [DataContract]
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
        private static readonly VertexAttributePointer[] _VAPInterleaved = new[] 
        { 
            new VertexAttributePointer(VertexAttributeLocation.Position, 3, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, false, 32, 0),
            new VertexAttributePointer(VertexAttributeLocation.Normals, 3, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, false, 32, 12),
            new VertexAttributePointer(VertexAttributeLocation.TexCoords, 2, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, false, 32, 24),
        };

        /// <summary>
        /// Gets a default <see cref="VertexAttributePointer"/>-configuration for the <see cref="VertexPositionNormalTexCoord"/>.
        /// The configuration can be used for interleaved vertex data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Attribute location in GLSL will be assumed as follows:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             Vertex Position: <see cref="VertexAttributeLocation.Vertex"/>
        ///         </description>
        ///     </item>
        ///         <description>
        ///             Vertex Normal: <see cref="VertexAttributeLocation.Normals"/>
        ///         </description>
        ///     </item>
        ///         <description>
        ///             Texture Coordinates: <see cref="VertexAttributeLocation.TexCoords"/>
        ///         </description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public static VertexAttributePointer[] VAPInterleaved
        {
            get
            {
                Contract.Ensures(Contract.Result<VertexAttributePointer[]>() != null);
                Contract.Ensures(Contract.Result<VertexAttributePointer[]>().Length == 3);

                return _VAPInterleaved.ToArray();
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private static readonly VertexAttributePointer[] _VAPMultipleBuffers = new VertexAttributePointer[]
        {
            new VertexAttributePointer(VertexAttributeLocation.Position, 3, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, false, 0, 0),
            new VertexAttributePointer(VertexAttributeLocation.Normals, 3, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, false, 0, 0),
            new VertexAttributePointer(VertexAttributeLocation.TexCoords, 2, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, false, 0, 0)
        };

        /// <summary>
        /// Gets a default <see cref="VertexAttributePointer"/>-configuration for the <see cref="VertexPositionNormalTexCoord"/>.
        /// The configuration can be used together with multiple buffers storing the attributes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Attribute location in GLSL will be assumed as follows:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             Vertex Position: <see cref="VertexAttributeLocation.Vertex"/>
        ///         </description>
        ///     </item>
        ///         <description>
        ///             Vertex Normal: <see cref="VertexAttributeLocation.Normals"/>
        ///         </description>
        ///     </item>
        ///         <description>
        ///             Texture Coordinates: <see cref="VertexAttributeLocation.TexCoords"/>
        ///         </description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public static VertexAttributePointer[] VAPMultipleBuffers
        {
            get
            {
                Contract.Ensures(Contract.Result<VertexAttributePointer[]>() != null);
                Contract.Ensures(Contract.Result<VertexAttributePointer[]>().Length == 3);

                return _VAPMultipleBuffers.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Vertex"/>s components as array.
        /// </summary>
        public float[] Array
        {
            get
            {
                return new[]
                {
                    this.Position.X, this.Position.Y, this.Position.Z,
                    this.Normal.X, this.Normal.Y, this.Normal.Z,
                    this.TexCoord.X, this.TexCoord.Y
                };
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.Length >= 8);

                this.Position.X = value[0];
                this.Position.Y = value[1];
                this.Position.Z = value[2];
                this.Normal.X = value[3];
                this.Normal.Y = value[4];
                this.Normal.Z = value[5];
                this.TexCoord.X = value[6];
                this.TexCoord.Y = value[7];
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
        /// Initializes a new <see cref="Vertex"/> from a position, a normal and a texture coordinate.
        /// </summary>
        /// <param name="position">The <see cref="Vertex"/>'s position.</param>
        /// <param name="normal">The <see cref="Vertex"/>'s normal.</param>
        /// <param name="texCoord">The <see cref="Vertex"/>'s texture coordinates.</param>
        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            this.Normal = normal;
            this.Position = position;
            this.TexCoord = texCoord;
        }

        /// <summary>
        /// Initializes a new <see cref="Vertex"/> from an array containing the data.
        /// </summary>
        /// <param name="data">The data to load from.</param>
        public Vertex(float[] data)
            : this(data, 0)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Length >= 8);
        }

        /// <summary>
        /// Initializes a new <see cref="Vertex"/> from an array containing the data and an offset inside the array.
        /// </summary>
        /// <param name="data">The data to load from.</param>
        /// <param name="offset">The offset inside the array to start reading from.</param>
        public Vertex(float[] data, int offset)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentException>(data.Length >= offset + 8);

            this.Position.X = data[offset];
            this.Position.Y = data[offset + 1];
            this.Position.Z = data[offset + 2];
            this.Normal.X = data[offset + 3];
            this.Normal.Y = data[offset + 4];
            this.Normal.Z = data[offset + 5];
            this.TexCoord.X = data[offset + 6];
            this.TexCoord.Y = data[offset + 7];
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
            return (this.Position == other.Position) && (this.Normal == other.Normal) && (this.TexCoord == other.TexCoord);
        }

        /// <summary>
        /// Gets the <see cref="Vertex"/>'s hash code.
        /// </summary>
        /// <returns>The <see cref="Vertex"/>'s hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Position, this.Normal, this.TexCoord);
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
