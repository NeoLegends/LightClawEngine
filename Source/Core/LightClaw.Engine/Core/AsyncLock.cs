using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Threading;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents an asynchronous lightweight lock built around a <see cref="SemaphoreSlim"/> for use with the 'using'-dispose-pattern.
    /// </summary>
    /// <remarks>
    /// Always make sure to release lock via <see cref="M:AsyncLockReleaser.Release"/> or
    /// <see cref="M:AsyncLockReleaser.Dispose"/>, otherwise the lock will not be released and deadlocks will occur.
    /// </remarks>
    /// <example>
    /// <code>
    /// using (await this.asyncLock.LockAsync())
    /// {
    ///     // do locked stuff here
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="SemaphoreSlim"/>
    public class AsyncLock : IDisposable
    {
        /// <summary>
        /// The underlying <see cref="SemaphoreSlim"/> performing the locking.
        /// </summary>
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Initializes a new <see cref="AsyncLock"/> with an initial count of 1. See remarks.
        /// </summary>
        /// <remarks>
        /// If the initial count is set to one, the <see cref="AsyncLock"/> behaves just like a regular lock,
        /// just that it allows using it together with async / await.
        /// </remarks>
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
        /// Finalizes the <see cref="AsyncLock"/>.
        /// </summary>
        ~AsyncLock()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Disposes the <see cref="AsyncLock"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Synchronously takes the lock.
        /// </summary>
        /// <remarks>
        /// This method <u>blocks</u> the calling thread until the lock can be taken. Use <see cref="Festify.AsyncLock.LockAsync()"/> if UI
        /// responsiveness is of the essence.
        /// </remarks>
        /// <returns>A <see cref="AsyncLockReleaser"/> used to release the lock.</returns>
        public AsyncLockReleaser Lock()
        {
            return this.Lock(Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Synchronously tries to take the lock during the specified time and indicates whether the lock has been taken.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method <u>blocks</u> the calling thread until the lock can be taken. Use <see cref="Festify.AsyncLock.LockAsync()"/> if UI
        /// responsiveness is of the essence.
        /// </p>
        /// <p>
        /// A property on <see cref="AsyncLockReleaser"/> indicates whether the lock has been taken.
        /// </p>
        /// </remarks>
        /// <param name="millisecondsTimeOut">The time which to wait until the lock is taken.</param>
        /// <returns>A <see cref="AsyncLockReleaser"/> used to release the lock.</returns>
        public AsyncLockReleaser Lock(int millisecondsTimeOut)
        {
            return this.Lock(TimeSpan.FromMilliseconds(millisecondsTimeOut));
        }

        /// <summary>
        /// Synchronously tries to take the lock during the specified time and indicates whether the lock has been taken.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method <u>blocks</u> the calling thread until the lock can be taken. Use <see cref="Festify.AsyncLock.LockAsync()"/> if UI
        /// responsiveness is of the essence.
        /// </p>
        /// <p>
        /// A property on <see cref="AsyncLockReleaser"/> indicates whether the lock has been taken.
        /// </p>
        /// </remarks>
        /// <param name="timeOut">The time which to wait until the lock is taken.</param>
        /// <returns>A <see cref="AsyncLockReleaser"/> used to release the lock.</returns>
        public AsyncLockReleaser Lock(TimeSpan timeOut)
        {
            return new AsyncLockReleaser(this.semaphore, this.semaphore.Wait(timeOut));
        }

        /// <summary>
        /// Synchronously tries to take the lock during the specified time and indicates whether the lock has been taken.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method <u>blocks</u> the calling thread until the lock can be taken. Use <see cref="Festify.AsyncLock.LockAsync()"/> if UI
        /// responsiveness is of the essence.
        /// </p>
        /// <p>
        /// A property on <see cref="AsyncLockReleaser"/> indicates whether the lock has been taken.
        /// </p>
        /// </remarks>
        /// <param name="token">The <see cref="CancellationToken"/> to observe while waiting.</param>
        /// <returns>A <see cref="AsyncLockReleaser"/> used to release the lock.</returns>
        public AsyncLockReleaser Lock(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            this.semaphore.Wait(token);
            return new AsyncLockReleaser(this.semaphore);
        }

        /// <summary>
        /// Asynchronously takes the lock.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous waiting operation. Its return value is used to free
        /// the lock.
        /// </returns>
        public Task<AsyncLockReleaser> LockAsync()
        {
            return this.LockAsync(Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Asynchronously tries to take the lock during the specified time and indicates whether the lock has been taken.
        /// </summary>
        /// <remarks>
        /// A property on <see cref="AsyncLockReleaser"/> indicates whether the lock has been taken.
        /// </remarks>
        /// <param name="millisecondsTimeOut">The time which to wait until the lock is taken.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous waiting operation. Its return value is used to free
        /// the lock.
        /// </returns>
        public Task<AsyncLockReleaser> LockAsync(int millisecondsTimeOut)
        {
            return this.LockAsync(TimeSpan.FromMilliseconds(millisecondsTimeOut));
        }

        /// <summary>
        /// Asynchronously tries to take the lock during the specified time and indicates whether the lock has been taken.
        /// </summary>
        /// <remarks>
        /// A property on <see cref="AsyncLockReleaser"/> indicates whether the lock has been taken.
        /// </remarks>
        /// <param name="timeOut">The time which to wait until the lock is taken.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous waiting operation. Its return value is used to free
        /// the lock.
        /// </returns>
        public async Task<AsyncLockReleaser> LockAsync(TimeSpan timeOut)
        {
            return new AsyncLockReleaser(semaphore, await this.semaphore.WaitAsync(timeOut).ConfigureAwait(false));
        }

        /// <summary>
        /// Asynchronously tries to take the lock.
        /// </summary>
        /// <remarks>
        /// An Exception will be thrown if the lock could not be taken during the time the <paramref name="token"/> is valid.
        /// </remarks>
        /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous waiting.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous waiting operation. Its return value is used to free
        /// the lock.
        /// </returns>
        /// <exception cref="OperationCanceledException">The <see cref="CancellationToken"/> was cancelled.</exception>
        public async Task<AsyncLockReleaser> LockAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await this.semaphore.WaitAsync(token).ConfigureAwait(false);
            return new AsyncLockReleaser(semaphore);
        }

        /// <summary>
        /// Disposes the <see cref="AsyncLock"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose managed resources as well.</param>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                this.semaphore.Dispose();
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.semaphore != null);
        }

        /// <summary>
        /// The struct used to release the previously acquired lock. Usable via using-pattern.
        /// </summary>
        /// <remarks>
        /// Always make sure to release lock via <see cref="M:Release"/> or <see cref="M:Dispose"/>, otherwise the lock
        /// will not be released and deadlocks will occur.
        /// </remarks>
        public struct AsyncLockReleaser : IDisposable
        {
            /// <summary>
            /// The <see cref="SemaphoreSlim"/> holding the lock.
            /// </summary>
            private readonly SemaphoreSlim semaphore;

            /// <summary>
            /// Inidicates whether the lock has been taken.
            /// </summary>
            public bool LockTaken { get; private set; }

            /// <summary>
            /// Initializes a new <see cref="AsyncLockReleaser"/>.
            /// </summary>
            /// <param name="semaphore">The <see cref="SemaphoreSlim"/> holding the lock.</param>
            public AsyncLockReleaser(SemaphoreSlim semaphore)
                : this(semaphore, true)
            {
                Contract.Requires<ArgumentNullException>(semaphore != null);
            }

            /// <summary>
            /// Initializes a new <see cref="AsyncLockReleaser"/>.
            /// </summary>
            /// <param name="lockTaken">Inidicates whether the lock has been taken.</param>
            /// <param name="semaphore">The <see cref="SemaphoreSlim"/> holding the lock.</param>
            public AsyncLockReleaser(SemaphoreSlim semaphore, bool lockTaken)
                : this()
            {
                Contract.Requires<ArgumentNullException>(semaphore != null);

                this.LockTaken = lockTaken;
                this.semaphore = semaphore;
            }

            /// <summary>
            /// Releases the lock if it has been taken.
            /// </summary>
            public void Release()
            {
                if (this.LockTaken && this.semaphore != null)
                {
                    this.semaphore.Release();
                }
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
