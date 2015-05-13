using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using Newtonsoft.Json;

namespace LightClaw.Engine.IO
{
    public class EffectReader : IContentReader
    {
        private static readonly JsonSerializer serializer = new JsonSerializer();

        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(Effect));
        }

        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            List<string> effects;
            using (StreamReader sr = new StreamReader(parameters.AssetStream, Encoding.UTF8, true, 1024, true))
            using (JsonTextReader jtr = new JsonTextReader(sr))
            {
                effects = serializer.Deserialize<List<string>>(jtr);
            }

            return new Effect(
                await Task.WhenAll(effects.Select(e => parameters.ContentManager.LoadAsync<EffectPass>(e, parameters.CancellationToken))).ConfigureAwait(false),
                false
            );
        }
    }
}
