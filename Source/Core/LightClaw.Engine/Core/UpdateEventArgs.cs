using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public class UpdateEventArgs : EventArgs
    {
        public GameTime GameTime { get; private set; }

        public int Pass { get; private set; }

        public UpdateEventArgs(GameTime gameTime, int pass)
        {
            Contract.Requires<ArgumentOutOfRangeException>(pass >= 0);

            this.GameTime = gameTime;
            this.Pass = pass;
        }
    }
}
