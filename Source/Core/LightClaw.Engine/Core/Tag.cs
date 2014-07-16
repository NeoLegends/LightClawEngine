using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Allows to tag something.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public sealed class TagAttribute : Attribute
    {
        /// <summary>
        /// The tag.
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="TagAttribute"/>.
        /// </summary>
        /// <param name="tag">The tag.</param>
        public TagAttribute(string tag)
        {
            Contract.Requires<ArgumentNullException>(tag != null);

            this.Tag = tag;
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Tag != null);
        }
    }
}
