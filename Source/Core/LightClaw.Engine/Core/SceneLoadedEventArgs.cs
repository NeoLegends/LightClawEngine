using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public class SceneLoadedEventArgs : EventArgs
    {
        public Scene NewScene { get; private set; }

        public SceneLoadedEventArgs(Scene newScene)
        {
            this.NewScene = newScene;
        }
    }
}
