using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public class BufferConfiguration
    {
        public Buffer VertexBuffer { get; private set; }

        public IEnumerable<VertexAttributePointer> VertexAttributePointers { get; private set; }

        public BufferConfiguration(Buffer vertexBuffer, IEnumerable<VertexAttributePointer> vertexAttributePointers)
        {
            Contract.Requires<ArgumentNullException>(vertexBuffer != null);
            Contract.Requires<ArgumentNullException>(vertexAttributePointers != null);

            this.VertexBuffer = vertexBuffer;
            this.VertexAttributePointers = vertexAttributePointers;
        }
    }
}
