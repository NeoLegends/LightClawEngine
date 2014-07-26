using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public sealed class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> inner;

        public ReverseComparer() : this(Comparer<T>.Default) { }

        public ReverseComparer(IComparer<T> inner)
        {
            this.inner = inner ?? Comparer<T>.Default;
        }

        public int Compare(T left, T right) 
        { 
            return inner.Compare(right, left);
        }
    }
}
