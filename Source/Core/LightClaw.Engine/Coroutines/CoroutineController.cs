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
    [DataContract]
    [Solitary(typeof(CoroutineController), "More than one CoroutineController induces unnecessary overhead.")]
    public class CoroutineController : Component
    {
        [IgnoreDataMember]
        private List<ICoroutineContext> contexts = new List<ICoroutineContext>();

        public CoroutineController() { }

        public CoroutineController(Func<IEnumerable> coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            this.Add(coroutine);
        }

        public CoroutineController(IEnumerable coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            this.Add(coroutine);
        }

        public void Add(IEnumerable coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            lock (this.contexts)
            {
                this.contexts.Add(new CoroutineContext(coroutine));
            }
        }

        public void Add(Func<IEnumerable> coroutine)
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);

            lock (this.contexts)
            {
                this.contexts.Add(new CoroutineContext(coroutine));
            }
        }

        public void AddRange(IEnumerable<IEnumerable> coroutines)
        {
            Contract.Requires<ArgumentNullException>(coroutines != null);

            foreach (IEnumerable coroutine in coroutines)
            {
                this.Add(coroutine);
            }
        }

        public void AddRange(IEnumerable<Func<IEnumerable>> coroutines)
        {
            Contract.Requires<ArgumentNullException>(coroutines != null);

            foreach (Func<IEnumerable> coroutine in coroutines)
            {
                this.Add(coroutine);
            }
        }

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
