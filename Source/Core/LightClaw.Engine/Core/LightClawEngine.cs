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
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using NLog;

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
        private static readonly Logger logger = LogManager.GetLogger(typeof(LightClawEngine).FullName);

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

        public void Start()
        {

        }

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void Main(string[] args)
        {
            logger.Info(
                () => "LightClaw starting up on {0}, {1} bit.".FormatWith(Environment.OSVersion.VersionString, Environment.Is64BitProcess ? "64" : "32")
            );

#if DESKTOP
            string profileRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LightClaw",
                "ProfileOptimization"
            );

            logger.Info(() => "Initializing profile optimization. Profile root will be '{0}'.".FormatWith(profileRoot));
            Directory.CreateDirectory(profileRoot);
            ProfileOptimization.SetProfileRoot(profileRoot);

            string profile = Assembly.GetExecutingAssembly().GetName().FullName;
            logger.Info(() => "Enabling profile '{0}'.".FormatWith(profile));
            ProfileOptimization.StartProfile(profile);
#endif
            MainThreadId = Thread.CurrentThread.ManagedThreadId;

            try
            {
                string startScene = GeneralSettings.Default.StartScene;
                if (startScene == null)
                {
                    throw new NullReferenceException("Start scene settings entry was null.");
                }

                logger.Info("Initializing Game with start scene '{0}'.".FormatWith(startScene));
                using (Game game = new Game(startScene))
                {
                    logger.Info("Game created, starting up.");
                    DefaultIocContainer.RegisterInstance<IGame>(game);
                    game.Run();
                }
            }
            catch (Exception ex)
            {
                logger.Fatal("An error of type '{0}' with message '{1}' occured. Engine shutting down.".FormatWith(ex.GetType().AssemblyQualifiedName, ex.Message), ex);
                throw;
            }
            finally
            {
                LogManager.Shutdown();
                DefaultIocContainer.Dispose();
                Dispatcher.Current.Dispose();
            }
        }
    }
}
