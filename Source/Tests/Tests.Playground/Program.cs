using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.GameCode;

namespace Tests.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Scene s = new Scene(
                "TestScene",
                new GameObject(
                    new BasicMesh()
                )
            );

            Directory.CreateDirectory(".\\Game");
            using (FileStream fs = File.Create(".\\Game\\Start.lcs"))
            {
                try
                {
                    s.SaveAsync(fs).Wait();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            using (FileStream fs = File.Create(".\\Game\\StartRaw.lcs"))
            {
                try
                {
                    s.SaveRawAsync(fs).Wait();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
