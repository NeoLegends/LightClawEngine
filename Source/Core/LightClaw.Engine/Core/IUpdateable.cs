using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public interface IUpdateable
    {
        event EventHandler<ParameterEventArgs> Updating;

        event EventHandler<ParameterEventArgs> Updated;

        void Update(GameTime gameTime);
    }
}
