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
    /// using (Binding bufferBinding = new Binding(this.Buffer))
    /// {
    ///     // Code interacting with bound buffer
    /// }
    /// </code>
    /// </example>
    public struct Binding : IDisposable, IBindable
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
            : this(bindable, true)
        {
            Contract.Requires<ArgumentNullException>(bindable != null);
        }

        /// <summary>
        /// Initializes a new <see cref="Binding"/>.
        /// </summary>
        /// <param name="bindable">The <see cref="IBindable"/> to (un)bind.</param>
        /// <param name="bindImmediately">Indicates whether to bind the element upon creation of this struct.</param>
        public Binding(IBindable bindable, bool bindImmediately)
        {
            Contract.Requires<ArgumentNullException>(bindable != null);

            this.bindable = bindable;
            if (bindImmediately)
            {
                this.Bind();
            }
        }

        /// <summary>
        /// Binds the <see cref="IBindable"/>.
        /// </summary>
        public void Bind()
        {
            this.bindable.Bind();
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
        public void Unbind()
        {
#if (DEBUG || ALWAYS_UNBIND)
            this.bindable.Unbind();
#endif
        }
    }
}
