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
        public static IocContainer DefaultIocContainer { get; private set; }

        static void Main(string[] args)
        {
            try
            {
                DefaultIocContainer = new IocContainer();

                using (IGame game = new Game(Assembly.LoadFrom(GeneralSettings.Default.GameCodeAssmbly), GeneralSettings.Default.StartScene) { Name = GeneralSettings.Default.GameName })
                {
                    DefaultIocContainer.Register<IContentManager>(d => new ContentManager());
                    DefaultIocContainer.Register<LightClawSerializer>(d => new LightClawSerializer());
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
