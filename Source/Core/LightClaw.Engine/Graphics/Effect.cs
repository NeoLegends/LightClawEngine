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
    [ContentReader(typeof(EffectReader))]
    public class Effect : DisposableEntity, IReadOnlyList<EffectPass>
    {
        public ImmutableDictionary<string, int> Attributes
        {
            get
            {
                return this.Passes.First().Attributes;
            }
        }

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

        public Effect() : this(false) { }

        public Effect(bool ownsPasses)
        {
            this.OwnsPasses = ownsPasses;
        }

        public Effect(params EffectPass[] passes)
            : this(passes, true)
        {
            Contract.Requires<ArgumentNullException>(passes != null);
            Contract.Requires<ArgumentException>(passes.Any());
            Contract.Requires<ArgumentException>(AttributeSignatureMatches(passes));
        }

        public Effect(IEnumerable<EffectPass> passes, bool ownsPasses)
            : this(ownsPasses)
        {
            Contract.Requires<ArgumentNullException>(passes != null);
            Contract.Requires<ArgumentException>(passes.Any());
            Contract.Requires<ArgumentException>(AttributeSignatureMatches(passes));

            this.Passes = passes.ToImmutableArray();
        }

        public Binding ApplyPass(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < this.Passes.Length);

            return this.Passes[index].Bind();
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

        [Pure]
        public static bool AttributeSignatureMatches(params EffectPass[] passes)
        {
            Contract.Requires<ArgumentNullException>(passes != null);

            return AttributeSignatureMatches((IEnumerable<EffectPass>)passes);
        }

        [Pure]
        public static bool AttributeSignatureMatches(IEnumerable<EffectPass> passes)
        {
            Contract.Requires<ArgumentNullException>(passes != null);

            if (!passes.Any())
            {
                return true;
            }
            
            bool result = true;
            ImmutableDictionary<string, int> current = passes.First().Attributes;
            foreach (EffectPass p in passes)
            {
                ImmutableDictionary<string, int> prev = current;
                result &= prev.SequenceEqual(current = p.Attributes);
            }
            return result;
        }
    }
}
