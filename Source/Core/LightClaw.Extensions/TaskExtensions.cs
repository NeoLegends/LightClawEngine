using System;
using System.Collections.Generic;
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
        /// Casts the result of the specified <see cref="Task{T}"/> to a base class.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of result of the original <see cref="Task{T}"/>.</typeparam>
        /// <typeparam name="TBase"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Task<TBase> Upcast<T, TBase>(this Task<T> task)
            where T : TBase
        {
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
