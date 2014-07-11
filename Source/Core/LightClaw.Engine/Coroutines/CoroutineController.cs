using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using ProtoBuf;

namespace LightClaw.Engine.Coroutines
{
    [ProtoContract]
    [Solitary(typeof(CoroutineController))]
    public class CoroutineController : Component, ICoroutineController
    {
        [ProtoIgnore]
        private List<ICoroutineContext> contexts = new List<ICoroutineContext>();

        public CoroutineController() { }

        public CoroutineController(IEnumerable coroutine)
            : this(coroutine.Yield())
        {
            Contract.Requires<ArgumentNullException>(coroutine != null);
        }

        public CoroutineController(IEnumerable<IEnumerable> coroutines)
        {
            Contract.Requires<ArgumentNullException>(coroutines != null);

            foreach (IEnumerable coroutine in coroutines)
            {
                this.Add(coroutine);
            }
        }

        public void Add(IEnumerable coroutine)
        {
            lock (this.contexts)
            {
                this.contexts.Add(new CoroutineContext(coroutine));
            }
        }

        public void Add(Func<IEnumerable> coroutine)
        {
            this.Add(coroutine());
        }

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void OnLoad()
        {
        }

        protected override void OnUpdate()
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
                    context.Step();
                }
                lock (this.contexts)
                {
                    this.contexts.RemoveAll(context => context.IsFinished);
                }
            }
        }
    }
}
