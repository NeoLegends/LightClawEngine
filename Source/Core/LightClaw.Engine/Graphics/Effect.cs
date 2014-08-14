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
    public class Effect : GLObject, IEnumerable<EffectPass>
    {
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

        public Effect() { }

        public Effect(IEnumerable<EffectPass> techniques)
            : this()
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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Passes != null);
        }
    }
}
