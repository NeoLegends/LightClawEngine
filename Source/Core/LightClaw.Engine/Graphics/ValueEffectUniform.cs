using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class ValueEffectUniform : EffectUniform
    {
        private readonly object initializationLock = new object();

        private int _Index;

        public int Index
        {
            get
            {
                return _Index;
            }
            private set
            {
                this.SetProperty(ref _Index, value);
            }
        }

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
                        this.Ubo = this.UboPool.GetBuffer(this.Length, UniformBufferPool.Stage.Fragment, this.Stage);
                    }
                }
            }
        }

        public void Set<T>(T value)
            where T : struct
        {
            this.Initialize();
            RangedBuffer buffer = this.Ubo;
            if (buffer == null)
            {
                throw new NullReferenceException("The uniform buffer was null.");
            }
            buffer.Set(value);
        }

        public void Set<T>(T[] value)
            where T : struct
        {
            this.Initialize();
            RangedBuffer buffer = this.Ubo;
            if (buffer == null)
            {
                throw new NullReferenceException("The uniform buffer was null.");
            }
            buffer.Set(value);
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
            catch
            {
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
            catch
            {
                return false;
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._UboPool != null);
        }
    }
}
