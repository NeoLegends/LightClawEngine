using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public abstract class Effect : DisposableEntity, IEnumerable<EffectPass>, IUpdateable, ILateUpdateable
    {
        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        protected bool OwnsPasses { get; private set; }

        private ImmutableArray<EffectPass> _Passes = ImmutableArray<EffectPass>.Empty;

        public ImmutableArray<EffectPass> Passes
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableArray<EffectPass>>() != null);
                Contract.Ensures(Contract.Result<ImmutableArray<EffectPass>>().All(pass => pass != null));

                return _Passes;
            }
            protected set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.All(pass => pass != null));

                this.SetProperty(ref _Passes, value);
            }
        }

        protected Effect()
            : this(false)
        {
        }

        protected Effect(bool ownsPasses)
        {
            this.OwnsPasses = ownsPasses;
        }

        protected Effect(IEnumerable<EffectPass> passes)
            : this(passes, false)
        {
            Contract.Requires<ArgumentNullException>(passes != null);

            this.Passes = passes.ToImmutableArray();
        }

        protected Effect(IEnumerable<EffectPass> passes, bool ownsPasses)
            : this(ownsPasses)
        {
            Contract.Requires<ArgumentNullException>(passes != null);

            this.Passes = passes.ToImmutableArray();
        }

        public void Apply(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            if (!this.TryApply(index))
            {
                throw new InvalidOperationException("The pass at index {0} could not be applied.".FormatWith(index));
            }
        }

        public IEnumerator<EffectPass> GetEnumerator()
        {
            return ((IEnumerable<EffectPass>)this.Passes).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Update(GameTime gameTime)
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
            {
                this.OnUpdate(gameTime);
            }
        }

        public void LateUpdate()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
            {
                this.OnLateUpdate();
            }
        }

        public bool TryApply(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            ImmutableArray<EffectPass> passes = this.Passes;
            if (passes.Length > index)
            {
                passes[index].Bind();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.OwnsPasses)
            {
                foreach (EffectPass pass in this.Passes)
                {
                    pass.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        protected abstract void OnUpdate(GameTime gameTime);

        protected abstract void OnLateUpdate();

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Passes != null);
            Contract.Invariant(this._Passes.All(pass => pass != null));
        }
    }
}
