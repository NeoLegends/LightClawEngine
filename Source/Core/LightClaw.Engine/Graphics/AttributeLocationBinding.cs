using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents the binding of a shader variable index to its name.
    /// </summary>
    /// <remarks>Used together with <see cref="ShaderProgram"/>s.</remarks>
    [ProtoContract]
    public struct AttributeLocationBinding
    {
        /// <summary>
        /// The index of the shader variable.
        /// </summary>
        [ProtoMember(1)]
        public int Index { get; private set; }

        /// <summary>
        /// The name of the shader variable.
        /// </summary>
        [ProtoMember(2)]
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="AttributeLocationBinding"/>.
        /// </summary>
        /// <param name="index">The index of the shader variable.</param>
        /// <param name="name">The name of the shader variable.</param>
        public AttributeLocationBinding(int index, string name)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentNullException>(name != null);

            this.Index = index;
            this.Name = name;
        }
    }
}
