using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Core.Threading
{
    [TestClass]
    public class DispatcherTest
    {
        [TestMethod]
        public void TestDispatcherSpeed()
        {
            ExecuteDispatcherTest(d => d.Invoke(ShortFunction));
        }

        [TestMethod]
        public void TestDispatcherSpeedInvokeSlim()
        {
            ExecuteDispatcherTest(d => d.InvokeSlim(ShortFunction));
        }

        private static void ExecuteDispatcherTest(Action<Dispatcher> dispatcherFunction)
        {
            TimeSpan elapsedWithFor = TimeSpan.Zero;
            TimeSpan elapsedWithDispatcher = TimeSpan.Zero;
            int count = 1000000;
            int runs = 10;

            Stopwatch st = Stopwatch.StartNew();

            for (int z = 0; z < runs; z++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                st.Restart();
                for (int i = 0; i < count; i++)
                {
                    ShortFunction();
                }
                elapsedWithFor += st.Elapsed;

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Dispatcher dispatcher = Dispatcher.Current;
                for (int i = 0; i < count; i++)
                {
                    dispatcherFunction(dispatcher);
                }
                dispatcher.Dispose();
                st.Restart();
                dispatcher.Run();

                elapsedWithDispatcher += st.Elapsed;
            }

            Console.WriteLine("Executing the short function with for times took {1} on average.",
                count,
                (double)elapsedWithFor.Ticks / runs
            );
            Console.WriteLine("Executing the short function with the dispatcher took {1} on average. This is {2} times longer than with for.",
                count,
                (double)elapsedWithDispatcher.Ticks / runs,
                (double)elapsedWithDispatcher.Ticks / (double)elapsedWithFor.Ticks
            );
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ShortFunction()
        {
            for (int i = 0; i < 15; i++)
            {
                MathF.IsPrime(i);
            }
        }
    }
}
