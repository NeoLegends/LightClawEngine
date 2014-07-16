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
    /// <summary>
    /// Represents a controller for the execution of coroutines.
    /// </summary>
    public class CoroutineContext : ICoroutineContext, IUpdateable
    {
        /// <summary>
        /// Used for locking the event handlers.
        /// </summary>
        private readonly object eventLock = new object();

        /// <summary>
        /// The coroutine to execute.
        /// </summary>
        private readonly IEnumerable enumerable;

        /// <summary>
        /// The <see cref="IEnumerator"/> performing the steps.
        /// </summary>
        private IEnumerator enumerator;

        /// <summary>
        /// Occurs when the coroutine was stepped.
        /// </summary>
        public event EventHandler<SteppedEventArgs> Stepped;

        /// <summary>
        /// Backing field.
        /// </summary>
        private EventHandler<ParameterEventArgs> _Updating;

        /// <summary>
        /// Occurs before the coroutine is being updated.
        /// </summary>
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

        /// <summary>
        /// Backing field.
        /// </summary>
        private EventHandler<ParameterEventArgs> _Updated;

        /// <summary>
        /// Occurs after the coroutine is was updated.
        /// </summary>
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

        /// <summary>
        /// Indicates whether the <see cref="CoroutineContext"/> is allowed to execute.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="CoroutineContext"/> is finished.
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="CoroutineContext"/> with a <see cref="Func{T}"/>.
        /// </summary>
        /// <param name="coroutine">The coroutine to be executed.</param>
        public CoroutineContext(Func<IEnumerable> coroutine)
            : this(coroutine())
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);
        }

        /// <summary>
        /// Initializes a new <see cref="CoroutineContext"/> with a <see cref="Func{T}"/>.
        /// </summary>
        /// <param name="coroutine">The coroutine to be executed.</param>
        public CoroutineContext(IEnumerable coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            this.enumerable = coroutine;
            this.Reset();
        }

        /// <summary>
        /// Resets the <see cref="CoroutineContext"/>.
        /// </summary>
        public void Reset()
        {
            this.enumerator = this.enumerable.GetEnumerator();
            this.IsFinished = false;
        }

        /// <summary>
        /// Steps the coroutine once.
        /// </summary>
        /// <param name="current">The current object.</param>
        /// <returns><c>true</c> if the coroutine has finished execution, otherwise <c>false</c>.</returns>
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

        /// <summary>
        /// Updates the <see cref="CoroutineContext"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        void IUpdateable.Update(GameTime gameTime)
        {
            EventHandler<ParameterEventArgs> handler = this._Updating;
            if (handler != null)
            {
                handler(this, new ParameterEventArgs());
            }
            bool result = this.Step();
            handler = this._Updated;
            if (handler != null)
            {
                handler(this, new ParameterEventArgs(result));
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Stepped"/>-event.
        /// </summary>
        /// <param name="current">The current object.</param>
        /// <param name="result"><c>true</c> if the coroutine finished execution, otherwise <c>false</c>.</param>
        private void RaiseStepped(object current, bool result)
        {
            EventHandler<SteppedEventArgs> handler = this.Stepped;
            if (handler != null)
            {
                handler(this, new SteppedEventArgs(current, result));
            }
        }

        /// <summary>
        /// Contains Contract.Invariant-definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.enumerable != null);
        }
    }
}
