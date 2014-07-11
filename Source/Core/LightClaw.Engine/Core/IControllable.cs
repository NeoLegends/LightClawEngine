﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public interface IControllable : IDisposable, IUpdateable
    {
        event EventHandler<ParameterEventArgs> Enabling;

        event EventHandler<ParameterEventArgs> Enabled;

        event EventHandler<ParameterEventArgs> Disabling;

        event EventHandler<ParameterEventArgs> Disabled;

        event EventHandler<ParameterEventArgs> Loading;

        event EventHandler<ParameterEventArgs> Loaded;

        bool IsEnabled { get; }

        bool IsLoaded { get; }

        void Enable();

        void Disable();

        void Load();
    }
}
