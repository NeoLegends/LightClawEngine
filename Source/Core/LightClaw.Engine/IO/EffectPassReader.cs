using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.IO
{
    public class EffectPassReader : IContentReader
    {
        private static readonly JsonSerializer serializer = JsonSerializer.CreateDefault();

        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(EffectPass));
        }

        public Task<object> ReadAsync(ContentReadParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
