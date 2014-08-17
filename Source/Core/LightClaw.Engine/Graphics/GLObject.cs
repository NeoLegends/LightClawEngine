﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents the base class for all OpenGL-wrapper-objects.
    /// </summary>
    [DataContract]
    public abstract class GLObject : Entity, IGLObject
    {
        /// <summary>
        /// Backing field.
        /// </summary>
        private int _Handle;

        /// <summary>
        /// The OpenGL-handle.
        /// </summary>
        [IgnoreDataMember]
        public int Handle
        {
            get
            {
                return _Handle;
            }
            protected set
            {
                this.SetProperty(ref _Handle, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _IsDisposed;

        /// <summary>
        /// Indicates whether the instance has already been disposed or not.
        /// </summary>
        [IgnoreDataMember]
        public bool IsDisposed
        {
            get
            {
                return _IsDisposed;
            }
            private set
            {
                this.SetProperty(ref _IsDisposed, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GLObject"/>.
        /// </summary>
        protected GLObject() { }

        /// <summary>
        /// Initializes a new <see cref="GLObject"/> setting the object's handle.
        /// </summary>
        /// <param name="handle">The object's associated OpenGL-handle.</param>
        protected GLObject(int handle)
            : this()
        {
            this.Handle = handle;
        }

        /// <summary>
        /// Finalizes the object and frees all unmanaged resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~GLObject()
	    {
            this.Dispose(false);
	    }

        /// <summary>
        /// Disposes the <see cref="GLObject"/> freeing all unmanaged and managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Protected dispose-callback freeing all unmanaged and optionally managed resources as well.
        /// </summary>
        /// <param name="disposing">Indicates whether to free managed resources as well.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Handle = 0;
            }
            this.IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Retreives the <see cref="GLObject"/>'s handle via implicit conversion.
        /// </summary>
        /// <param name="glObject">The <see cref="GLObject"/> to obtain the handle of.</param>
        /// <returns>The <see cref="GLObject"/>'s handle or <c>0</c>, if <paramref name="glObject"/> was <c>null</c>.</returns>
        public static implicit operator int(GLObject glObject)
        {
            return (glObject != null) ? glObject.Handle : 0;
        }
    }
}
