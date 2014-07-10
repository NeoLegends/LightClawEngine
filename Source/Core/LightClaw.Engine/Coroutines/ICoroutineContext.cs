using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Coroutines
{
    public interface ICoroutineContext : IUpdateable
    {
        bool IsEnabled { get; set; }

        bool IsFinished { get; }

        void Reset();

        bool Step();

        bool Step(out object current);

        void Step(TimeSpan timeOut);
    }
}
