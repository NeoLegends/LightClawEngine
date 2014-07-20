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
                DefaultIocContainer.Register<IContentManager>(d => new ContentManager());

                Assembly gameCodeAssembly = Assembly.LoadFrom(GeneralSettings.Default.EntryAssembly);
                Type gameCodeType = gameCodeAssembly.GetType(GeneralSettings.Default.EntryClass, false, true);
                if (gameCodeType == null || !typeof(IGameCodeInterface).IsAssignableFrom(gameCodeType))
                {
                    gameCodeType = gameCodeAssembly.GetTypesByBase<IGameCodeInterface>(true).FirstOrDefault();
                }
                if (gameCodeType == null)
                {
                    throw new NotSupportedException("The game code assembly was loaded, but it was not possible to find a game code class.");
                }

                using (Game game = new Game((IGameCodeInterface)Activator.CreateInstance(gameCodeType), GeneralSettings.Default.StartScene)
                {
                    Name = GeneralSettings.Default.GameName
                })
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
    }
}
