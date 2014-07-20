using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightClaw.TeamServer.Server
{
    /// <summary>
    /// Dispatches work stored in a <see cref="ConcurrentQueue{T}"/> to a number of <see cref="Task"/>s.
    /// </summary>
    public class ThreadedDispatcher : IDisposable
    {
        /// <summary>
        /// Used for locking.
        /// </summary>
        private object taskLock = new object();

        /// <summary>
        /// An array of <see cref="Task"/>s 
        /// </summary>
        private List<Task> tasks = new List<Task>();

        /// <summary>
        /// A <see cref="CancellationTokenSource"/> used to break the working loops.
        /// </summary>
        private CancellationTokenSource ctSource = new CancellationTokenSource();

        /// <summary>
        /// Indicates whether the instance has already been disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// The work to be done.
        /// </summary>
        public ConcurrentQueue<Action> Work { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ThreadedDispatcher"/> with 5 working threads.
        /// </summary>
        public ThreadedDispatcher() : this(10) { }

        /// <summary>
        /// Initializes a new <see cref="ThreadedDispatcher"/> setting the amount of working threads.
        /// </summary>
        /// <param name="threads">The number of threads to use.</param>
        public ThreadedDispatcher(int threads)
        {
            CancellationToken token = this.ctSource.Token;
            for (int i = 0; i < threads; i++)
            {
                Task worker = new Task(() =>
                {
                    while (true)
                    {
                        Action work;
                        if (this.Work.TryDequeue(out work))
                        {
                            work();
                        }

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                },  token);

                this.tasks.Add(worker);
            }
        }

        /// <summary>
        /// Finalizes the object releasing all allocated resource before the object is reclaimed by garbage collection.
        /// </summary>
        ~ThreadedDispatcher()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Starts the worker tasks.
        /// </summary>
        public void Start()
        {
            lock (taskLock)
            {
                foreach (Task t in this.tasks)
                {
                    t.Start();
                }
            }
        }

        /// <summary>
        /// Stops the worker tasks.
        /// </summary>
        public void Stop()
        {
            lock (taskLock)
            {
                this.ctSource.Cancel();
            }
        }

        /// <summary>
        /// Stops the worker tasks synchroneously.
        /// </summary>
        public void StopSync()
        {
            lock (taskLock)
            {
                this.Stop();
                Task.WaitAll(this.tasks.ToArray());
            }
        }

        /// <summary>
        /// Disposes the instance releasing all object-allocated resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes the instance releasing all unmanaged resources and optionally managed resources as well.
        /// </summary>
        /// <param name="disposing">A boolean indicating whether to release managed resources as well.</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (taskLock)
            {
                if (!this.IsDisposed)
                {
                    this.StopSync();

                    if (this.tasks != null)
                    {
                        foreach (Task t in this.tasks)
                        {
                            t.Dispose();
                        }
                    }

                    this.Work = null;

                    this.IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }
    }
}
