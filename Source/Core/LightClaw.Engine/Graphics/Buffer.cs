using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class Buffer : GLObject, IBindable
    {
        private static ILog logger = LogManager.GetLogger(typeof(Buffer));

        public int Count { get; private set; }

        public BufferUsageHint Hint { get; private set; }

        public BufferTarget Target { get; private set; }

        private Buffer(int id, int count, BufferTarget target, BufferUsageHint hint)
            : base(id)
        {
            logger.Debug("Initializing a new {0} for tar containing {1} elements. Hint: {2}".FormatWith(target, count, hint));

            this.Count = count;
            this.Hint = hint;
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

        public void Update<T>(T[] data)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);

            using (BindableClause releaser = new BindableClause(this))
            {
                GL.BufferData(this.Target, (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length), data, this.Hint);
            }
        }

        public void UpdateRange<T>(T[] data, int offsetInBytes)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(offsetInBytes >= 0);

            using (BindableClause releaser = new BindableClause(this))
            {
                GL.BufferSubData(this.Target, (IntPtr)offsetInBytes, (IntPtr)(Marshal.SizeOf(typeof(T)) * data.Length), data);
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

        public static Buffer Create<T>(T[] data)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<Buffer>() != null);

            return Create(data, BufferTarget.ArrayBuffer);
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

            Buffer b = new Buffer(GL.GenBuffer(), data.Length, target, hint);
            b.Update(data);
            return b;
        }
    }
}
