using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using Munq;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    public class LightClawEngine
    {
        private static readonly IocContainer _DefaultIocContainer = new IocContainer();

        [CLSCompliant(false)]
        public static IocContainer DefaultIocContainer
        {
            get
            {
                return _DefaultIocContainer;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                IGameCodeInterface gameCodeInterface = GetGameCodeInterface();

                DefaultIocContainer.Register<IGameCodeInterface>(d => gameCodeInterface);
                DefaultIocContainer.Register<LightClawSerializer>(d => new LightClawSerializer(gameCodeInterface));
                DefaultIocContainer.Register<IContentManager>(d => new ContentManager());
                using (IGame game = new Game(gameCodeInterface, GeneralSettings.Default.StartScene) { Name = GeneralSettings.Default.GameName })
                {
                    DefaultIocContainer.Register<IGame>(d => game);

                    game.Run();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }

        static IGameCodeInterface GetGameCodeInterface()
        {
            Assembly gameCodeAssembly = Assembly.LoadFrom(GeneralSettings.Default.EntryAssembly);
            Type gameCodeType = gameCodeAssembly.GetType(GeneralSettings.Default.EntryClass, false, true);
            if (gameCodeType == null)
            {
                throw new NotSupportedException("The game code assembly was loaded, but it was not possible to find a game code class.");
            }
            return (IGameCodeInterface)Activator.CreateInstance(gameCodeType);
        }
    }
}
