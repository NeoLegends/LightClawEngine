using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.Threading
{
    /// <summary>
    /// Represents a sink used to dispatch work onto another thread.
    /// </summary>
    [ThreadMode(ThreadMode.Safe | ThreadMode.Affine)]
    public class Dispatcher : DisposableEntity
    {
        /// <summary>
        /// All currently existing <see cref="Dispatcher"/>s with their respective threads.
        /// </summary>
        private static readonly ConcurrentDictionary<Thread, Dispatcher> dispatchers = new ConcurrentDictionary<Thread, Dispatcher>();

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> for the current <see cref="Thread"/> and creates one, if it doesn't exist.
        /// </summary>
        public static Dispatcher Current
        {
            get
            {
                Contract.Ensures(Contract.Result<Dispatcher>() != null);

                return dispatchers.GetOrAdd(Thread.CurrentThread, t => new Dispatcher());
            }
        }

        /// <summary>
        /// The work queues.
        /// </summary>
        private readonly ConcurrentDictionary<DispatcherPriority, ConcurrentQueue<Task>> queues = new ConcurrentDictionary<DispatcherPriority, ConcurrentQueue<Task>>();

        /// <summary>
        /// A <see cref="Stopwatch"/> used to measure execution time.
        /// </summary>
        private readonly Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Backing field.
        /// </summary>
        private Thread _Thread = Thread.CurrentThread;

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
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Thread, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Dispatcher"/>.
        /// </summary>
        public Dispatcher() { }

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

            Task task = new Task(action, token);
            this.queues.GetOrAdd(priority, p => new ConcurrentQueue<Task>()).Enqueue(task);
            return task;
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
        /// Asynchronously invokes the specified  parameterized <see cref="Action"/> with the specified <paramref name="priority"/> on the
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

            Task task = new Task(o => action((TParam)o), param, token);
            this.queues.GetOrAdd(priority, p => new ConcurrentQueue<Task>()).Enqueue(task);
            return task;
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
        public Task<TResult> Invoke<TResult>(Func<TResult> func, DispatcherPriority priority, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(func != null);
            this.CheckDisposed();

            Task<TResult> task = new Task<TResult>(func, token);
            this.queues.GetOrAdd(priority, p => new ConcurrentQueue<Task>()).Enqueue(task);
            return task;
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

            Task<TResult> task = new Task<TResult>(o => func((TParam)o), param, token);
            this.queues.GetOrAdd(priority, p => new ConcurrentQueue<Task>()).Enqueue(task);
            return task;
        }

        /// <summary>
        /// Asynchronously invokes the specified delegate on the target thread with late binding.
        /// </summary>
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
        /// Executes the work on the current thread until all queues are empty.
        /// </summary>
        public void Pop()
        {
            ThreadF.ThrowIfNotCurrentThread(this.Thread);

            this.Pop(Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Synchronously executes the work on the current thread either until all queues are empty or until the time has run out.
        /// </summary>
        /// <param name="targetTime">The approximate time the <see cref="Dispatcher"/> is allowed to execute.</param>
        public void Pop(TimeSpan targetTime)
        {
            ThreadF.ThrowIfNotCurrentThread(this.Thread);

            this.stopWatch.Restart();
            foreach (ConcurrentQueue<Task> taskQueue in this.queues.OrderByDescending(kvp => kvp.Key).Select(kvp => kvp.Value))
            {
                Task task = null;
#warning There might be a better way to check for infinity on a timespan than < 0
                while ((this.stopWatch.Elapsed < targetTime || targetTime < TimeSpan.Zero) && taskQueue.TryDequeue(out task))
                {
                    task.RunSynchronously();
                }
            }
        }

        /// <summary>
        /// Disposes the <see cref="Dispatcher"/> unregistering it from the dictionary.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            Dispatcher outObject;
            dispatchers.TryRemove(this.Thread, out outObject);
            this.Pop();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Checks whether the <see cref="Dispatcher"/> has been disposed and throws an exception.
        /// </summary>
        private void CheckDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(typeof(Dispatcher).Name);
            }
        }

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> for the specified <paramref name="thread"/>.
        /// </summary>
        /// <param name="thread">The <see cref="Thread"/> to obtain the <see cref="Dispatcher"/> of.</param>
        /// <returns>The <see cref="Dispatcher"/> of the target thread.</returns>
        /// <exception cref="KeyNotFoundException">The <see cref="Dispatcher"/> for the specified <see cref="Thread"/> was not yet created.</exception>
        public static Dispatcher GetDispatcher(Thread thread)
        {
            Contract.Requires<ArgumentNullException>(thread != null);

            return dispatchers[thread];
        }

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> for the specified <paramref name="threadId"/>.
        /// </summary>
        /// <param name="threadId">The ID of the <see cref="Thread"/> to obtain the <see cref="Dispatcher"/> of.</param>
        /// <returns>The <see cref="Dispatcher"/> of the target thread.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="Dispatcher"/> for the specified <see cref="Thread"/> was not yet created.</exception>
        public static Dispatcher GetDispatcher(int threadId)
        {
            return dispatchers.First(kvp => kvp.Key.ManagedThreadId == threadId).Value;
        }

        /// <summary>
        /// Tries to get the <see cref="Dispatcher"/> for the specified <paramref name="thread"/>.
        /// </summary>
        /// <param name="thread">The <see cref="Thread"/> to obtain the <see cref="Dispatcher"/> of.</param>
        /// <param name="result">The <see cref="Dispatcher"/> for the specified <paramref name="thread"/>, if the method succeeds.</param>
        /// <returns><c>true</c> if the <see cref="Dispatcher"/> could be obtained, otherwise <c>false</c>.</returns>
        public static bool TryGetDispatcher(Thread thread, out Dispatcher result)
        {
            Contract.Requires<ArgumentNullException>(thread != null);
            Contract.Ensures(!Contract.Result<bool>() || Contract.ValueAtReturn(out result) != null);

            return dispatchers.TryGetValue(thread, out result);
        }

        /// <summary>
        /// Tries to get the <see cref="Dispatcher"/> for the specified <paramref name="threadId"/>.
        /// </summary>
        /// <param name="threadId">The <see cref="Thread"/> to obtain the <see cref="Dispatcher"/> of.</param>
        /// <param name="result">The <see cref="Dispatcher"/> for the specified <paramref name="threadId"/>, if the method succeeds.</param>
        /// <returns><c>true</c> if the <see cref="Dispatcher"/> could be obtained, otherwise <c>false</c>.</returns>
        public static bool TryGetDispatcher(int threadId, out Dispatcher result)
        {
            Contract.Ensures(!Contract.Result<bool>() || Contract.ValueAtReturn(out result) != null);

            return (result = dispatchers.FirstOrDefault(kvp => kvp.Key.ManagedThreadId == threadId).Value) != null;
        }
    }
}
