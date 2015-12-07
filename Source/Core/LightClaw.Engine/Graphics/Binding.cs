using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a mechanism for binding <see cref="IBindable"/>-instances for a specified time using the
    /// 'using'-clause for convenient syntax and ensured unbinding after usage.
    /// </summary>
    /// <remarks>
    /// If LightClaw runs in release mode, the mechanism does NOT unbind the instance for performance reasons.
    /// If you wish to override that behavior, set the ALWAYS_UNBIND compile time constant.
    /// </remarks>
    /// <example>
    /// <code>
    /// using (Binding bufferBinding = this.Buffer.Bind())
    /// {
    ///     // Code interacting with bound buffer
    /// }
    /// </code>
    /// </example>
    public struct Binding : IDisposable
    {
        /// <summary>
        /// The <see cref="IBindable"/> to (un)bind.
        /// </summary>
        private readonly IBindable bindable;

        /// <summary>
        /// Initializes a new <see cref="Binding"/>.
        /// </summary>
        /// <param name="bindable">The <see cref="IBindable"/> to (un)bind.</param>
        public Binding(IBindable bindable)
        {
            Contract.Requires<ArgumentNullException>(bindable != null);

            this.bindable = bindable;
        }

        /// <summary>
        /// Disposes the <see cref="Binding"/> unbinding the <see cref="IBindable"/>.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Unbind();
        }

        /// <summary>
        /// Unbinds the <see cref="IBindable"/>.
        /// </summary>
        [Conditional("DEBUG"), Conditional("ALWAYS_UNBIND")]
        public void Unbind()
        {
            if (this.bindable != null)
            {
                this.bindable.Unbind();
            }
        }
    }
}
