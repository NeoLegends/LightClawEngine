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
            using (FileStream fs = new FileStream("Shaders/Basic.shr", FileMode.Create, FileAccess.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                new JsonSerializer()
                {
                    DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate,
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                }.Serialize(sw, new EffectPassDescription("Shaders/Basic.vert", "Shaders/Basic.frag"));
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
