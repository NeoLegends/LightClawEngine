using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public class ValueEffectUniform : EffectUniform
    {
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

        public void Set<T>(T value)
            where T : struct
        {
            RangedBuffer buffer = this.Ubo;
            if (buffer != null)
            {
                buffer.Set(value);
            }
        }

        public void Set<T>(T[] value)
            where T : struct
        {
            RangedBuffer buffer = this.Ubo;
            if (buffer != null)
            {
                buffer.Set(value);
            }
        }

        public bool TrySet<T>(T value)
            where T : struct
        {
            try
            {
                RangedBuffer buffer = this.Ubo;
                if (buffer != null)
                {
                    buffer.Set(value);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TrySet<T>(T[] value)
            where T : struct
        {
            try
            {
                RangedBuffer buffer = this.Ubo;
                if (buffer != null)
                {
                    buffer.Set(value);
                }
                return true;
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
