using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightClaw.AssetPipeline.AssetProcessing
{
    /// <summary>
    /// Method signature for a LightClaw asset processing module
    /// </summary>
    public interface IAssetProcessable
    {
        /// <summary>
        /// Method being called for processing a single asset file
        /// </summary>
        /// <param name="file">The path of the file to process</param>
        /// <returns>Whether the file processing was successful or not</returns>
        bool OnProcess(String file);

        /// <summary>
        /// The list of supported file types the asset processor supports
        /// </summary>
        /// <example>{ ".jpg", ".png", ".tif" }</example>
        String[] SupportedFileExtensions { get; }
    }
}
