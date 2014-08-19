using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// A struct raising <see cref="ParameterEventArgs"/>-events. Made for use with the 'using'-clause.
    /// </summary>
    /// <example>
    /// <code>
    /// using (ParameterEventRaiser raiser = new ParameterEventRaiser(this, this.Updating, this.Updated))
    /// {
    ///     // Code surrounded by events. "Updating" will be raised on entry of the block, "Updated" right after.
    /// }
    /// </code>
    /// </example>
    public struct ParameterEventArgsRaiser : IDisposable
    {
        /// <summary>
        /// The handler raised on exit of the block.
        /// </summary>
        private readonly EventHandler<ParameterEventArgs> onAfterHandler;

        /// <summary>
        /// <see cref="EventArgs"/> for the event raised on exit.
        /// </summary>
        private readonly ParameterEventArgs onAfterEventArgs;

        /// <summary>
        /// The handler raised on entry of the block.
        /// </summary>
        private readonly EventHandler<ParameterEventArgs> onBeforeHandler;

        /// <summary>
        /// <see cref="EventArgs"/> for the event raised on entry.
        /// </summary>
        private readonly ParameterEventArgs onBeforeEventArgs;

        /// <summary>
        /// The object that triggered the event.
        /// </summary>
        private readonly object sender;

        /// <summary>
        /// Initializes a new <see cref="ParameterEventArgsRaiser"/>.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="onBeforeHandler">The handler raised on entry of the block.</param>
        /// <param name="onAfterHandler">The handler raised on exit of the block.</param>
        /// <param name="onBeforeParameter">The parameter for the for the event raised on entry.</param>
        /// <param name="onAfterParameter">The parameter for the for the event raised on exit.</param>
        /// <param name="raiseImmediately">
        /// Indicates whether to raise the <paramref name="onBeforeHandler"/> immediately on construction
        /// of the <see cref="ParameterEventArgsRaiser"/> or only on <see cref="M:RaiseOnBefore"/>.
        /// </param>
        public ParameterEventArgsRaiser(
                object sender,
                EventHandler<ParameterEventArgs> onBeforeHandler,
                EventHandler<ParameterEventArgs> onAfterHandler,
                object onBeforeParameter = null,
                object onAfterParameter = null,
                bool raiseImmediately = true
            )
        {
            this.onAfterHandler = onAfterHandler;
            this.onAfterEventArgs = (onAfterParameter != null) ? new ParameterEventArgs(onAfterParameter) : ParameterEventArgs.Default;
            this.onBeforeHandler = onBeforeHandler;
            this.onBeforeEventArgs = (onBeforeParameter != null) ? new ParameterEventArgs(onBeforeParameter) : ParameterEventArgs.Default;
            this.sender = sender;

            if (raiseImmediately)
            {
                this.RaiseOnBefore();
            }
        }

        /// <summary>
        /// Raises the event for the exit of the block.
        /// </summary>
        public void RaiseOnAfter()
        {
            EventHandler<ParameterEventArgs> handler = this.onAfterHandler;
            if (handler != null)
            {
                handler(this.sender, this.onAfterEventArgs ?? ParameterEventArgs.Default);
            }
        }

        /// <summary>
        /// Raises the event on entry of the block.
        /// </summary>
        public void RaiseOnBefore()
        {
            EventHandler<ParameterEventArgs> handler = this.onBeforeHandler;
            if (handler != null)
            {
                handler(this.sender, this.onBeforeEventArgs ?? ParameterEventArgs.Default);
            }
        }

        /// <summary>
        /// Disposes the instance raising the event on exit.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.RaiseOnAfter();
        }
    }
}
