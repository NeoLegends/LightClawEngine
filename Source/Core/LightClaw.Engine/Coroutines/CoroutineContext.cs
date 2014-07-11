﻿using System;
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
    public class CoroutineContext : ICoroutineContext, IUpdateable
    {
        private readonly object eventLock = new object();

        private readonly IEnumerable enumerable;

        private IEnumerator enumerator;

        public event EventHandler<SteppedEventArgs> Stepped;

        private EventHandler<ParameterEventArgs> _Updating;

        event EventHandler<ParameterEventArgs> IUpdateable.Updating
        {
            add 
            {
                lock (eventLock)
                {
                    _Updating += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    _Updating -= value;
                }
            }
        }

        private EventHandler<ParameterEventArgs> _Updated;

        event EventHandler<ParameterEventArgs> IUpdateable.Updated
        {
            add
            {
                lock (eventLock)
                {
                    _Updated += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    _Updated -= value;
                }
            }
        }

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

        public bool Step(out object current)
        {
            if (this.IsEnabled && !this.IsFinished)
            {
                bool result = !(this.IsFinished = !this.enumerator.MoveNext());
                current = this.enumerator.Current;
                this.RaiseStepped(current, result);
                return result;
            }
            else
            {
                current = null;
                return false;
            }
        }

        void IUpdateable.Update()
        {
            EventHandler<ParameterEventArgs> handler = this._Updating;
            if (handler != null)
            {
                handler(this, new ParameterEventArgs());
            }
            this.Step();
            handler = this._Updated;
            if (handler != null)
            {
                handler(this, new ParameterEventArgs());
            }
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