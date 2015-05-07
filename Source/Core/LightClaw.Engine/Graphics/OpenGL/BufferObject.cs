using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents a data store on GPU memory.
    /// </summary>
    /// <remarks>
    /// <see cref="BufferObject"/> is named <see cref="BufferObject"/> because naming it "Buffer" would conflict
    /// with <see cref="System.Buffer"/>. Visual Studio gets confused when setting breakpoints and you'd have to 
    /// redefine it in every class you include the System-namespace anyway.
    /// </remarks>
    [DebuggerDisplay("Target = {Target}, Usage Hint = {Hint}, Length = {Length}")]
    public class BufferObject : GLObject, IBuffer
    {
        /// <summary>
        /// Used for restricting access to the name generation process.
        /// </summary>
        private readonly object nameGenerationLock = new object();

        /// <summary>
        /// Backing field.
        /// </summary>
        private BufferUsageHint _Hint;

        /// <summary>
        /// The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="BufferObject"/>.
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
        private int _Length;

        /// <summary>
        /// The length of the buffer in bytes.
        /// </summary>
        public int Length
        {
            get
            {
                return _Length;
            }
            private set
            {
                this.SetProperty(ref _Length, value);
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
        /// Initializes a new <see cref="BufferObject"/> setting usage hint and target.
        /// </summary>
        /// <param name="target">The <see cref="BufferTarget"/>.</param>
        /// <param name="hint">
        /// The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="BufferObject"/>.
        /// </param>
        public BufferObject(BufferTarget target, BufferUsageHint hint)
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
        /// Gets all of the <see cref="BufferObject"/>s data.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of result to read the data into.</typeparam>
        /// <returns>The <see cref="BufferObject"/>s data.</returns>
        public T[] Get<T>()
            where T : struct
        {
            return GetRange<T>(0, this.Length);
        }

        /// <summary>
        /// Gets a range of the <see cref="BufferObject"/>s data.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of result to read the data into.</typeparam>
        /// <param name="offset">The starting index.</param>
        /// <param name="count">The amount of bytes to read.</param>
        /// <returns>The <see cref="BufferObject"/>s data.</returns>
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
        /// Binds the buffer and maps it with <see cref="BufferAccess.ReadWrite"/>.
        /// </summary>
        /// <remarks>Important, this method binds the buffer!</remarks>
        /// <returns>An <see cref="IntPtr"/> pointing to the data inside the buffer.</returns>
        public IntPtr Map()
        {
            return this.Map(BufferAccess.ReadWrite);
        }

        /// <summary>
        /// Maps it with the specified <paramref name="bufferAccess"/>. See remarks.
        /// </summary>
        /// <remarks>Expects the buffer to be bound.</remarks>
        /// <param name="bufferAccess">A <see cref="BufferAccess"/> enum describing the access capabilities.</param>
        /// <returns>An <see cref="IntPtr"/> pointing to the data inside the buffer.</returns>
        public IntPtr Map(BufferAccess bufferAccess)
        {
            return GL.MapBuffer(this.Target, bufferAccess);
        }

        /// <summary>
        /// Unbinds the buffer from the current <see cref="BufferTarget"/>.
        /// </summary>
        public void Unbind()
        {
            GL.BindBuffer(this.Target, 0);
        }

        /// <summary>
        /// Unmaps the buffer.
        /// </summary>
        /// <remarks>Important, this method expects the current buffer to be bound.</remarks>
        public void Unmap()
        {
            GL.UnmapBuffer(this.Target);
        }

        /// <summary>
        /// Updates the <see cref="BufferObject"/>'s contents.
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
        /// Updates the <see cref="BufferObject"/>'s contents.
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
        /// Sets a range of the <see cref="BufferObject"/>'s contents.
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
        /// Updates a range of the <see cref="BufferObject"/>'s contents.
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
        /// Updates the <see cref="BufferObject"/>'s contents.
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
            this.Length = sizeInBytes;
        }

        /// <summary>
        /// Updates the <see cref="BufferObject"/>'s contents.
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
            int difference = (sizeInBytes - (this.Length - offset));
            this.Length = this.Length + ((difference >= 0) ? difference : 0);
        }

        /// <summary>
        /// Disposes the <see cref="BufferObject"/> removing it from the GPU memory.
        /// </summary>
        /// <param name="disposing">A boolean indicating whether to dispose managed resources as well.</param>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        protected override void Dispose(bool disposing)
        {
            lock (this.nameGenerationLock)
            {
                if (this.IsInitialized)
                {
                    try
                    {
                        GL.DeleteBuffer(this);
                    }
                    catch (AccessViolationException ex)
                    {
                        Log.Warn("An {0} was thrown while disposing of a {1}. This might or might not be an unwanted condition.".FormatWith(ex.GetType().Name, typeof(BufferObject).Name), ex);
                    }
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialization callback.
        /// </summary>
        protected override void OnInitialize()
        {
            Log.Debug(() => "Initializing {0}.".FormatWith(typeof(BufferObject).Name));

            this.Handle = GL.GenBuffer();
        }

        /// <summary>
        /// Creates a new buffer from an array.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of data to upload.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="target">The <see cref="BufferTarget"/> the <see cref="BufferObject"/> will be bound to.</param>
        /// <returns>The newly created <see cref="BufferObject"/>.</returns>
        public static BufferObject Create<T>(T[] data, BufferTarget target)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<BufferObject>() != null);

            return Create(data, target, BufferUsageHint.StaticDraw);
        }

        /// <summary>
        /// Creates a new buffer from an array.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of data to upload.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="target">The <see cref="BufferTarget"/> the <see cref="BufferObject"/> will be bound to.</param>
        /// <param name="hint">
        /// The <see cref="BufferUsageHint"/> hinting the desired way of using the <see cref="BufferObject"/>.
        /// </param>
        /// <returns>The newly created <see cref="BufferObject"/>.</returns>
        public static BufferObject Create<T>(T[] data, BufferTarget target, BufferUsageHint hint)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<BufferObject>() != null);

            BufferObject buffer = new BufferObject(target, hint);
            buffer.Set(data);
            return buffer;
        }
    }
}
