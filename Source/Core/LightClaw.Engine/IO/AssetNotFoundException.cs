using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// The error that occures when an asset can not be found.
    /// </summary>
    [Serializable]
    public class AssetNotFoundException : Exception
    {
        /// <summary>
        /// The <see cref="Type"/> of asset to be loaded.
        /// </summary>
        public Type AssetType
        {
            get
            {
                return (Type)this.Data["AssetType"];
            }
            private set
            {
                this.Data["AssetType"] = value;
            }
        }

        /// <summary>
        /// The path to the asset.
        /// </summary>
        public ResourceString ResourceString
        {
            get
            {
                return (ResourceString)this.Data["ResourceString"];
            }
            private set
            {
                this.Data["ResourceString"] = (string)value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="AssetNotFoundException"/>.
        /// </summary>
        /// <param name="resourceString">The path to the asset.</param>
        /// <param name="assetType">The <see cref="Type"/> of asset to be loaded.</param>
        public AssetNotFoundException(ResourceString resourceString, Type assetType)
            : base("Asset of type {0} could not be found.".FormatWith(assetType.FullName))
        {
            Contract.Requires<ArgumentNullException>(assetType != null);

            this.AssetType = assetType;
            this.ResourceString = resourceString;
        }
    }
}
