using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;
using Munq;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents the entry point for any game.
    /// </summary>
    public class LightClawEngine
    {
        /// <summary>
        /// A <see cref="ILog"/> used to track application messages.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(LightClawEngine));

        /// <summary>
        /// Backign field.
        /// </summary>
        private static readonly IocContainer _DefaultIocContainer = new IocContainer();

        /// <summary>
        /// A <see cref="IocContainer"/> used to resolve instances of certain interfaces at runtime.
        /// </summary>
        public static IocContainer DefaultIocContainer
        {
            get
            {
                Contract.Ensures(Contract.Result<IocContainer>() != null);

                return _DefaultIocContainer;
            }
        }

        /// <summary>
        /// Initializes all static members of <see cref="LightClawEngine"/>.
        /// </summary>
        static LightClawEngine()
        {
            DefaultIocContainer.Register<IContentManager>(d => new ContentManager());
        }

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            logger.Info(() => "Starting engine...");

            try
            {
                Contract.Assume(GeneralSettings.Default.StartScene != null);
                using (IGame game = new Game(GeneralSettings.Default.StartScene) { Name = GeneralSettings.Default.GameName })
                {
                    DefaultIocContainer.Register<IGame>(d => game);
                    game.Run();
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(() => "An error of type '{0}' with message '{1}' occured. Engine shutting down.".FormatWith(ex.GetType().AssemblyQualifiedName, ex.Message), ex);
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}
