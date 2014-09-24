using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public class UniformBufferGroup : DisposableEntity
    {
        private static readonly UniformBufferGroup _Default = new UniformBufferGroup();

        public static UniformBufferGroup Default
        {
            get
            {
                Contract.Ensures(Contract.Result<UniformBufferGroup>() != null);

                return _Default;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                base.Dispose(disposing);
            }
        }
    }
}
