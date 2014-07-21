using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace LightClaw.Engine.Core
{
    public interface IGame : IDisposable, INameable
    {
        GameTime CurrentGameTime { get; }

        int Height { get; set; }

        ISceneManager SceneManager { get; }

        bool SuppressDraw { get; set; }

        int Width { get; set; }

        void Run();
    }
}
