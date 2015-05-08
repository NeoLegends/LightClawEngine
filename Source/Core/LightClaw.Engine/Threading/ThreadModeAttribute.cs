using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Threading
{
    /// <summary>
    /// Indicates a classes or a value types <see cref="ThreadMode"/>.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, 
        AllowMultiple = false, Inherited = false)]
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
        public ThreadModeAttribute(bool isThreadSafe) : this(isThreadSafe ? ThreadMode.Safe : ThreadMode.Unsafe) { }

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
        /// The method not thread-safe. Thus, the caller has to make sure the instance will only be
        /// accessed by one thread at a time.
        /// </summary>
        Unsafe = 0,

        /// <summary>
        /// The method is thread safe and can be used from multiple threads at the same time.
        /// </summary>
        Safe = 1,

        /// <summary>
        /// The method is thread-affine, it detects which threads are currently executing and protects itself from problems.
        /// </summary>
        Affine = 2,

        /// <summary>
        /// The thread mode was not explicitly specified. Non-safety will be assumed.
        /// </summary>
        Unspecified = Unsafe
    }

    /// <summary>
    /// Contains extension methods to <see cref="MethodInfo"/>.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets a methods <see cref="ThreadMode"/>. See remarks.
        /// </summary>
        /// <remarks>
        /// If there was no direct specification on the method itself, the declaring type of the method will be searched.
        /// Nested types do not inherit the <see cref="ThreadMode"/> from their declaring types. You will need to re-declare
        /// their mode.
        /// </remarks>
        /// <param name="mInfo">The method to get the <see cref="ThreadMode"/> of.</param>
        /// <returns>The methods <see cref="ThreadMode"/> or <see cref="ThreadMode.Unspecified"/> if the mode was not explicitly set.</returns>
        public static ThreadMode GetThreadMode(this MemberInfo mInfo)
        {
            Contract.Requires<ArgumentNullException>(mInfo != null);

            ThreadModeAttribute attr = mInfo.GetCustomAttribute<ThreadModeAttribute>();
            if (attr != null)
            {
                return attr.Mode;
            }
            else
            {
                Type declaringType = mInfo.DeclaringType;
                if (declaringType != null)
                {
                    return declaringType.GetThreadMode();
                }
            }

            return ThreadMode.Unspecified;
        }
    }
}
