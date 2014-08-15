using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public class OutOfUniformBufferSpaceException : OutOfMemoryException
    {
        public OutOfUniformBufferSpaceException() { }

        public OutOfUniformBufferSpaceException(string message) : base(message) { }

        public OutOfUniformBufferSpaceException(string message, Exception inner) : base(message, inner) { }
    }
}
