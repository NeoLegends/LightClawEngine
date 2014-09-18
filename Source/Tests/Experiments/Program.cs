using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using Newtonsoft.Json;

namespace Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            EffectData epd = new EffectData("BasicEffect", 
                new EffectData.StageSources(
                    new EffectData.StageData("Game/Effects/BasicEffect.vert", "vPosition", "vTexCoords", "vNormal", "vBinormal", "vTanget", "vColor"),
                    new EffectData.StageData("Game/Effects/BasicEffect.frag")
                ), 
                new[] { "mat_MVP" }
            );

            JsonSerializerSettings settings = new JsonSerializerSettings() 
            { 
                Formatting = Formatting.Indented, 
                NullValueHandling = NullValueHandling.Ignore 
            };
            settings.Converters.Add(new LightClaw.Engine.IO.ResourceStringConverter());
            
            string json = JsonConvert.SerializeObject(epd, settings);
            Console.WriteLine(json);

            EffectData epdDeser = JsonConvert.DeserializeObject<EffectData>(json);
            Console.WriteLine(JsonConvert.SerializeObject(epdDeser, settings));

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }
    }
}
