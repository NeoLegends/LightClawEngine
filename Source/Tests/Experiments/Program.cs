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
            EffectPassData epd = new EffectPassData("BasicEffect", new EffectStageSources(
                    new EffectStageData("Game/Effects/BasicEffect.vert", "vPosition", "vTexCoords", "vNormal", "vBinormal", "vTanget", "vColor"),
                    new EffectStageData("Game/Effects/BasicEffect.frag")
                ), new[] { "mat_MVP" });

            JsonSerializerSettings settings = new JsonSerializerSettings() 
            { 
                Formatting = Formatting.Indented, 
                NullValueHandling = NullValueHandling.Ignore 
            };
            settings.Converters.Add(new LightClaw.Engine.IO.ResourceStringConverter());
            
            string json = JsonConvert.SerializeObject(epd, settings);
            Console.WriteLine(json);

            EffectPassData epdDeser = JsonConvert.DeserializeObject<EffectPassData>(json);
            Console.WriteLine(JsonConvert.SerializeObject(epdDeser, settings));

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }
    }
}
