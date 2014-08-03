using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.Coroutines
{
    /// <summary>
    /// Represents a lightweight <see cref="Component"/> that controls execution of coroutines.
    /// </summary>
    [DataContract]
    [Solitary(typeof(CoroutineController), "More than one CoroutineController induces unnecessary overhead.")]
    public class CoroutineController : Component
    {
        /// <summary>
        /// The list of managed coroutines.
        /// </summary>
        [IgnoreDataMember]
        private List<ICoroutineContext> contexts = new List<ICoroutineContext>();

        /// <summary>
        /// Initializes a new <see cref="CoroutineController"/>.
        /// </summary>
        public CoroutineController() { }

        /// <summary>
        /// Initializes a new <see cref="CoroutineController"/> from a <see cref="Func{T}"/>.
        /// </summary>
        /// <param name="coroutine">The coroutine function.</param>
        public CoroutineController(Func<IEnumerable> coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            this.Add(coroutine);
        }

        /// <summary>
        /// Initializes a new <see cref="CoroutineController"/> from a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to be executed.</param>
        public CoroutineController(IEnumerable coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            this.Add(coroutine);
        }

        /// <summary>
        /// Queries a new coroutine for execution.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        public void Add(IEnumerable coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            lock (this.contexts)
            {
                this.contexts.Add(new CoroutineContext(coroutine));
            }
        }

        /// <summary>
        /// Queries a new coroutine for execution.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        public void Add(Func<IEnumerable> coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            lock (this.contexts)
            {
                this.contexts.Add(new CoroutineContext(coroutine));
            }
        }

        /// <summary>
        /// Queries coroutines for execution.
        /// </summary>
        /// <param name="coroutines">The coroutines to execute.</param>
        public void AddRange(IEnumerable<IEnumerable> coroutines)
        {
            Contract.Requires<ArgumentNullException>(coroutines != null);

            foreach (IEnumerable coroutine in coroutines)
            {
                this.Add(coroutine);
            }
        }

        /// <summary>
        /// Queries coroutines for execution.
        /// </summary>
        /// <param name="coroutines">The coroutines to execute.</param>
        public void AddRange(IEnumerable<Func<IEnumerable>> coroutines)
        {
            Contract.Requires<ArgumentNullException>(coroutines != null);

            foreach (Func<IEnumerable> coroutine in coroutines)
            {
                this.Add(coroutine);
            }
        }

        /// <summary>
        /// Updates all coroutines and removes the ones that have finished execution.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        protected override void OnUpdate(GameTime gameTime)
        {
            IEnumerable<ICoroutineContext> contexts = this.contexts;
            if (contexts != null)
            {
                lock (contexts)
                {
#pragma warning disable 0728
                    contexts = contexts.ToArray();
#pragma warning restore 0728
                }

                foreach (ICoroutineContext context in contexts)
                {
                    if (context.Step())
                    {
                        lock (this.contexts)
                        {
                            this.contexts.Remove(context);
                        }
                    }
                }
            }
        }
    }
}
