using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents an <see cref="IContentReader"/> reading <see cref="Scene"/>s.
    /// </summary>
    public class SceneReader : Entity, IContentReader
    {
        /// <summary>
        /// Checks whether the <see cref="SceneReader"/> can read assets of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="assetType">The <see cref="Type"/> to read.</param>
        /// <param name="parameter">A parameter the client specifies when requesting an asset.</param>
        /// <returns>
        /// <c>true</c> if assets of the specified <paramref name="assetType"/> can be read, otherwise <c>false</c>.
        /// </returns>
        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(Scene));
        }

        /// <summary>
        /// Asynchronously reads the <see cref="Scene"/>.
        /// </summary>
        /// <param name="parameters">
        /// <see cref="ContentReadParameters"/> containing information about the asset to be loaded.
        /// </param>
        /// <returns>The deserialized <see cref="Scene"/>.</returns>
        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            return await Scene.Load(parameters.AssetStream); // await for covariance
        }
    }
}
