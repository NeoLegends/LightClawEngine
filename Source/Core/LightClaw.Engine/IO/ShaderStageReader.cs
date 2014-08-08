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
    /// <summary>
    /// Represents an <see cref="IContentReader"/> reading <see cref="ShaderStage"/>s.
    /// </summary>
    public class ShaderStageReader : IContentReader
    {
        /// <summary>
        /// Asynchronously reads the shader stage.
        /// </summary>
        /// <param name="contentManager">The <see cref="IContentManager"/> that triggered the loading process.</param>
        /// <param name="resourceString">The resource string of the asset to be loaded.</param>
        /// <param name="assetStream">A <see cref="Stream"/> of the asset's data.</param>
        /// <param name="assetType">The <see cref="Type"/> of asset to read.</param>
        /// <param name="parameter">A parameter the client specifies when requesting an asset.</param>
        /// <returns>
        /// The deserialized asset or <c>null</c> if an error occured or the specified <paramref name="assetType"/>
        /// cannot be read.
        /// </returns>
        public async Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            if (assetType == typeof(ShaderStage) && parameter != null && parameter is ShaderType)
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
