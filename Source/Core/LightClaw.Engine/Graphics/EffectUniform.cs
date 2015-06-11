using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Extensions;

namespace LightClaw.Engine.Graphics
{
    public abstract class EffectUniform : DispatcherEntity, IBindable
    {
        public int Location
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return this.Uniform.Location;
            }
        }

        public override string Name
        {
            get
            {
                return this.Uniform.Name;
            }
            set
            {
                throw new NotSupportedException("{0}'s name cannot be set. It is hardcoded in the shader file".FormatWith(typeof(EffectUniform).Name));
            }
        }

        private EffectPass _Pass;

        public EffectPass Pass
        {
            get
            {
                return _Pass;
            }
            protected set
            {
                this.SetProperty(ref _Pass, value);
            }
        }

        private ProgramUniform _Uniform;

        public ProgramUniform Uniform
        {
            get
            {
                Contract.Ensures(Contract.Result<ProgramUniform>() != null);

                return _Uniform;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Uniform, value);
            }
        }

        protected EffectUniform(EffectPass pass, ProgramUniform uniform)
        {
            Contract.Requires<ArgumentNullException>(pass != null);
            Contract.Requires<ArgumentNullException>(uniform != null);

            this.Pass = pass;
            this.Uniform = uniform;
        }

        public abstract Binding Bind();

        public abstract void Unbind();

        protected override void Dispose(bool disposing)
        {
            try
            {
                this.Uniform.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Pass != null);
            Contract.Invariant(this._Uniform != null);
        }
    }
}
