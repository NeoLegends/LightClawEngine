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
        public void TestDispatcherSpeedLong()
        {
            Dispatcher dispatcher = new Dispatcher();

            TimeSpan elapsedWithFor = TimeSpan.Zero;
            TimeSpan elapsedWithDispatcher = TimeSpan.Zero;
            int count = 100;
            int runs = 10;

            Stopwatch st = Stopwatch.StartNew();

            for (int z = 0; z < runs; z++)
            {
                st.Restart();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                for (int i = 0; i < count; i++)
                {
                    long primeNumber = FindPrimeNumber(i * 100);
                }

                elapsedWithFor += st.Elapsed;

                GC.Collect();
                GC.WaitForPendingFinalizers();

                st.Restart();
                for (int i = 0; i < count; i++)
                {
                    dispatcher.Invoke(j => FindPrimeNumber(j * 100), i);
                }
                dispatcher.Pop();
                
                elapsedWithDispatcher += st.Elapsed;
            }

            Console.WriteLine("Calculating prime numbers (long operation) with a for loop took {0} ticks on average.", (double)elapsedWithFor.Ticks / runs);
            Console.WriteLine("Calculating prime numbers (long operation) with the dispatcher took {0} ticks on average. This is {1} times the original time.", 
                (double)elapsedWithDispatcher.Ticks / runs, 
                (double)elapsedWithDispatcher.Ticks / (double)elapsedWithFor.Ticks
            );
        }

        [TestMethod]
        public void TestDispatcherSpeedShort()
        {
            Dispatcher dispatcher = new Dispatcher();

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

                for (int i = 0; i < count; i++)
                {
                    dispatcher.Invoke(ShortFunction);
                }
                st.Restart();
                dispatcher.Pop();

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

        private static long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
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
