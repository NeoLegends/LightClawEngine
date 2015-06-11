using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a shader program attribute.
    /// </summary>
    [DebuggerDisplay("Name: {Name}, Location: {Location}, Type: {Type}")]
    public class ProgramAttribute : Entity
    {
        private int _Location;

        /// <summary>
        /// The uniform's location.
        /// </summary>
        public int Location
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _Location;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _Location, value);
            }
        }

        /// <summary>
        /// The attributes name.
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                throw new NotSupportedException("The {0}s name cannot be set. It is hardcoded in the shader file.".FormatWith(typeof(ProgramAttribute).Name));
            }
        }

        private ShaderProgram _Program;

        /// <summary>
        /// The <see cref="ShaderProgram"/> the <see cref="ProgramAttribute"/> belongs to.
        /// </summary>
        public ShaderProgram Program
        {
            get
            {
                Contract.Ensures(Contract.Result<ShaderProgram>() != null);

                return _Program;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Program, value);
            }
        }

        private ActiveAttribType _Type;

        /// <summary>
        /// The <see cref="ProgramAttribute"/>s type.
        /// </summary>
        public ActiveAttribType Type
        {
            get
            {
                return _Type;
            }
            private set
            {
                this.SetProperty(ref _Type, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="ProgramAttribute"/>.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/> the <see cref="ProgramAttribute"/> belongs to.</param>
        /// <param name="index">The uniform index as per GL.GetProgramInterface.</param>
        public ProgramAttribute(ShaderProgram program, int index)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            this.Program = program;

            ActiveAttribType aat;
            int size;
            base.Name = GL.GetActiveAttrib(program, index, out size, out aat);
            this.Location = GL.GetAttribLocation(program, this.Name);
            this.Type = aat;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj as ProgramAttribute);
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns><c>true</c> if the objects are equal, otherwise <c>false</c>.</returns>
        public bool Equals(ProgramAttribute other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(other, this))
                return true;

            return (this.Location == other.Location) && (this.Name == other.Name) && (this.Program == other.Program);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Location, this.Name, this.Program);
        }

        /// <summary>
        /// Explicitly gets the <see cref="Location"/> of a <see cref="ProgramAttribute"/>.
        /// </summary>
        /// <param name="attr">The attribute.</param>
        /// <returns>The attribute location.</returns>
        public static explicit operator int(ProgramAttribute attr)
        {
            Contract.Requires<ArgumentNullException>(attr != null);

            return attr.Location;
        }

        /// <summary>
        /// Checks whether two <see cref="ProgramAttribute"/>s are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="ProgramAttribute"/>s are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(ProgramAttribute left, ProgramAttribute right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="ProgramAttribute"/>s are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="ProgramAttribute"/>s are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(ProgramAttribute left, ProgramAttribute right)
        {
            return !(left == right);
        }
    }
}
