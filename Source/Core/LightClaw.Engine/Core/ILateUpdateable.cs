using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public interface ILateUpdateable
    {
        event EventHandler<ParameterEventArgs> LateUpdating;

        event EventHandler<ParameterEventArgs> LateUpdated;

        void LateUpdate();
    }
}
