using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Threading
{
    public class LightClawSynchronizationContext : SynchronizationContext
    {
        private readonly Dispatcher dispatcher;

        private readonly DispatcherPriority priority;

        public LightClawSynchronizationContext() : this(Dispatcher.Current) { }

        public LightClawSynchronizationContext(Dispatcher dispatcher)
            : this(dispatcher, DispatcherPriority.Normal)
        {
            Contract.Requires<ArgumentNullException>(dispatcher != null);
        }
        
        public LightClawSynchronizationContext(Dispatcher dispatcher, DispatcherPriority priority)
        {
            Contract.Requires<ArgumentNullException>(dispatcher != null);

            this.dispatcher = dispatcher;
            this.priority = priority;
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            this.dispatcher.InvokeSlim(new Action<object>(d), state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            this.dispatcher.Invoke(new Action<object>(d), DispatcherPriority.Immediate).Wait();
        }
    }
}
