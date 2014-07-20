using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public interface IDrawable
    {
        event EventHandler<ParameterEventArgs> Drawing;

        event EventHandler<ParameterEventArgs> Drawn;

        void Draw();
    }
}
