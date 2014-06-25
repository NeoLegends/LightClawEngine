using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    public class Buffer<T> : GLObject
        where T : struct
    {
        private BufferTarget target;

        private int _Count = 0;

        public int Count
        {
            get
            {
                return _Count;
            }
            private set
            {
                _Count = value;
            }
        }

        public Buffer(T[] data)
            : this(data, BufferTarget.ArrayBuffer)
        {
            Contract.Requires<ArgumentNullException>(data != null);
        }

        public Buffer(T[] data, BufferTarget target)
            : this(data, target, BufferUsageHint.StaticDraw)
        {
            Contract.Requires<ArgumentNullException>(data != null);
        }

        public Buffer(T[] data, BufferTarget target, BufferUsageHint hint)
        {
            Contract.Requires<ArgumentNullException>(data != null);

            this.Count = data.Length;
            this.target = target;
            this.Id = GL.GenBuffer();

            GL.BindBuffer(target, this);
            GL.BufferData(target, (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length), data, hint);
        }

        public void Bind()
        {
            GL.BindBuffer(this.target, this);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.DeleteBuffer(this);
            }
            catch (AccessViolationException)
            {
                throw; // Log and swallow in the future!
            }
            base.Dispose(disposing);
        }
    }
}
