using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Coroutines
{
    public class CoroutineContext : ICoroutineContext
    {
        private readonly IEnumerable enumerable;

        private IEnumerator enumerator;

        public event EventHandler<SteppedEventArgs> Stepped;

        public bool IsEnabled { get; set; }

        public bool IsFinished { get; private set; }

        public CoroutineContext(Func<IEnumerable> coroutine)
            : this(coroutine())
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);
        }

        public CoroutineContext(IEnumerable coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            this.enumerable = coroutine;
        }

        public void Reset()
        {
            this.enumerator = this.enumerable.GetEnumerator();
            this.IsFinished = false;
        }

        public bool Step()
        {
            object current;
            return this.Step(out current);
        }

        public bool Step(out object current)
        {
            if (this.IsEnabled && !this.IsFinished)
            {
                bool result = !(this.IsFinished = !this.enumerator.MoveNext());
                current = this.enumerator.Current;
                return result;
            }
            else
            {
                current = null;
                return false;
            }
        }

        public void Step(TimeSpan timeOut)
        {
            Stopwatch st = Stopwatch.StartNew();
            while (st.Elapsed < timeOut && this.Step()) { }
        }

        void IUpdateable.Update()
        {
            this.Step();
        }

        private void RaiseStepped(object current, bool result)
        {
            EventHandler<SteppedEventArgs> handler = this.Stepped;
            if (handler != null)
            {
                handler(this, new SteppedEventArgs(current, result));
            }
        }
    }
}
