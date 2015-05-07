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
            EffectData data = new EffectData(
                new EffectData.StageSources(
                    new EffectData.StageData("Test/Basic.frag"),
                    new EffectData.StageData("Test/Basic.vert", "inVertexPosition", "inVertexTexCoord", "inVertexNormals", "inVertexBinormals", "inVertexTangent", "inVertexColor")
                ),
                new[] { "mat_MVP" }
            );

            using (FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EffectData.json"), FileMode.Create, FileAccess.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                new JsonSerializer()
                {
                    DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate,
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                }.Serialize(sw, data);
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
