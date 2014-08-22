using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.Graphics
{
    public abstract class Effect : GLObject, IEnumerable<EffectPass>, IUpdateable, ILateUpdateable
    {
        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        private ImmutableList<EffectPass> _Passes = ImmutableList<EffectPass>.Empty;

        public ImmutableList<EffectPass> Passes
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableList<EffectPass>>() != null);

                return _Passes;
            }
            protected set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.All(pass => pass != null));

                this.SetProperty(ref _Passes, value);
            }
        }

        protected Effect() { }

        protected Effect(IEnumerable<EffectPass> techniques)
        {
            Contract.Requires<ArgumentNullException>(techniques != null);

            this.Passes = techniques.ToImmutableList();
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
            return this.Passes.GetEnumerator();
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

            ImmutableList<EffectPass> passes = this.Passes;
            if (passes.Count > index)
            {
                passes[index].Bind();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected abstract void OnUpdate(GameTime gameTime);

        protected abstract void OnLateUpdate();

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Passes != null);
        }
    }
}
