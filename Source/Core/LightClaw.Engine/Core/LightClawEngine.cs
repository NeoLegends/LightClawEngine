using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;

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
        private static readonly Container _DefaultIocContainer = new Container();

        /// <summary>
        /// A <see cref="Container"/> used to resolve instances of certain interfaces at runtime.
        /// </summary>
        [CLSCompliant(false)]
        public static Container DefaultIocContainer
        {
            get
            {
                Contract.Ensures(Contract.Result<DryIoc.Container>() != null);

                return _DefaultIocContainer;
            }
        }

        /// <summary>
        /// The managed thread ID of the main thread the <see cref="IGame"/> was created on.
        /// </summary>
        public static int MainThreadId { get; private set; }

        /// <summary>
        /// Initializes all static members of <see cref="LightClawEngine"/>.
        /// </summary>
        static LightClawEngine()
        {
            DefaultIocContainer.Register<IContentManager, ContentManager>(
                Reuse.Singleton,
                contentManagerType => contentManagerType.GetConstructor(Type.EmptyTypes)
            );
        }

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void Main(string[] args)
        {
            logger.Info("LightClaw starting up.");

#if DESKTOP && !MONO
            string profileRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LightClaw",
                "ProfileOptimization"
            );

            logger.Info(pr => "Initializing profile optimization. Profile root will be '{0}'.".FormatWith(pr), profileRoot);
            Directory.CreateDirectory(profileRoot);
            ProfileOptimization.SetProfileRoot(profileRoot);

            string profile = Assembly.GetExecutingAssembly().GetName().FullName;
            logger.Info(p => "Enabling profile '{0}'.".FormatWith(p), profile);
            ProfileOptimization.StartProfile(profile);
#endif

            try
            {
                MainThreadId = Thread.CurrentThread.ManagedThreadId;

                string startScene = GeneralSettings.Default.StartScene;
                if (startScene == null)
                {
                    throw new NullReferenceException("Start scene settings entry was null.");
                }

                logger.Info(s => "Initializing Game with start scene '{0}'.".FormatWith(s), startScene);
                using (Game game = new Game(startScene))
                {
                    logger.Info("Game created, starting up.");
                    DefaultIocContainer.RegisterInstance<Game>(game);
                    game.Run();
                }
            }
            catch (Exception exception)
            {
                logger.Fatal(ex => "An error of type '{0}' with message '{1}' occured. Engine shutting down.".FormatWith(ex.GetType().AssemblyQualifiedName, ex.Message), exception, exception);
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}
