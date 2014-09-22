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

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a data store on GPU memory.
    /// </summary>
    public class Buffer : GLObject, IBuffer, IInitializable
    {
        /// <summary>
        /// Used for restricting access to the name generation process.
        /// </summary>
        private readonly object nameGenerationLock = new object();

        /// <summary>
        /// Backing field.
        /// </summary>
        private int _Count;

        /// <summary>
        /// The length of the buffer in bytes.
        /// </summary>
        public int Count
        {
            get
            {
                return _Count;
            }
            private set
            {
                this.SetProperty(ref _Count, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private BufferUsageHint _Hint;

        /// <summary>
        /// The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="Buffer"/>.
        /// </summary>
        public BufferUsageHint Hint
        {
            get
            {
                return _Hint;
            }
            private set
            {
                this.SetProperty(ref _Hint, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _IsInitialized;

        /// <summary>
        /// Indicates whether the <see cref="Buffer"/> is already initialized and has got a name.
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            private set
            {
                this.SetProperty(ref _IsInitialized, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private BufferTarget _Target;

        /// <summary>
        /// The <see cref="BufferTarget"/>.
        /// </summary>
        public BufferTarget Target
        {
            get
            {
                return _Target;
            }
            private set
            {
                this.SetProperty(ref _Target, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Buffer"/> setting usage hint and target.
        /// </summary>
        /// <param name="target">The <see cref="BufferTarget"/>.</param>
        /// <param name="hint">The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="Buffer"/>.</param>
        public Buffer(BufferTarget target, BufferUsageHint hint)
        {
            this.Hint = hint;
            this.Target = target;
        }

        /// <summary>
        /// Binds the buffer to the specified <see cref="BufferTarget"/>.
        /// </summary>
        public void Bind()
        {
            this.Initialize();
            GL.BindBuffer(this.Target, this);
        }

        /// <summary>
        /// Gets all of the <see cref="Buffer"/>s data.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of result to read the data into.</typeparam>
        /// <returns>The <see cref="Buffer"/>s data.</returns>
        public T[] Get<T>()
            where T : struct
        {
            return GetRange<T>(0, this.Count);
        }

        /// <summary>
        /// Gets a range of the <see cref="Buffer"/>s data.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of result to read the data into.</typeparam>
        /// <param name="offset">The starting index.</param>
        /// <param name="count">The amount of bytes to read.</param>
        /// <returns>The <see cref="Buffer"/>s data.</returns>
        public T[] GetRange<T>(int offset, int count)
            where T : struct
        {
            T[] results = new T[count];
            using (Binding bufferBinding = new Binding(this))
            {
                GL.GetBufferSubData(this.Target, (IntPtr)offset, (IntPtr)count, results);
            }

            return results;
        }

        /// <summary>
        /// Initializes the <see cref="Buffer"/>.
        /// </summary>
        public void Initialize()
        {
            if (!this.IsInitialized) // Buffer name will be generated lazily in order to allow for multithreaded content loading
            {
                lock (this.nameGenerationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Handle = GL.GenBuffer();
                        this.IsInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Unbinds the buffer from the current <see cref="BufferTarget"/>.
        /// </summary>
        public void Unbind()
        {
            GL.BindBuffer(this.Target, 0);
        }

        /// <summary>
        /// Updates the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the new data.</typeparam>
        /// <param name="data">The data itself.</param>
        public virtual void Set<T>(T data)
            where T : struct
        {
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                this.Set(dataHandle.AddrOfPinnedObject(), Marshal.SizeOf(typeof(T)));
            }
            finally
            {
                dataHandle.Free();
            }
        }

        /// <summary>
        /// Updates the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the new data.</typeparam>
        /// <param name="data">The data itself.</param>
        public virtual void Set<T>(IEnumerable<T> data)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Any());

            this.Set(data.ToArray());
        }

        /// <summary>
        /// Updates the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the new data.</typeparam>
        /// <param name="data">The data itself.</param>
        public virtual void Set<T>(T[] data)
            where T : struct
        {
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                this.Set(dataHandle.AddrOfPinnedObject(), Marshal.SizeOf(typeof(T)) * data.Length);
            }
            finally
            {
                dataHandle.Free();
            } 
        }

        /// <summary>
        /// Sets a range of the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the new data.</typeparam>
        /// <param name="data">The data itself.</param>
        /// <param name="offset">The offset in bytes to start applying the new data at.</param>
        public virtual void SetRange<T>(T data, int offset)
            where T : struct
        {
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                this.SetRange(dataHandle.AddrOfPinnedObject(), offset, Marshal.SizeOf(typeof(T)));
            }
            finally
            {
                dataHandle.Free();
            }
        }

        /// <summary>
        /// Updates a range of the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the new data.</typeparam>
        /// <param name="data">The data itself.</param>
        /// <param name="offset">The offset in bytes to start applying the new data at.</param>
        public virtual void SetRange<T>(IEnumerable<T> data, int offset)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentNullException>(offset >= 0);
            Contract.Requires<ArgumentException>(data.Any());

            this.SetRange(data.ToArray(), offset);
        }

        /// <summary>
        /// Updates a range of the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the new data.</typeparam>
        /// <param name="data">The data itself.</param>
        /// <param name="offset">The offset in bytes to start applying the new data at.</param>
        public virtual void SetRange<T>(T[] data, int offset)
            where T : struct
        {
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                this.SetRange(dataHandle.AddrOfPinnedObject(), offset, Marshal.SizeOf(typeof(T)) * data.Length);
            }
            finally
            {
                dataHandle.Free();
            }
        }

        /// <summary>
        /// Updates the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <param name="sizeInBytes">The size of the data in bytes.</param>
        public virtual void Set(IntPtr data, int sizeInBytes)
        {
            this.Initialize();
            using (Binding bufferBinding = new Binding(this))
            {
                GL.BufferData(this.Target, (IntPtr)sizeInBytes, data, this.Hint);
            }
            this.CalculateCount(sizeInBytes, 0);
        }

        /// <summary>
        /// Updates the <see cref="Buffer"/>'s contents.
        /// </summary>
        /// <param name="data">The data itself.</param>
        /// <param name="sizeInBytes">The size of the data in bytes.</param>
        /// <param name="offset">The offset in bytes to start applying the new data at.</param>
        public virtual void SetRange(IntPtr data, int offset, int sizeInBytes)
        {
            this.Initialize();
            using (Binding bufferBinding = new Binding(this))
            {
                GL.BufferSubData(this.Target, (IntPtr)offset, (IntPtr)sizeInBytes, data);
            }
            this.CalculateCount(sizeInBytes, offset);
        }

        /// <summary>
        /// Disposes the <see cref="Buffer"/> removing it from the GPU memory.
        /// </summary>
        /// <param name="disposing">A boolean indicating whether to dispose managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                try
                {
                    GL.DeleteBuffer(this);
                }
                catch (Exception ex)
                {
                    Logger.Warn(() => "An exception of type '{0}' occured while disposing the {1}'s underlying OpenGL Buffer.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(Buffer).Name), ex);
                }
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Recalculates the size of the <see cref="Buffer"/> after a change.
        /// </summary>
        /// <param name="sizeOfNewData">The size of the new data in bytes.</param>
        /// <param name="offset">The offset of the new data.</param>
        private void CalculateCount(int sizeOfNewData, int offset)
        {
            Contract.Requires<ArgumentOutOfRangeException>(sizeOfNewData >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);

            int difference = (sizeOfNewData - (this.Count - offset));
            this.Count = this.Count + ((difference >= 0) ? difference : 0);
        }

        /// <summary>
        /// Creates a new buffer from an array.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of data to upload.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="target">The <see cref="BufferTarget"/> the <see cref="Buffer"/> will be bound to.</param>
        /// <returns>The newly created <see cref="Buffer"/>.</returns>
        public static Buffer Create<T>(T[] data, BufferTarget target)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<Buffer>() != null);

            return Create(data, target, BufferUsageHint.StaticDraw);
        }

        /// <summary>
        /// Creates a new buffer from an array.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of data to upload.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="target">The <see cref="BufferTarget"/> the <see cref="Buffer"/> will be bound to.</param>
        /// <param name="hint">The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="Buffer"/>.</param>
        /// <returns>The newly created <see cref="Buffer"/>.</returns>
        public static Buffer Create<T>(T[] data, BufferTarget target, BufferUsageHint hint)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<Buffer>() != null);

            Buffer buffer = new Buffer(target, hint);
            buffer.Set(data);
            return buffer;
        }
    }
}
