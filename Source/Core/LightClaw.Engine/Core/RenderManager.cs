using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;

namespace LightClaw.Engine.Core
{
    internal class RenderManager : Entity, IRenderManager
    {
        public event EventHandler OnLoad;

        public event EventHandler OnUnload;

        public event EventHandler OnRender;

        public event EventHandler<ResizeEventArgs> OnResize;

        public event EventHandler<FrameEventArgs> OnUpdate;

        private GameTime _CurrentGameTime;

        public GameTime CurrentGameTime
        {
            get
            {
                return _CurrentGameTime;
            }
            private set
            {
                this.SetProperty(ref _CurrentGameTime, value);
            }
        }

        private readonly IGameWindow _GameWindow = new GameWindow(
            VideoSettings.Default.Width,
            VideoSettings.Default.Height,
            new GraphicsMode(),
            GeneralSettings.Default.WindowTitle
        )
        {
            WindowState = VideoSettings.Default.WindowState,
            VSync = VideoSettings.Default.VSync
        };

        public IGameWindow GameWindow
        {
            get
            {
                Contract.Ensures(Contract.Result<IGameWindow>() != null);

                return _GameWindow;
            }
        }

        private bool _SuppressDraw;

        public bool SuppressDraw
        {
            get
            {
                return _SuppressDraw;
            }
            set
            {
                this.SetProperty(ref _SuppressDraw, value);
            }
        }

        public RenderManager()
        {
            this.GameWindow.Load += (s, e) => this.Raise(this.OnLoad);
            this.GameWindow.Unload += (s, e) => this.Raise(this.OnUnload);
            this.GameWindow.RenderFrame += (s, e) => this.Raise(this.OnRender);
            this.GameWindow.Resize += (s, e) => this.RaiseOnResize(this.GameWindow.Width, this.GameWindow.Height);
            this.GameWindow.UpdateFrame += (s, e) => this.RaiseOnUpdate(e);
        }

        ~RenderManager()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.GameWindow.Dispose();
        }

        private void Raise(EventHandler handler, EventArgs args = null)
        {
            if (handler != null)
            {
                handler(this, args ?? EventArgs.Empty);
            }
        }

        private void RaiseOnResize(int newWidth, int newHeight)
        {
            Contract.Requires<ArgumentOutOfRangeException>(newWidth >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(newHeight >= 0);

            EventHandler<ResizeEventArgs> handler = this.OnResize;
            if (handler != null)
            {
                handler(this, new ResizeEventArgs(newWidth, newHeight));
            }
        }

        private void RaiseOnUpdate(FrameEventArgs args)
        {
            Contract.Requires<ArgumentNullException>(args != null);

            EventHandler<FrameEventArgs> handler = this.OnUpdate;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}
