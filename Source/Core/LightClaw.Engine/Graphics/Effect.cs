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
    public class Effect : GLObject
    {
        private ObservableCollection<EffectPass> _Passes = new ObservableCollection<EffectPass>();

        public ObservableCollection<EffectPass> Passes
        {
            get
            {
                return _Passes;
            }
            private set
            {
                this.SetProperty(ref _Passes, value);
            }
        }

        public Effect()
        {
            this.Passes.CollectionChanged += (s, e) =>
            {
            };
        }

        public Effect(IEnumerable<EffectPass> techniques)
            : this()
        {
            Contract.Requires<ArgumentNullException>(techniques != null);

            this.Passes.AddRange(techniques);
        }
    }
}
