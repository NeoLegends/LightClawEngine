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
using LightClaw.Engine.Graphics;
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
        /// A <see cref="IocContainer"/> used to resolve instances of certain interfaces at runtime.
        /// </summary>
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
        static void Main(string[] args)
        {
#if DESKTOP
            ProfileOptimization.SetProfileRoot(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "LightClaw",
                    "ProfileOptimization"
                )
            );
            ProfileOptimization.StartProfile(Assembly.GetExecutingAssembly().GetName().FullName);
#endif

            logger.Info(() => "Starting engine...");

            try
            {
                MainThreadId = Thread.CurrentThread.ManagedThreadId;

                string startScene = GeneralSettings.Default.StartScene;
                if (startScene == null)
                {
                    throw new NullReferenceException("Start scene settings entry was null.");
                }

                using (IGame game = new Game(startScene) { Name = GeneralSettings.Default.GameName })
                {
                    DefaultIocContainer.RegisterInstance<IGame>(game);
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
