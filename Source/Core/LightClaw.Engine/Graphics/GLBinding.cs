using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public struct GLBinding : IDisposable, IBindable
    {
        private readonly IBindable bindable;

        public GLBinding(IBindable bindable, bool bindImmediately = true)
        {
            Contract.Requires<ArgumentNullException>(bindable != null);

            this.bindable = bindable;
            if (bindImmediately)
            {
                this.Bind();
            }
        }

        public void Bind()
        {
            this.bindable.Bind();
        }

        public void Unbind()
        {
            this.bindable.Unbind();
        }

        void IDisposable.Dispose()
        {
            this.Unbind();
        }
    }
}
