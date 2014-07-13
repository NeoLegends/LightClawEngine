using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public struct VertexAttributePointer
    {
        public int Index { get; private set; }

        public bool IsNormalized { get; private set; }

        public int Offset { get; private set; }

        public int Size { get; private set; }

        public int Stride { get; private set; }

        public VertexAttribPointerType Type { get; private set; }
    }
}
