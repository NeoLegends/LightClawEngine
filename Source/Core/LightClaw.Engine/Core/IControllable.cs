using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public interface IControllable : IDisposable
    {
        event EventHandler<EnabledChangedEventArgs> EnabledChanged;
    
        event EventHandler<LoadedChangedEventArgs> LoadedChanged;

        event EventHandler<ControllableEventArgs> Updated;

        bool IsEnabled { get; }

        bool IsLoaded { get; }

        void Enable();

        void Disable();

        void Load();

        void Update();
    }
}
