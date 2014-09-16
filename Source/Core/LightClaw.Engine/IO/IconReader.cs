using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents an <see cref="IContentReader"/> reading <see cref="Icon"/>s.
    /// </summary>
    public class IconReader : IContentReader
    {
        /// <summary>
        /// Asynchronously reads the icon.
        /// </summary>
        /// <param name="contentManager">The <see cref="IContentManager"/> that triggered the loading process.</param>
        /// <param name="resourceString">The resource string of the asset to be loaded.</param>
        /// <param name="assetStream">A <see cref="Stream"/> of the asset's data.</param>
        /// <param name="assetType">The <see cref="Type"/> of asset to read.</param>
        /// <param name="parameter">A parameter the client specifies when requesting an asset.</param>
        /// <returns>
        /// The deserialized asset or <c>null</c> if an error occured or the specified <paramref name="assetType"/> is not an icon.
        /// </returns>
        public Task<object> ReadAsync(IContentManager contentManager, ResourceString resourceString, Stream assetStream, Type assetType, object parameter)
        {
            return (assetType == typeof(Icon)) ?
                Task.FromResult<object>(new Icon(assetStream)) :
                Task.FromResult<object>(null);
        }
    }
}
