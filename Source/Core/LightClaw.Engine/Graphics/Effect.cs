using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using Newtonsoft.Json;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public class Effect : DisposableEntity, IReadOnlyList<EffectPass>
    {
        public int Count
        {
            get 
            {
                return this.Passes.Length;
            }
        }

        private bool _OwnsPasses;

        protected bool OwnsPasses
        {
            get
            {
                return _OwnsPasses;
            }
            set
            {
                this.SetProperty(ref _OwnsPasses, value);
            }
        }

        private ImmutableArray<EffectPass> _Passes = ImmutableArray<EffectPass>.Empty;

        [DataMember]
        public ImmutableArray<EffectPass> Passes
        {
            get
            {
                // Code contracts don't like ImmutableArrayExtensions.All
                Contract.Ensures(Enumerable.All(Contract.Result<ImmutableArray<EffectPass>>(), pass => pass != null));

                return _Passes;
            }
            protected set
            {
                // Code contracts don't like ImmutableArrayExtensions.All
                Contract.Requires<ArgumentException>(Enumerable.All(value, pass => pass != null));

                this.SetProperty(ref _Passes, value);
            }
        }

        public EffectPass this[int index]
        {
            get
            {
                return this.Passes[index];
            }
        }

        protected Effect() : this(false) { }

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

        public Binding ApplyPass(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < this.Passes.Length);

            return new Binding(this.Passes[index]);
        }

        public IEnumerator<EffectPass> GetEnumerator()
        {
            return ((IEnumerable<EffectPass>)this.Passes).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.OwnsPasses)
                {
                    foreach (EffectPass pass in this.Passes)
                    {
                        pass.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Passes != null);
            Contract.Invariant(Enumerable.All(this._Passes, ep => ep != null)); // Code Contracts complains about ImmutableArrayExtensions.All
        }
    }
}
