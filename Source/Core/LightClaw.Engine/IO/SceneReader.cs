using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using log4net;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents an <see cref="IContentReader"/> reading <see cref="Scene"/>s.
    /// </summary>
    public class SceneReader : IContentReader
    {
        /// <summary>
        /// Asynchronously reads the <see cref="Scene"/>.
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
        public async Task<object> ReadAsync(IContentManager contentManager, ResourceString resourceString, Stream assetStream, Type assetType, object parameter)
        {
            // Await for covariance
            return await ((assetType == typeof(Scene)) ? Scene.Load(assetStream) : Task.FromResult((Scene)null));
        }
    }
}
