using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents an <see cref="IContentReader"/> reading <see cref="Material"/>s.
    /// </summary>
    public class MaterialReader : IContentReader
    {
        /// <summary>
        /// Asynchronously reads the <see cref="Material"/>.
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
        public Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            return Task.Run(() => new NetDataContractSerializer().ReadObject(assetStream));
        }
    }
}
