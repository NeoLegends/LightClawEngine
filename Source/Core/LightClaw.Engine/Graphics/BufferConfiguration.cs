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
        public Buffer IndexBuffer { get; private set; }

        public Buffer VertexBuffer { get; private set; }

        public IEnumerable<VertexAttributePointer> VertexAttributePointers { get; private set; }

        public BufferConfiguration(
                Buffer indexBuffer, 
                Buffer vertexBuffer,
                IEnumerable<VertexAttributePointer> vertexAttributePointers
            )
        {
            Contract.Requires<ArgumentNullException>(indexBuffer != null);
            Contract.Requires<ArgumentNullException>(vertexBuffer != null);
            Contract.Requires<ArgumentNullException>(vertexAttributePointers != null);

            this.IndexBuffer = indexBuffer;
            this.VertexBuffer = vertexBuffer;
            this.VertexAttributePointers = vertexAttributePointers;
        }
    }
}
