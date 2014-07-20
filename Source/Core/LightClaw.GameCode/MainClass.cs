using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.GameCode
{
    /// <summary>
    /// Represents the entry point for custom LightClaw game code.
    /// </summary>
    public class MainClass : Manager
    {
        /// <summary>
        /// Represents the entry point for custom game code.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(String[] args)
        {

        }

        /// <summary>
        /// Called when the <see cref="MainClass"/> was started.
        /// </summary>
        /// <returns>A <see cref="Task"/> describing the asynchronous process.</returns>
        protected override void OnInitialization()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the <see cref="MainClass"/> was stopped.
        /// </summary>
        /// <returns>A <see cref="Task"/> describing the asynchronous process.</returns>
        protected override void OnShutdown()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the <see cref="MainClass"/> was updated.
        /// </summary>
        /// <returns>A <see cref="Task"/> describing the asynchronous process.</returns>
        protected override bool OnUpdate(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
