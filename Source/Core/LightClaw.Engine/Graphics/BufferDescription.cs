using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public struct BufferDescription
    {
        public Buffer VertexBuffer { get; private set; }

        public IEnumerable<VertexAttributePointer> VertexAttributePointers { get; private set; }

        public BufferDescription(Buffer vertexBuffer, IEnumerable<VertexAttributePointer> vertexAttributePointers)
            : this()
        {
            Contract.Requires<ArgumentNullException>(vertexBuffer != null);
            Contract.Requires<ArgumentNullException>(vertexAttributePointers != null);

            this.VertexBuffer = vertexBuffer;
            this.VertexAttributePointers = vertexAttributePointers;
        }
    }
}
