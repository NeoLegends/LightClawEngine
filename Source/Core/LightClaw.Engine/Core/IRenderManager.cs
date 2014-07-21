using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Platform;

namespace LightClaw.Engine.Core
{
    public interface IRenderManager : IDisposable
    {
        event EventHandler OnLoad;

        event EventHandler OnUnload;

        event EventHandler OnRender;

        event EventHandler<ResizeEventArgs> OnResize;

        event EventHandler<FrameEventArgs> OnUpdate;

        event EventHandler<WindowStateChangedEventArgs> OnWindowStateChanged;

        GameTime CurrentGameTime { get; }

        IGameWindow GameWindow { get; }

        bool SuppressDraw { get; set; }

        void Run(double updateRate);
    }
}
