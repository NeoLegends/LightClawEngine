using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class Buffer : GLObject, IBindable
    {
        private bool requiresNameGeneration = true;

        private bool requiresDataUpload = false;

        private GCHandle dataHandle;

        private IntPtr dataSize;

        public int Count { get; private set; }

        public BufferUsageHint Hint { get; private set; }

        public BufferTarget Target { get; private set; }

        public Buffer(BufferTarget target, BufferUsageHint hint)
        {
            this.Hint = hint;
            this.Target = target;
        }

        public Buffer(BufferTarget target, BufferUsageHint hint, GCHandle dataHandle, IntPtr size)
            : this(target, hint)
        {
            this.dataHandle = dataHandle;
            this.dataSize = size;
            this.requiresDataUpload = true;
        }

        public void Bind()
        {
            if (this.requiresNameGeneration) // Buffer name will be generated lazily in order to allow for multithreaded content loading
            {
                this.Handle = GL.GenBuffer();
                this.requiresNameGeneration = false;
            }
            if (this.requiresDataUpload) // Same goes for data, upload when required on drawing thread
            {
                this.Update(this.dataHandle.AddrOfPinnedObject(), this.dataSize);
                this.dataHandle.Free();
                this.dataSize = IntPtr.Zero;
                this.requiresDataUpload = false;
            }
            GL.BindBuffer(this.Target, this);
        }

        public void Unbind()
        {
            GL.BindBuffer(this.Target, 0);
        }

        public void Update<T>(T[] data)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);

            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            this.Update(dataHandle.AddrOfPinnedObject(), (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length));
            dataHandle.Free();
        }

        public void UpdateRange<T>(T[] data, int offset)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);

            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            this.UpdateRange(dataHandle.AddrOfPinnedObject(), (IntPtr)offset, (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length));
            dataHandle.Free();
        }

        public void Update(IntPtr data, IntPtr size)
        {
            using (GLBinding bufferBinding = new GLBinding(this))
            {
                GL.BufferData(this.Target, size, data, this.Hint);
                this.CalculateCount((int)size, 0);
            }
        }

        public void UpdateRange(IntPtr data, IntPtr offset, IntPtr size)
        {
            using (GLBinding bufferBindg = new GLBinding(this))
            {
                GL.BufferSubData(this.Target, offset, size, data);
                this.CalculateCount((int)size, (int)offset);
            }
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

        private void CalculateCount(int sizeOfNewData, int offset)
        {
            int difference = (sizeOfNewData - (this.Count - offset));
            this.Count = this.Count + ((difference >= 0) ? difference : 0);
        }

        public static Buffer Create<T>(T[] data, BufferTarget target)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<Buffer>() != null);

            return Create(data, target, BufferUsageHint.StaticDraw);
        }

        public static Buffer Create<T>(T[] data, BufferTarget target, BufferUsageHint hint)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<Buffer>() != null);

            return new Buffer(target, hint, GCHandle.Alloc(data, GCHandleType.Pinned), (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length));
        }
    }
}
