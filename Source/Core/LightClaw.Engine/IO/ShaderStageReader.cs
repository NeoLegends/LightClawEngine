using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.IO
{
    public class ShaderStageReader : IContentReader
    {
        public async Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            if (assetType == typeof(ShaderStage) && parameter != null)
            {
                using (StreamReader sr = new StreamReader(assetStream))
                {
                    return new ShaderStage(await sr.ReadToEndAsync(), (ShaderType)parameter);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
