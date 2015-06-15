using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.GameCode;
using Newtonsoft.Json;

namespace Tests.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Scene s = new Scene(
                "TestScene",
                new GameObject(
                    new Mesh("Game/Dragonborn.FBX"),
                    new TestTransformer()
                )
            ))
            {
                s.SaveRawAsync("Game/Start.lcs").Wait();
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
