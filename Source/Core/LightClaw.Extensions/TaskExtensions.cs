using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    /// <summary>
    /// Contains extensions to the <see cref="Task"/>-class.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Casts the result of the specified <see cref="Task{T}"/> to a subclass.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of result of the original <see cref="Task{T}"/>.</typeparam>
        /// <typeparam name="TDerived">The <see cref="Type"/> of the derived class to cast to.</typeparam>
        /// <param name="task">The <see cref="Task{T}"/> whose result is to be casted.</param>
        /// <returns>A <see cref="Task{T}"/> returning the casted result.</returns>
        public static Task<TDerived> Downcast<T, TDerived>(this Task<T> task)
            where TDerived : T
        {
            Contract.Requires<ArgumentNullException>(task != null);
            Contract.Ensures(Contract.Result<Task<TDerived>>() != null);

            TaskCompletionSource<TDerived> tcs = new TaskCompletionSource<TDerived>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult((TDerived)t.Result);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        /// <summary>
        /// Casts the result of the specified <see cref="Task{T}"/> to a base class.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of result of the original <see cref="Task{T}"/>.</typeparam>
        /// <typeparam name="TBase">The <see cref="Type"/> of the base class to cast to.</typeparam>
        /// <param name="task">The <see cref="Task{T}"/> whose result is to be casted.</param>
        /// <returns>A <see cref="Task{T}"/> returning the casted result.</returns>
        public static Task<TBase> Upcast<T, TBase>(this Task<T> task)
            where T : TBase
        {
            Contract.Requires<ArgumentNullException>(task != null);
            Contract.Ensures(Contract.Result<Task<TBase>>() != null);

            TaskCompletionSource<TBase> tcs = new TaskCompletionSource<TBase>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(t.Result);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }
    }
}
