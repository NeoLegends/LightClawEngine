using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.IO
{
    public class ShaderReader : DispatcherEntity, IContentReader
    {
        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(Shader) && parameter is ShaderType);
        }

        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            if (!(parameters.Parameter is ShaderType))
            {
                throw new ArgumentException("The parameter must not be null and must be a {0}.".FormatWith(typeof(ShaderType).FullName));
            }

            string source;
            using (StreamReader sr = new StreamReader(parameters.AssetStream, Encoding.UTF8, true, 2048, true))
            {
                source = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            return await this.Dispatcher.Invoke(() => new Shader(source, (ShaderType)parameters.Parameter)).ConfigureAwait(false);
        }
    }
}
