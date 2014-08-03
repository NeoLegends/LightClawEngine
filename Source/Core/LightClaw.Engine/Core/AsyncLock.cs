using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents an asynchronous lock built around a <see cref="SemaphoreSlim"/> for use with the 'using'-dispose-pattern.
    /// </summary>
    /// <example>
    /// using (AsyncLockReleaser releaser = await this.asyncLock.LockAsync())
    /// {
    ///     // do locked stuff here
    /// }
    /// </example>
    /// <seealso cref="SemaphoreSlim"/>
    public class AsyncLock
    {
        /// <summary>
        /// The underlying <see cref="SemaphoreSlim"/> performing the locking.
        /// </summary>
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Initializes a new <see cref="AsyncLock"/> with an initial count of 1 (= only one thread has access at a time).
        /// </summary>
        public AsyncLock() : this(1) { }

        /// <summary>
        /// Initializes a new <see cref="AsyncLock"/> setting the amount of threads that can acquire the lock.
        /// </summary>
        /// <param name="initialCount">The amount of threads that can acquire the lock.</param>
        public AsyncLock(int initialCount)
        {
            Contract.Requires<ArgumentOutOfRangeException>(initialCount > 0);

            this.semaphore = new SemaphoreSlim(initialCount);
        }

        /// <summary>
        /// Asynchronously takes the lock.
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous waiting and lock-releasing operation.</returns>
        public Task<AsyncLockReleaser> LockAsync()
        {
            return this.semaphore.WaitAsync().ContinueWith(t => new AsyncLockReleaser(this.semaphore), TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// The struct used to release the previously acquired lock. Usabile via using-pattern.
        /// </summary>
        /// <remarks>
        /// Always make sure to release lock via <see cref="M:Release"/> or <see cref="M:Dispose"/>, otherwise
        /// the lock will not be released and deadlocks will occur.
        /// </remarks>
        public struct AsyncLockReleaser : IDisposable
        {
            /// <summary>
            /// The <see cref="SemaphoreSlim"/> holding the lock.
            /// </summary>
            private readonly SemaphoreSlim semaphore;
            
            /// <summary>
            /// Initializes a new <see cref="AsyncLockReleaser"/>.
            /// </summary>
            /// <param name="semaphore">The <see cref="SemaphoreSlim"/> holding the lock.</param>
            public AsyncLockReleaser(SemaphoreSlim semaphore)
            {
                Contract.Requires<ArgumentNullException>(semaphore != null);

                this.semaphore = semaphore;
            }

            /// <summary>
            /// Releases the lock.
            /// </summary>
            public void Release()
            {
                this.semaphore.Release();
            }

            /// <summary>
            /// Disposes the instance releasing the lock.
            /// </summary>
            void IDisposable.Dispose()
            {
                this.Release();
            }
        }
    }
}
