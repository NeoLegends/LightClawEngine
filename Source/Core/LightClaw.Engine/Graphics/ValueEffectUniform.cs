using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class ValueEffectUniform : EffectUniform
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

        private int _Length;

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

        private UniformBufferPool _UboPool;

        public UniformBufferPool UboPool
        {
            get
            {
                Contract.Ensures(Contract.Result<UniformBufferPool>() != null);

                return _UboPool;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _UboPool, value);
            }
        }

        public ValueEffectUniform(EffectStage stage, UniformBufferPool pool, string name, int length)
            : base(stage, name)
        {
            Contract.Requires<ArgumentNullException>(stage != null);
            Contract.Requires<ArgumentNullException>(pool != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
            Contract.Requires<ArgumentOutOfRangeException>(length > 0);

            this.Length = length;
            this.UboPool = pool;
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        throw new NotImplementedException();
                        this.Ubo = this.UboPool.GetBuffer(this.Length, GetStage(this.Stage.ShaderProgram.Type), this.Stage);
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

        public void Set<T>(T[] value)
            where T : struct
        {
            if (!this.TrySet(value))
            {
                throw new InvalidOperationException("Something bad happened while setting the data in the {0}.".FormatWith(typeof(ValueEffectUniform).Name));
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
                Logger.Warn(() => "An exception of type '{0}' was thrown while setting the data in a {1}.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(ValueEffectUniform).Name), ex);
                return false;
            }
        }

        public bool TrySet<T>(T[] value)
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
                Logger.Warn(() => "An exception of type '{0}' was thrown while setting the data in a {1}.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(ValueEffectUniform).Name), ex);
                return false;
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._UboPool != null);
        }

        private static UniformBufferPool.Stage GetStage(ShaderType type)
        {
            switch (type)
            {
                case ShaderType.FragmentShader:
                    return UniformBufferPool.Stage.Fragment;
                case ShaderType.GeometryShader:
                    return UniformBufferPool.Stage.Geometry;
                case ShaderType.VertexShader:
                    return UniformBufferPool.Stage.Vertex;
                default:
                    throw new NotSupportedException("Only fragment, geometry and vertex shader are supported.");
            }
        }
    }
}
