﻿using System;
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
        private static readonly ILog logger = LogManager.GetLogger(typeof(Buffer));

        public int Count { get; private set; }

        public BufferUsageHint Hint { get; private set; }

        public BufferTarget Target { get; private set; }

        public Type Type { get; private set; }

        public Buffer(BufferTarget target, BufferUsageHint hint)
            : base(GL.GenBuffer())
        {
            logger.Debug("Initializing a new {0} for {2}".FormatWith(target, hint));

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

        public void Update<T>(T data)
            where T : struct
        {
            Contract.Requires<ArgumentException>(this.Type == null || typeof(T) == this.Type);

            this.Update(data, 0);
        }

        public void Update<T>(T data, int offsetInBytes)
            where T : struct
        {
            Contract.Requires<ArgumentException>(this.Type == null || typeof(T) == this.Type);

            this.CalculateCount(1, Marshal.SizeOf(typeof(T)), offsetInBytes);
            this.Type = typeof(T);
            using (GLBinding bufferBinding = new GLBinding(this))
            {
                GL.BufferSubData(this.Target, (IntPtr)offsetInBytes, (IntPtr)Marshal.SizeOf(typeof(T)), ref data);
            }
        }

        public void Update<T>(T[] data)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(this.Type == null || typeof(T) == this.Type);

            this.UpdateRange(data, 0);
        }

        public void UpdateRange<T>(T[] data, int offsetInBytes)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(offsetInBytes >= 0);
            Contract.Requires<ArgumentException>(this.Type == null || typeof(T) == this.Type);

            this.CalculateCount(data.Length, Marshal.SizeOf(typeof(T)), offsetInBytes);
            this.Type = typeof(T);
            using (GLBinding bufferBinding = new GLBinding(this))
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

        private void CalculateCount(int count, int sizeOfNewData, int offset)
        {
            int difference = (count * sizeOfNewData - (this.Count - offset));
            this.Count = this.Count + ((difference >= 0) ? difference : 0);
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

            Buffer b = new Buffer(target, hint);
            b.Update(data);
            return b;
        }
    }
}
