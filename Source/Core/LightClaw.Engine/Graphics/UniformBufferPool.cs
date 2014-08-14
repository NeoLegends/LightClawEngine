using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public class UniformBufferPool : Entity
    {
        private static readonly UniformBufferPool _Default = new UniformBufferPool();

        public static UniformBufferPool Default
        {
            get
            {
                Contract.Ensures(Contract.Result<UniformBufferPool>() != null);

                return _Default;
            }
        }
    }
}
