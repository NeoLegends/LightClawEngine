using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public abstract class ModelEffect : Effect
    {
        private ModelPart _ModelPart;

        public ModelPart ModelPart
        {
            get
            {
                return _ModelPart;
            }
            protected internal set // Wow, I used protected internal. Rare occasion.
            {
                this.SetProperty(ref _ModelPart, value);
            }
        }

        protected ModelEffect() : this(false) { }

        protected ModelEffect(bool ownsPasses) : base(ownsPasses) { }

        protected ModelEffect(ModelPart modelPart)
            : this(modelPart, false)
        {
            Contract.Requires<ArgumentNullException>(modelPart != null);
        }

        protected ModelEffect(ModelPart modelPart, bool ownsPasses)
            : this(ownsPasses)
        {
            Contract.Requires<ArgumentNullException>(modelPart != null);

            this.ModelPart = modelPart;
        }

        protected ModelEffect(IEnumerable<EffectPass> passes)
            : this(passes, false)
        {
            Contract.Requires<ArgumentNullException>(passes != null);
        }

        protected ModelEffect(IEnumerable<EffectPass> passes, bool ownsPasses)
            : base(passes, ownsPasses)
        {
            Contract.Requires<ArgumentNullException>(passes != null);
        }

        protected ModelEffect(ModelPart modelPart, IEnumerable<EffectPass> passes)
            : this(modelPart, passes, false)
        {
            Contract.Requires<ArgumentNullException>(modelPart != null);
            Contract.Requires<ArgumentNullException>(passes != null);

            this.ModelPart = modelPart;
        }

        protected ModelEffect(ModelPart modelPart, IEnumerable<EffectPass> passes, bool ownsPasses)
            : base(passes, ownsPasses)
        {
            Contract.Requires<ArgumentNullException>(modelPart != null);
            Contract.Requires<ArgumentNullException>(passes != null);

            this.ModelPart = modelPart;
        }
    }
}
