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
    public class SceneReader : Entity, IContentReader
    {
        /// <summary>
        /// Checks whether the <see cref="IContentReader"/> can read assets of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="assetType">The type of the asset that is about to be read.</param>
        /// <returns><c>true</c> if the <see cref="IContentReader"/> can read assets of the specified <see cref="Type"/>, otherwise <c>false</c>.</returns>
        public bool CanRead(Type assetType)
        {
            return (assetType == typeof(Scene));
        }

        /// <summary>
        /// Asynchronously reads the <see cref="Scene"/>.
        /// </summary>
        /// <param name="parameters"><see cref="ContentReadParameters"/> containing information about the asset to be loaded.</param>
        /// <returns>
        /// The deserialized asset or <c>null</c> if an error occured or the specified <paramref name="assetType"/>
        /// cannot be read.
        /// </returns>
        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            try
            {
                return await Scene.Load(parameters.AssetStream);
            }
            catch 
            {
                Logger.Warn(s => "Loading scene '{0}' from the compressed format failed, trying to load uncompressed...".FormatWith(s), parameters.ResourceString);
            }

            return await Scene.LoadRaw(parameters.AssetStream);
        }
    }
}
