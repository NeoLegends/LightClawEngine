using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class ValueEffectUniform : EffectUniform, IBindable
    {
        private readonly object initializationLock = new object();

        private bool _IsInitialized;

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

        private int _Length = 0;

        public int Length
        {
            get
            {
                return _Length;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value > 0);

                this.SetProperty(ref _Length, value);
            }
        }

        private RangedBuffer _Ubo;

        public RangedBuffer Ubo
        {
            get
            {
                return _Ubo;
            }
            private set
            {
                this.SetProperty(ref _Ubo, value);
            }
        }

        public int UboBindingIndex
        {
            get
            {
                RangedBuffer ubo = this.Ubo;
                return (ubo != null) ? this.Ubo.Index : -1;
            }
        }

        public ValueEffectUniform(EffectStage stage, string name)
            : base(stage, name)
        {
            Contract.Requires<ArgumentNullException>(stage != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
        }

        public void Bind()
        {
            RangedBuffer buffer = this.Ubo;
            if (buffer != null)
            {
                buffer.Bind();
            }
        }

        public void Initialize<T>()
            where T : struct
        {
            this.Initialize<T>(1);
        }

        public void Initialize<T>(int count)
            where T : struct
        {
            Contract.Requires<ArgumentOutOfRangeException>(count > 0);

            int allocationSize = Marshal.SizeOf(typeof(T)) * count;
            if (!this.IsInitialized || this.Length != allocationSize)
            {
                lock (this.initializationLock)
                {
                    if (this.Length != allocationSize)
                    {
                        this.Length = allocationSize;
                        this.Ubo = this.Stage.Pass.UniformBufferManager.GetBuffer(allocationSize);
                    }
                    if (!this.IsInitialized)
                    {
                        this.Location = GL.GetUniformBlockIndex(this.Stage.ShaderProgram, this.Name);
                        GL.UniformBlockBinding(this.Stage.ShaderProgram, this.Location, this.Ubo.Index);
                    }
                }
            }
        }

        public void Set<T>(T value)
            where T : struct
        {
            if (!this.TrySet(value))
            {
                throw new InvalidOperationException("Something bad happened while setting the data in the {0}.".FormatWith(typeof(ValueEffectUniform).Name));
            }
        }

        public void Set<T>(T[] values)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentException>(values.Length > 0);

            if (!this.TrySet(values))
            {
                throw new InvalidOperationException("Something bad happened while setting the data in the {0}.".FormatWith(typeof(ValueEffectUniform).Name));
            }
        }

        public bool TrySet<T>(T value)
            where T : struct
        {
            this.Initialize<T>();
            try
            {
                RangedBuffer buffer = this.Ubo;
                if (buffer != null)
                {
                    buffer.Set(value);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Warn(() => "An exception of type '{0}' was thrown while setting the data in a {1}.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(ValueEffectUniform).Name), ex);
                return false;
            }
        }

        public bool TrySet<T>(T[] values)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentException>(values.Length > 0);

            this.Initialize<T>(values.Length);
            try
            {
                RangedBuffer buffer = this.Ubo;
                if (buffer != null)
                {
                    buffer.Set(values);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Warn(() => "An exception of type '{0}' was thrown while setting the data in a {1}.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(ValueEffectUniform).Name), ex);
                return false;
            }
        }

        public void Unbind()
        {
            RangedBuffer buffer = this.Ubo;
            if (buffer != null)
            {
                buffer.Unbind();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                this.Ubo.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}
