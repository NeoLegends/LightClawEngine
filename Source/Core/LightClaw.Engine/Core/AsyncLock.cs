using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public class AsyncLock
    {
        private SemaphoreSlim semaphore;

        public AsyncLock() : this(1) { }

        public AsyncLock(int initialCount)
        {
            this.semaphore = new SemaphoreSlim(initialCount);
        }

        public Task<AsyncLockReleaser> LockAsync()
        {
            return this.semaphore.WaitAsync().ContinueWith(t => new AsyncLockReleaser(this.semaphore), TaskContinuationOptions.ExecuteSynchronously);
        }

        public struct AsyncLockReleaser : IDisposable
        {
            private readonly SemaphoreSlim semaphore;

            public AsyncLockReleaser(SemaphoreSlim semaphore)
                : this()
            {
                Contract.Requires<ArgumentNullException>(semaphore != null);

                this.semaphore = semaphore;
            }

            public void Release()
            {
                this.semaphore.Release();
            }

            void IDisposable.Dispose()
            {
                this.Release();
            }
        }
    }
}
