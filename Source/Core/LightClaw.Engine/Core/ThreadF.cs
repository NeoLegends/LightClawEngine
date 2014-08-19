using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Extensions;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Contains methods helping with deciding whether the calling thread is valid for the actions to execute.
    /// </summary>
    public static class ThreadF
    {
        /// <summary>
        /// Checks whether the current thread is the main thread and throws a <see cref="WrongThreadException"/> if it is not.
        /// </summary>
        /// <seealso cref="LightClawEngine.MainThreadId"/>
        /// <seealso cref="WrongThreadException"/>
        public static void AssertMainThread()
        {
            if (!TryAssertMainThread())
            {
                throw new WrongThreadException(
                    "The current ({0}) thread is not the main thread.".FormatWith(Thread.CurrentThread.ManagedThreadId),
                    Thread.CurrentThread.ManagedThreadId,
                    LightClawEngine.MainThreadId
                );
            }
        }

        /// <summary>
        /// Checks whether the current thread is the specified thread and throws a <see cref="WrongThreadException"/> if it is not.
        /// </summary>
        /// <seealso cref="WrongThreadException"/>
        public static void AssertThread(int targetThreadId)
        {
            if (!TryAssertThread(targetThreadId))
            {
                throw new WrongThreadException(
                    "The current ({0}) thread is not the specified ({1}) thread.".FormatWith(Thread.CurrentThread.ManagedThreadId, targetThreadId),
                    Thread.CurrentThread.ManagedThreadId,
                    targetThreadId
                );
            }
        }

        /// <summary>
        /// Checks whether the current thread is the main thread.
        /// </summary>
        /// <returns><c>true</c> if the current thread is the main thread, otherwise <c>false</c>.</returns>
        /// <seealso cref="LightClawEngine.MainThreadId"/>
        public static bool TryAssertMainThread()
        {
            return TryAssertThread(LightClawEngine.MainThreadId);
        }

        /// <summary>
        /// Checks whether the current thread is the specified thread.
        /// </summary>
        /// <returns><c>true</c> if the current thread is the specified thread, otherwise <c>false</c>.</returns>
        public static bool TryAssertThread(int targetThreadId)
        {
            return (Thread.CurrentThread.ManagedThreadId == targetThreadId);
        }
    }
}
