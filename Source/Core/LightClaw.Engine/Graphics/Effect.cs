using System;
using System.Collections.Generic;
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

        private ObservableCollection<EffectPass> _Passes = new ObservableCollection<EffectPass>();

        public ObservableCollection<EffectPass> Passes
        {
            get
            {
                Contract.Ensures(Contract.Result<ObservableCollection<EffectPass>>() != null);

                return _Passes;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Passes, value);
            }
        }

        protected Effect() { }

        protected Effect(IEnumerable<EffectPass> techniques)
        {
            Contract.Requires<ArgumentNullException>(techniques != null);

            this.Passes.AddRange(techniques);
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

        protected abstract void OnUpdate(GameTime gameTime);

        protected abstract void OnLateUpdate();

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Passes != null);
        }
    }
}
