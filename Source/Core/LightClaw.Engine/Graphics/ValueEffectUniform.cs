using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public class ValueEffectUniform : EffectUniform
    {
        private RangedBuffer _Ubo;

        public RangedBuffer Ubo
        {
            get
            {
                return _Ubo;
            }
            set
            {
                this.SetProperty(ref _Ubo, value);
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
    }
}
