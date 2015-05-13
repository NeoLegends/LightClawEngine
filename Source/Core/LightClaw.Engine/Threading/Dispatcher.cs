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
    [ThreadMode(ThreadMode.Safe)]
    [DebuggerDisplay("Thread: {Thread.ManagedThreadId}, Count: {Count}")]
    public class Dispatcher : DisposableEntity
    {
        private const int PM_REMOVE = 1;

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
        /// The work queues.
        /// </summary>
        private readonly SortedDictionary<DispatcherPriority, ConcurrentQueue<Action>> queues = new SortedDictionary<DispatcherPriority, ConcurrentQueue<Action>>(
            new Dictionary<DispatcherPriority, ConcurrentQueue<Action>>
            {
                { DispatcherPriority.Immediate, new ConcurrentQueue<Action>() },
                { DispatcherPriority.High, new ConcurrentQueue<Action>() },
                { DispatcherPriority.Normal, new ConcurrentQueue<Action>() },
                { DispatcherPriority.Background, new ConcurrentQueue<Action>() },
            },
            new ReverseComparer<DispatcherPriority>()
        );

        /// <summary>
        /// Indicates whether the <see cref="Dispatcher"/> is currently running.
        /// </summary>
        private int isRunning = 0;

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

            return this.Invoke(action, priority, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously invokes the specified <see cref="Action"/> with the specified <paramref name="priority"/> on the target thread
        /// and allows cancellation.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="priority">The priority of the action to run.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation to the method.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution of the <paramref name="action"/>.</returns>
        public Task Invoke(Action action, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(action != null);
            this.CheckDisposed();

            return this.Invoke(() => { action(); return true; }, priority, token);
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

            return this.Invoke(action, param, priority, CancellationToken.None);
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
        public Task Invoke<TParam>(Action<TParam> action, TParam param, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(action != null);
            this.CheckDisposed();

            return this.Invoke(() => action(param), priority, token);
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

            return this.Invoke(func, priority, CancellationToken.None);
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
        public async Task<TResult> Invoke<TResult>(Func<TResult> func, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(func != null);
            this.CheckDisposed();

            if (priority == DispatcherPriority.Immediate && ThreadF.IsCurrentThread(this.Thread))
            {
                return func();
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
                            tcs.TrySetResult(func());
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

            return this.Invoke(func, param, priority, CancellationToken.None);
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
        public Task<TResult> Invoke<TParam, TResult>(Func<TParam, TResult> func, TParam param, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(func != null);
            this.CheckDisposed();

            return this.Invoke(() => func(param), priority, token);
        }

        /// <summary>
        /// Asynchronously invokes the specified delegate on the target thread with late binding.
        /// </summary>
        /// <remarks>Warning: Do not use this method in tight loops, it is slow!</remarks>
        /// <param name="del">The <see cref="Delegate"/> to invoke.</param>
        /// <param name="parameters">Delegate parameters.</param>
        /// <returns>The value returned by the <see cref="Delegate"/>.</returns>
        public Task<object> InvokeDynamic(Delegate del, params object[] parameters)
        {
            Contract.Requires<ArgumentNullException>(del != null);
            Contract.Requires<ArgumentNullException>(parameters != null);

            return this.InvokeDynamic(del, DispatcherPriority.Normal, parameters);
        }

        /// <summary>
        /// Asynchronously invokes the specified delegate on the target thread with the specified <paramref name="priority"/> 
        /// with late binding.
        /// </summary>
        /// <remarks>Warning: Do not use this method in tight loops, it is slow!</remarks>
        /// <param name="del">The <see cref="Delegate"/> to invoke.</param>
        /// <param name="parameters">Delegate parameters.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run.</param>
        /// <returns>The value returned by the <see cref="Delegate"/>.</returns>
        public Task<object> InvokeDynamic(Delegate del, DispatcherPriority priority, params object[] parameters)
        {
            Contract.Requires<ArgumentNullException>(del != null);
            Contract.Requires<ArgumentNullException>(parameters != null);

            return this.InvokeDynamic(del, priority, CancellationToken.None, parameters);
        }

        /// <summary>
        /// Asynchronously invokes the specified delegate on the target thread with the specified <paramref name="priority"/> 
        /// with late binding and allows for cancellation.
        /// </summary>
        /// <remarks>Warning: Do not use this method in tight loops, it is slow!</remarks>
        /// <param name="del">The <see cref="Delegate"/> to invoke.</param>
        /// <param name="parameters">Delegate parameters.</param>
        /// <param name="priority">The <paramref name="priority"/> of the action to run.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation to the method.</param>
        /// <returns>The value returned by the <see cref="Delegate"/>.</returns>
        public Task<object> InvokeDynamic(Delegate del, DispatcherPriority priority, CancellationToken token, params object[] parameters)
        {
            Contract.Requires<ArgumentNullException>(del != null);
            Contract.Requires<ArgumentNullException>(parameters != null);

            return this.Invoke(new Func<object[], object>(del.DynamicInvoke), parameters, priority, token);
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

                // Empty the execution stack if we're disposed.
                int tries = 0; // Make sure we don't get stuck in infinite loops
                while (!this.queues.Values.All(q => q.IsEmpty) && tries ++ < 5)
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
        public void Stop()
        {
            this.isRunning = 0;
        }

        /// <summary>
        /// Disposes the <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed.</param>
        [ThreadMode(ThreadMode.Affine)]
        protected override void Dispose(bool disposing)
        {
            try
            {
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

            ConcurrentQueue<Action> queue;
            if (!this.queues.TryGetValue(priority, out queue))
            {
                throw new ArgumentException("Parameter priority was not one of the predefined values!");
            }
            return queue;
        }

        private void ExecuteStack()
        {
            List<Action> actions = null; // Do not allocate the list, if we can avoid it
            foreach (ConcurrentQueue<Action> queue in this.queues.Values)
            {
                Action action;
                while (queue.TryDequeue(out action))
                {
                    if (action != null)
                    {
                        if (actions == null)
                        {
                            actions = new List<Action>();
                        }
                        actions.Add(action);
                    }
                }

                if (actions != null)
                {
                    for (int i = 0; i < actions.Count; i++)
                    {
                        try
                        {
                            actions[i].Invoke();
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
                                throw;
                            }
                        }
                    }

                    actions.Clear();
                }
            }
        }
    }
}
