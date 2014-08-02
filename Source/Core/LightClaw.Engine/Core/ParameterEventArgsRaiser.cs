using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public struct ParameterEventArgsRaiser : IDisposable
    {
        private readonly EventHandler<ParameterEventArgs> onAfterHandler;

        private readonly ParameterEventArgs onAfterEventArgs;

        private readonly EventHandler<ParameterEventArgs> onBeforeHandler;

        private readonly ParameterEventArgs onBeforeEventArgs;

        private readonly object sender;

        public ParameterEventArgsRaiser(
                object sender, 
                EventHandler<ParameterEventArgs> onBeforeHandler, 
                EventHandler<ParameterEventArgs> onAfterHandler,
                ParameterEventArgs onBeforeEventArgs = null,
                ParameterEventArgs onAfterEventArgs = null,
                bool raiseImmediately = true
            )
        {
            this.onAfterHandler = onAfterHandler;
            this.onAfterEventArgs = onAfterEventArgs;
            this.onBeforeHandler = onBeforeHandler;
            this.onBeforeEventArgs = onBeforeEventArgs;
            this.sender = sender;

            if (raiseImmediately)
            {
                this.RaiseOnBefore();
            }
        }

        public void RaiseOnAfter()
        {
            EventHandler<ParameterEventArgs> handler = this.onAfterHandler;
            if (handler != null)
            {
                handler(this.sender, this.onAfterEventArgs ?? ParameterEventArgs.Default);
            }
        }

        public void RaiseOnBefore()
        {
            EventHandler<ParameterEventArgs> handler = this.onBeforeHandler;
            if (handler != null)
            {
                handler(this.sender, this.onBeforeEventArgs ?? ParameterEventArgs.Default);
            }
        }

        void IDisposable.Dispose()
        {
            this.RaiseOnAfter();
        }
    }
}
