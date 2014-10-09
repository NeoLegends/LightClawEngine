using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Event arguments containing a parseable parameter.
    /// </summary>
    public class ParameterEventArgs : EventArgs
    {
        /// <summary>
        /// Backing field.
        /// </summary>
        private static readonly ParameterEventArgs _Default = new ParameterEventArgs();

        /// <summary>
        /// Default <see cref="ParameterEventArgs"/> with the <see cref="P:Parameter"/> set to <c>null</c>.
        /// </summary>
        public static ParameterEventArgs Default
        {
            get
            {
                return _Default;
            }
        }

        /// <summary>
        /// The parameter. May be null.
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ParameterEventArgs"/>.
        /// </summary>
        public ParameterEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ParameterEventArgs"/> and sets the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public ParameterEventArgs(object parameter)
        {
            this.Parameter = parameter;
        }
    }
}
