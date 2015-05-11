using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.Threading
{
    /// <summary>
    /// Contains methods helping with deciding whether the calling thread is valid for the actions to execute.
    /// </summary>
    public static class ThreadF
    {
        /// <summary>
        /// Checks whether the current thread is the specified thread.
        /// </summary>
        /// <param name="targetThread">The thread that should be the current one.</param>
        /// <returns><c>true</c> if the current thread is the specified thread, otherwise <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCurrentThread(Thread targetThread)
        {
            Contract.Requires<ArgumentNullException>(targetThread != null);

            return IsCurrentThread(targetThread.ManagedThreadId);
        }

        /// <summary>
        /// Checks whether the current thread is the specified thread.
        /// </summary>
        /// <param name="targetThreadId">The ID of the thread that should be the current one.</param>
        /// <returns><c>true</c> if the current thread is the specified thread, otherwise <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCurrentThread(int targetThreadId)
        {
            return (Thread.CurrentThread.ManagedThreadId == targetThreadId);
        }

        /// <summary>
        /// Checks whether the current thread is the main thread.
        /// </summary>
        /// <returns><c>true</c> if the current thread is the main thread, otherwise <c>false</c>.</returns>
        /// <seealso cref="LightClawEngine.MainThreadId"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMainThread()
        {
            return IsCurrentThread(LightClawEngine.MainThreadId);
        }

        /// <summary>
        /// Checks whether the current thread is the specified thread and throws a <see cref="WrongThreadException"/> if
        /// it is not.
        /// </summary>
        /// <param name="targetThread">The thread that should be the current one.</param>
        /// <seealso cref="WrongThreadException"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotCurrentThread(Thread targetThread)
        {
            Contract.Requires<ArgumentNullException>(targetThread != null);

            ThrowIfNotCurrentThread(targetThread.ManagedThreadId);
        }

        /// <summary>
        /// Checks whether the current thread is the specified thread and throws a <see cref="WrongThreadException"/> if
        /// it is not.
        /// </summary>
        /// <param name="targetThreadId">The ID of the thread that should be the current one.</param>
        /// <seealso cref="WrongThreadException"/>
        public static void ThrowIfNotCurrentThread(int targetThreadId)
        {
            if (!IsCurrentThread(targetThreadId))
            {
                throw new WrongThreadException(
                    "The current ({0}) thread is not the specified ({1}) thread.".FormatWith(Thread.CurrentThread.ManagedThreadId, targetThreadId),
                    Thread.CurrentThread.ManagedThreadId,
                    targetThreadId
                );
            }
        }

        /// <summary>
        /// Checks whether the current thread is the main thread and throws a <see cref="WrongThreadException"/> if it
        /// is not.
        /// </summary>
        /// <seealso cref="LightClawEngine.MainThreadId"/>
        /// <seealso cref="WrongThreadException"/>
        public static void ThrowIfNotMainThread()
        {
            if (!IsMainThread())
            {
                throw new WrongThreadException(
                    "The current ({0}) thread is not the main thread.".FormatWith(Thread.CurrentThread.ManagedThreadId),
                    Thread.CurrentThread.ManagedThreadId,
                    LightClawEngine.MainThreadId
                );
            }
        }
    }
}
