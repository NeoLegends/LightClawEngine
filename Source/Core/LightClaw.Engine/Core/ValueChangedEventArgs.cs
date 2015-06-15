using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Event arguments for a change in a specified value.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of value that changed.</typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The variable's new value.
        /// </summary>
        public T NewValue { get; private set; }

        /// <summary>
        /// The variable's old value.
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ValueChangedEventArgs{T}"/> with just the new value.
        /// </summary>
        /// <param name="newValue">The variable's new value.</param>
        public ValueChangedEventArgs(T newValue) : this(newValue, default(T)) { }

        /// <summary>
        /// Initializes a new <see cref="ValueChangedEventArgs{T}"/> setting the new and the old value.
        /// </summary>
        /// <param name="newValue">The variable's new value.</param>
        /// <param name="oldValue">The variable's old value.</param>
        public ValueChangedEventArgs(T newValue, T oldValue)
        {
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }
    }
}
