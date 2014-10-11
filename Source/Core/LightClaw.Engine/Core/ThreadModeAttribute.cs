using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Indicates a class' or a value types thread mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ThreadModeAttribute : Attribute
    {
        /// <summary>
        /// The instance's thread mode.
        /// </summary>
        public ThreadMode Mode { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ThreadModeAttribute"/> and sets whether the instance is thread-safe or not.
        /// </summary>
        /// <param name="isThreadSafe">Indicates whether instances of the type are thread-safe.</param>
        public ThreadModeAttribute(bool isThreadSafe) : this(isThreadSafe ? ThreadMode.Safe : ThreadMode.None) { }

        /// <summary>
        /// Initializes a new <see cref="ThreadModeAttribute"/> and sets the <see cref="ThreadMode"/>.
        /// </summary>
        /// <param name="mode">The type's thread mode.</param>
        public ThreadModeAttribute(ThreadMode mode)
        {
            this.Mode = mode;
        }
    }

    /// <summary>
    /// Indicates how an instance can be used in respect to multiple threads.
    /// </summary>
    public enum ThreadMode
    {
        /// <summary>
        /// The instance is neither thread-safe nor affine. Thus, the caller has to make sure the instance will only be
        /// accessed by one thread at a time.
        /// </summary>
        None = 0,

        /// <summary>
        /// The instance is thread safe and can be used from multiple threads at the same time.
        /// </summary>
        Safe,

        /// <summary>
        /// The instance is thread-affine, it detects which threads are currently executing and protects itself from
        /// problems.
        /// </summary>
        Affine
    }
}
