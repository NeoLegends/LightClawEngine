using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Utilities
{
    public class TestUtilities
    {
        public static TimeSpan DoMeasuredAction(Action action)
        {
            Stopwatch st = Stopwatch.StartNew();
            action();
            return st.Elapsed;
        }

        public static async Task<TimeSpan> DoMeasuredAction(Func<Task> func)
        {
            Stopwatch st = Stopwatch.StartNew();
            await func();
            return st.Elapsed;
        }
    }
}
