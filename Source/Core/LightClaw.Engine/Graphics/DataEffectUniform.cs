using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class DataEffectUniform : EffectUniform, IBindable
    {
        private int _Size;

        public int Size
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() > 0);

                return _Size;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value > 0);

                this.SetProperty(ref _Size, value);
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

        public DataEffectUniform(EffectPass stage, string name, int size)
            : base(stage, name)
        {
            Contract.Requires<ArgumentNullException>(stage != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
            Contract.Requires<ArgumentOutOfRangeException>(size > 0);

            this.Size = size;
        }

        public override void Bind()
        {
            RangedBuffer buffer = this.Ubo;
            if (buffer != null)
            {
                buffer.Bind();
            }
            else
            {
                Logger.Warn(() => "The buffer of value uniform '{0}' to bind is null. This is presumably unwanted behaviour!".FormatWith(this.UniformName));
            }
        }

        public void Set<T>(T value)
            where T : struct
        {
            if (!this.TrySet(value))
            {
                throw new InvalidOperationException("Something bad happened while setting the data in the {0}.".FormatWith(typeof(DataEffectUniform).Name));
            }
        }

        public void Set<T>(T value, int index)
            where T : struct
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            if (!this.TrySet(value, index))
            {
                throw new InvalidOperationException("Something bad happened while setting the data in the {0}.".FormatWith(typeof(DataEffectUniform).Name));
            }
        }

        public void Set<T>(T[] values)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentException>(values.Length > 0);

            if (!this.TrySet(values))
            {
                throw new InvalidOperationException("Something bad happened while setting the data in the {0}.".FormatWith(typeof(DataEffectUniform).Name));
            }
        }

        public void Set<T>(T[] values, int index)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentException>(values.Length > 0);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            if (!this.TrySet(values, index))
            {
                throw new InvalidOperationException("Something bad happened while setting the data in the {0}.".FormatWith(typeof(DataEffectUniform).Name));
            }
        }

        public bool TrySet<T>(T value)
            where T : struct
        {
            this.Initialize();
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
                Logger.Warn(() => "An exception of type '{0}' was thrown while setting the data in a {1}.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(DataEffectUniform).Name), ex);
                return false;
            }
        }

        public bool TrySet<T>(T value, int index)
            where T : struct
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            this.Initialize();
            try
            {
                RangedBuffer buffer = this.Ubo;
                if (buffer != null)
                {
                    buffer.SetRange(value, Marshal.SizeOf(typeof(T)) * index);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Warn(() => "An exception of type '{0}' was thrown while setting the data in a {1}.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(DataEffectUniform).Name), ex);
                return false;
            }
        }

        public bool TrySet<T>(T[] values)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentException>(values.Length > 0);

            this.Initialize();
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
                Logger.Warn(() => "An exception of type '{0}' was thrown while setting the data in a {1}.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(DataEffectUniform).Name), ex);
                return false;
            }
        }

        public bool TrySet<T>(T[] values, int index)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentException>(values.Length > 0);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            this.Initialize();
            try
            {
                RangedBuffer buffer = this.Ubo;
                if (buffer != null)
                {
                    buffer.SetRange(values, Marshal.SizeOf(typeof(T)) * index);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Warn(() => "An exception of type '{0}' was thrown while setting the data in a {1}.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(DataEffectUniform).Name), ex);
                return false;
            }
        }

        public override void Unbind()
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

        protected override void OnInitialize()
        {
            throw new NotImplementedException();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Size > 0);
        }
    }
}
