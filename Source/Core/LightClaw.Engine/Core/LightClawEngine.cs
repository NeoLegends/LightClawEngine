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

        public static IocContainer DefaultIocContainer
        {
            get
            {
                return _DefaultIocContainer;
            }
        }

        static LightClawEngine()
        {
            DefaultIocContainer.Register<IContentManager>(d => new ContentManager());
        }

        static void Main(string[] args)
        {
            try
            {
                IGameCodeInterface gameCodeInterface = new GameCodeInterface(Assembly.LoadFrom(GeneralSettings.Default.EntryAssembly));

                DefaultIocContainer.Register<IGameCodeInterface>(d => gameCodeInterface);
                DefaultIocContainer.Register<LightClawSerializer>(d => new LightClawSerializer(gameCodeInterface));

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
    }
}
