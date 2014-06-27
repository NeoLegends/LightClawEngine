using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public class SceneLoadingEventArgs : EventArgs
    {
        public Scene NewScene { get; private set; }

        public Scene OldScene { get; private set; }

        public SceneLoadingEventArgs(Scene newScene, Scene oldScene)
        {
            this.NewScene = newScene;
            this.OldScene = oldScene;
        }
    }
}
