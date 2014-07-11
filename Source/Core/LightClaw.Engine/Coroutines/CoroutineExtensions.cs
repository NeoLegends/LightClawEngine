using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Coroutines
{
    public static class CoroutineExtensions
    {
        public static bool Step(this ICoroutineContext context)
        {
            object current;
            return context.Step(out current);
        }

        public static void StepUntil(this ICoroutineContext context, TimeSpan timeOut)
        {
            Stopwatch st = Stopwatch.StartNew();
            while (st.Elapsed < timeOut && context.Step()) { }
        }
    }
}
