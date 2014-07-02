using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public class SceneLoadEventArgs : EventArgs
    {
        public Scene NewScene { get; private set; }

        public Scene OldScene { get; private set; }

        public SceneLoadEventArgs(Scene newScene, Scene oldScene)
        {
            this.NewScene = newScene;
            this.OldScene = oldScene;
        }
    }
}
