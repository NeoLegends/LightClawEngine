using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public interface IGLObject : IDisposable
    {
        int Handle { get; }
    }
}
