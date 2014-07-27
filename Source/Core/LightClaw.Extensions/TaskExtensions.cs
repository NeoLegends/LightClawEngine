using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<T> WhenFirst<T>(this IEnumerable<Task<T>> collection, Predicate<Task<T>> predicate)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(predicate != null);

            List<Task<T>> workingCopy = collection.ToList();

            while (workingCopy.Count > 0)
            {
                Task<T> finishedTask = await Task.WhenAny(workingCopy);
                if (predicate(finishedTask))
                {
                    return finishedTask.Result;
                }
                else
                {
                    workingCopy.Remove(finishedTask);
                }
            }

            return default(T);
        }
    }
}
