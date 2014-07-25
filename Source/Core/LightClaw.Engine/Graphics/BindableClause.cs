using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public struct BindableClause : IDisposable
    {
        private readonly IBindable bindable;

        public BindableClause(IBindable bindable)
            : this()
        {
            Contract.Requires<ArgumentNullException>(bindable != null);

            this.bindable = bindable;
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
