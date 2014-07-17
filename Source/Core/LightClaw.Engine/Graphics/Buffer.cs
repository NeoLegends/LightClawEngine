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
    public abstract class Buffer : GLObject, IBindable
    {
        public BufferTarget Target { get; protected set; }

        protected Buffer() { }

        protected Buffer(BufferTarget target) : this(0, target) { }

        protected Buffer(int id, BufferTarget target)
            : base(id)
        {
            this.Target = target;
        }

        public void Bind()
        {
            GL.BindBuffer(this.Target, this);
        }

        public void Unbind()
        {
            GL.BindBuffer(this.Target, 0);
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

    public class Buffer<T> : Buffer
        where T : struct
    {
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
            : base(GL.GenBuffer(), target)
        {
            Contract.Requires<ArgumentNullException>(data != null);

            this.Count = data.Length;

            GL.BindBuffer(target, this);
            GL.BufferData(target, (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length), data, hint);
            GL.BindBuffer(target, 0);
        }
    }
}
