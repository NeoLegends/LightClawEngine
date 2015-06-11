using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using NLog;

namespace LightClaw.Engine.Threading
{
    /// <summary>
    /// Represents a message pump.
    /// </summary>
    [ThreadMode(true)]
    [DebuggerDisplay("Thread: {Thread.ManagedThreadId}, Count: {Count}")]
    public class Dispatcher : DisposableEntity
    {
        /// <summary>
        /// All living dispatchers.
        /// </summary>
        private static readonly ConcurrentDictionary<Thread, Dispatcher> dispatchers = new ConcurrentDictionary<Thread, Dispatcher>();

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> for the current thread.
        /// </summary>
        public static Dispatcher Current
        {
            get
            {
                Contract.Ensures(Contract.Result<Dispatcher>() != null);

                return dispatchers.GetOrAdd(Thread.CurrentThread, t => new Dispatcher(t));
            }
        }

        /// <summary>
        /// A list to temporarily store the actions to be executed.
        /// </summary>
        private readonly List<Action> actionList = new List<Action>();

        /// <summary>
        /// Indicates whether the <see cref="Dispatcher"/> is currently running.
        /// </summary>
        private volatile int isRunning = 0;

        /// <summary>
        /// The work queues.
        /// </summary>
        private readonly SortedDictionary<DispatcherPriority, ConcurrentQueue<Action>> queues = new SortedDictionary<DispatcherPriority, ConcurrentQueue<Action>>(
            ((DispatcherPriority[])Enum.GetValues(typeof(DispatcherPriority))).ToDictionary(
                dp => dp, 
                dp => new ConcurrentQueue<Action>()
            ),
            new ReverseComparer<DispatcherPriority>()
        );

        /// <summary>
        /// The <see cref="AutoResetEvent"/> used to trigger when new operations have arrived.
        /// </summary>
        private readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

        /// <summary>
        /// Occurs when a exception was raised inside the dispatcher loop.
        /// </summary>
        public event EventHandler<UnhandledDispatcherExceptionEventArgs> UnhandledException;

        /// <summary>
        /// The amount of items to be processed.
        /// </summary>
        public int Count
        {
            get
            {
                return this.queues.Values.Sum(q => q.Count);
            }
        }

        /// <summary>
        /// Gets whether the current thread is the <see cref="Thread"/> associated with the <see cref="Dispatcher"/>.
        /// </summary>
        public bool IsOnThread
        {
            get
            {
                return ThreadF.IsCurrentThread(this.Thread);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private readonly Thread _Thread;

        /// <summary>
        /// The <see cref="Thread"/> the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        public Thread Thread
        {
            get
            {
                Contract.Ensures(Contract.Result<Thread>() != null);

                return _Thread;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="t">The <see cref="Thread"/> to initialize from.</param>
        private Dispatcher(Thread t)
        {
            Contract.Requires<ArgumentNullException>(t != null);

            this._Thread = t;
        }

        /// <summary>
        /// Asynchronously invokes the specified <see cref="Action"/> with normal priority on the target thread.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution of the <paramref name="action"/>.</returns>
        public Task Invoke(Action action)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            return this.Invoke(action, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Asynchronously invokes the specified <see cref="Action"/> with the specified <paramref name="priority"/> on the target thread.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="priority">The priority of the action to run.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution of the <paramref name="action"/>.</returns>
        public Task Invoke(Action action, DispatcherPriority priority)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            return this.Invoke(ct => action(), priority, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously invokes the specified <see cref="Action"/> with the specified <paramref name="priority"/> on the target thread
        /// and allows cancellation.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="priority">The priority of the action to run.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation to the method.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution of the <paramref name="action"/>.</returns>
        public Task Invoke(Action<CancellationToken> action, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(action != null);
            this.CheckDisposed();

            return this.Invoke(ct => { action(ct); return true; }, priority, token);
        }

        /// <summary>
        /// Asynchronously invokes the specified parameterized <see cref="Action"/> with normal priority on the target thread.
        /// </summary>
        /// <typeparam name="TParam">The <see cref="Type"/> of the parameter.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="param">The parameter value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution of the <paramref name="action"/>.</returns>
        public Task Invoke<TParam>(Action<TParam> action, TParam param)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            return this.Invoke(action, param, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Asynchronously invokes the specified parameterized <see cref="Action"/> with the specified <paramref name="priority"/> on the
        /// target thread.
        /// </summary>
        /// <typeparam name="TParam">The <see cref="Type"/> of the parameter.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="param">The parameter value.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution of the <paramref name="action"/>.</returns>
        public Task Invoke<TParam>(Action<TParam> action, TParam param, DispatcherPriority priority)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            return this.Invoke((ct, p) => action(p), param, priority, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously invokes the specified parameterized <see cref="Action"/> with the specified <paramref name="priority"/> on the
        /// target thread and allows cancellation.
        /// </summary>
        /// <typeparam name="TParam">The <see cref="Type"/> of the parameter.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="param">The parameter value.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation to the method.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution of the <paramref name="action"/>.</returns>
        public Task Invoke<TParam>(Action<CancellationToken, TParam> action, TParam param, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(action != null);
            this.CheckDisposed();

            return this.Invoke(ct => action(ct, param), priority, token);
        }

        /// <summary>
        /// Asynchronously executes the specified <see cref="Func{T}"/> on the target thread and returns its return value.
        /// </summary>
        /// <typeparam name="TResult">The <see cref="Type"/> of the result value.</typeparam>
        /// <param name="func">The <see cref="Func{T}"/> to execute.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public Task<TResult> Invoke<TResult>(Func<TResult> func)
        {
            Contract.Requires<ArgumentNullException>(func != null);

            return this.Invoke(func, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Asynchronously executes the specified <see cref="Func{T}"/> with the specified <paramref name="priority"/> on the 
        /// target thread and returns its return value.
        /// </summary>
        /// <typeparam name="TResult">The <see cref="Type"/> of the result value.</typeparam>
        /// <param name="func">The <see cref="Func{T}"/> to execute.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public Task<TResult> Invoke<TResult>(Func<TResult> func, DispatcherPriority priority)
        {
            Contract.Requires<ArgumentNullException>(func != null);

            return this.Invoke(ct => func(), priority, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously executes the specified <see cref="Func{T}"/> with the specified <paramref name="priority"/> on the 
        /// target thread and returns its return value. This method allows for cancellation.
        /// </summary>
        /// <typeparam name="TResult">The <see cref="Type"/> of the result value.</typeparam>
        /// <param name="func">The <see cref="Func{T}"/> to execute.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation to the method.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public async Task<TResult> Invoke<TResult>(Func<CancellationToken, TResult> func, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(func != null);
            this.CheckDisposed();

            if (priority == DispatcherPriority.Immediate && ThreadF.IsCurrentThread(this.Thread))
            {
                return func(token);
            }
            else
            {
                TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
                using (token.Register(() => tcs.TrySetCanceled()))
                {
                    this.InvokeSlim(() =>
                    {
                        try
                        {
                            tcs.TrySetResult(func(token));
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    }, priority);

                    return await tcs.Task;
                }
            }
        }

        /// <summary>
        /// Asynchronously executes the specified parameterized <see cref="Func{T}"/> on the target thread and returns its return value.
        /// </summary>
        /// <typeparam name="TParam">The <see cref="Type"/> of the parameter.</typeparam>
        /// <typeparam name="TResult">The <see cref="Type"/> of the result value.</typeparam>
        /// <param name="func">The <see cref="Func{T}"/> to execute.</param>
        /// <param name="param">The parameter value.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public Task<TResult> Invoke<TParam, TResult>(Func<TParam, TResult> func, TParam param)
        {
            Contract.Requires<ArgumentNullException>(func != null);

            return this.Invoke(func, param, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Asynchronously executes the specified parameterized <see cref="Func{T}"/> with the specified <paramref name="priority"/> on the 
        /// target thread and returns its return value.
        /// </summary>
        /// <typeparam name="TParam">The <see cref="Type"/> of the parameter.</typeparam>
        /// <typeparam name="TResult">The <see cref="Type"/> of the result value.</typeparam>
        /// <param name="func">The <see cref="Func{T}"/> to execute.</param>
        /// <param name="param">The parameter value.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public Task<TResult> Invoke<TParam, TResult>(Func<TParam, TResult> func, TParam param, DispatcherPriority priority)
        {
            Contract.Requires<ArgumentNullException>(func != null);

            return this.Invoke((ct, p) => func(p), param, priority, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously executes the specified parameterized <see cref="Func{T}"/> with the specified <paramref name="priority"/> on the 
        /// target thread and returns its return value. This method allows for cancellation.
        /// </summary>
        /// <typeparam name="TParam">The <see cref="Type"/> of the parameter.</typeparam>
        /// <typeparam name="TResult">The <see cref="Type"/> of the result value.</typeparam>
        /// <param name="func">The <see cref="Func{T}"/> to execute.</param>
        /// <param name="param">The parameter value.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation to the method.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public Task<TResult> Invoke<TParam, TResult>(Func<CancellationToken, TParam, TResult> func, TParam param, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(func != null);
            this.CheckDisposed();

            return this.Invoke(ct => func(ct, param), priority, token);
        }

        /// <summary>
        /// Asynchronously invokes the specified <see cref="Action"/> with normal priority on the target thread.
        /// Does not return a <see cref="Task"/> in order to improve execution time.
        /// </summary>
        /// <remarks>
        /// Warning: If <paramref name="action"/> throws an exception, <see cref="E:UnhandledException"/> will be raised.
        /// If none of the event handlers handles the event, the application will crash.
        /// </remarks>
        /// <param name="action">The action to invoke.</param>
        public void InvokeSlim(Action action)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            this.InvokeSlim(action, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Asynchronously invokes the specified <see cref="Action"/> with the specified <paramref name="priority"/> on the target thread.
        /// Does not return a <see cref="Task"/> in order to improve execution time.
        /// </summary>
        /// <remarks>
        /// Warning: If <paramref name="action"/> throws an exception, <see cref="E:UnhandledException"/> will be raised.
        /// If none of the event handlers handles the event, the application will crash.
        /// </remarks>
        /// <param name="action">The action to invoke.</param>
        /// <param name="priority">The priority of the action to run.</param>
        public void InvokeSlim(Action action, DispatcherPriority priority)
        {
            Contract.Requires<ArgumentNullException>(action != null);
            this.CheckDisposed();

            if (priority >= DispatcherPriority.Immediate && ThreadF.IsCurrentThread(this.Thread))
            {
                action();
            }
            else
            {
                this.GetPriorityQueue(priority).Enqueue(action);
                this.resetEvent.Set();
            }
        }

        /// <summary>
        /// Asynchronously invokes the specified parameterized <see cref="Action"/> with normal priority on the target thread.
        /// Does not return a <see cref="Task"/> in order to improve execution time.
        /// </summary>
        /// <remarks>
        /// Warning: If <paramref name="action"/> throws an exception, <see cref="E:UnhandledException"/> will be raised.
        /// If none of the event handlers handles the event, the application will crash.
        /// </remarks>
        /// <typeparam name="TParam">The <see cref="Type"/> of the parameter.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="param">The parameter value.</param>
        public void InvokeSlim<TParam>(Action<TParam> action, TParam param)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            this.InvokeSlim(action, param, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Asynchronously invokes the specified parameterized <see cref="Action"/> with the specified <paramref name="priority"/> on the
        /// target thread. Does not return a <see cref="Task"/> in order to improve execution time.
        /// </summary>
        /// <remarks>
        /// Warning: If <paramref name="action"/> throws an exception, <see cref="E:UnhandledException"/> will be raised.
        /// If none of the event handlers handles the event, the application will crash.
        /// </remarks>
        /// <typeparam name="TParam">The <see cref="Type"/> of the parameter.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="param">The parameter value.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run.</param>
        public void InvokeSlim<TParam>(Action<TParam> action, TParam param, DispatcherPriority priority)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            this.InvokeSlim(() => action(param), priority);
        }

        /// <summary>
        /// Executes the specified <paramref name="action"/> immediately, if the calling thread is the <see cref="Dispatcher"/>s thread,
        /// or enqueues it with the specified <see cref="DispatcherPriority"/> if not.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run, if it cannot be run instantly.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public async Task ImmediateOr(Action action, DispatcherPriority priority)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            if (ThreadF.IsCurrentThread(this.Thread))
            {
                action();
            }
            else
            {
                await this.Invoke(action, priority);
            }
        }

        /// <summary>
        /// Executes the specified <paramref name="action"/> immediately, if the calling thread is the <see cref="Dispatcher"/>s thread,
        /// or enqueues it with the specified <see cref="DispatcherPriority"/> if not.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run, if it cannot be run instantly.</param>
        /// <param name="token">A <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public Task ImmediateOr(Action<CancellationToken> action, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            return this.ImmediateOr(() => action(token), priority);
        }

        /// <summary>
        /// Executes the specified <paramref name="action"/> immediately, if the calling thread is the <see cref="Dispatcher"/>s thread,
        /// or enqueues it with the specified <see cref="DispatcherPriority"/> if not.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run, if it cannot be run instantly.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public Task ImmediateOr<TParam>(Action<TParam> action, TParam param, DispatcherPriority priority)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            return this.ImmediateOr((ct, p) => action(p), param, priority, CancellationToken.None);
        }

        /// <summary>
        /// Executes the specified <paramref name="action"/> immediately, if the calling thread is the <see cref="Dispatcher"/>s thread,
        /// or enqueues it with the specified <see cref="DispatcherPriority"/> if not.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run, if it cannot be run instantly.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="token">A <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous execution of the <paramref name="func"/>.</returns>
        public async Task ImmediateOr<TParam>(Action<CancellationToken, TParam> action, TParam param, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(action != null);
            
            if (ThreadF.IsCurrentThread(this.Thread))
            {
                action(token, param);
            }
            else
            {
                await this.Invoke(action, param, priority, token);
            }
        }

        /// <summary>
        /// Starts the dispatcher loop.
        /// </summary>
        /// <remarks>
        /// This method may only be executed from the thread the <see cref="Dispatcher"/> was created on. Otherwise an exception
        /// will be thrown.
        /// </remarks>
        [ThreadMode(ThreadMode.Affine)]
        public void Run()
        {
            ThreadF.ThrowIfNotCurrentThread(this.Thread);

            SynchronizationContext currentContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(new LightClawSynchronizationContext(this));
                this.isRunning = 1;

                while (!this.IsDisposed && this.isRunning != 0)
                {
                    this.resetEvent.WaitOne();
                    this.ExecuteStack();
                }

                // Empty the execution stack if we're stopping.
                int tries = 0; // Make sure we don't get stuck in infinite loops
                while (!this.queues.Values.All(q => q.IsEmpty) && tries++ < 5)
                {
                    this.ExecuteStack();
                }
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(currentContext);
            }
        }

        /// <summary>
        /// Stops a running dispatcher loop.
        /// </summary>
        [ThreadMode(true)]
        public void Stop()
        {
            this.isRunning = 0;
        }

        /// <summary>
        /// Disposes the <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed.</param>
        [ThreadMode(ThreadMode.Safe)]
        protected override void Dispose(bool disposing)
        {
            try
            {
                this.Stop();

                Dispatcher outVal;
                dispatchers.TryRemove(this.Thread, out outVal);
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void CheckDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(typeof(Dispatcher).Name);
            }
        }

        private ConcurrentQueue<Action> GetPriorityQueue(DispatcherPriority priority)
        {
            Contract.Ensures(Contract.Result<ConcurrentQueue<Action>>() != null);

            // As long as we're not adding items during runtime to the queue list,
            // we can use it concurrently as per https://msdn.microsoft.com/de-de/library/ms132319(v=vs.110).aspx
            ConcurrentQueue<Action> queue;
            if (!this.queues.TryGetValue(priority, out queue))
            {
                throw new ArgumentException("Parameter priority was not one of the predefined values!");
            }
            if (queue == null)
            {
                throw new InvalidOperationException("The queue of priority '{0}' was null!".FormatWith(priority));
            }
            return queue;
        }

        private void ExecuteStack()
        {
            foreach (ConcurrentQueue<Action> queue in this.queues.Values)
            {
                this.actionList.Clear();

                Action action;
                while (queue.TryDequeue(out action))
                {
                    if (action != null)
                    {
                        this.actionList.Add(action);
                    }
                }

                int count = this.actionList.Count; // Small performance gain
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        this.actionList[i].Invoke();
                    }
                    catch (Exception ex)
                    {
                        UnhandledDispatcherExceptionEventArgs e = new UnhandledDispatcherExceptionEventArgs(ex);
                        this.Raise(this.UnhandledException, e);
                        this.Log.Error(
                            "An {0}exception of type '{1}' was thrown inside the {2}.".FormatWith(
                                e.IsHandled ? string.Empty : "unhandled ",
                                ex.GetType().FullName,
                                typeof(Dispatcher).Name
                            ),
                            ex
                        );
                        if (!e.IsHandled)
                        {
                            // If we have an error, copy the rest of the elements back into the queue
                            // so we won't loose them.
                            for (int j = i + 1; j < count; j++)
                            {
                                queue.Enqueue(this.actionList[j]);
                            }
                            throw;
                        }
                    }
                }
            }
        }
    }
}
