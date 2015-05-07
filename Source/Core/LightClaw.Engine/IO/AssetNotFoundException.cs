using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    [Serializable]
    public class AssetNotFoundException : Exception
    {
        public Type AssetType { get; private set; }

        public ResourceString ResourceString { get; private set; }

        public AssetNotFoundException(ResourceString resourceString, Type assetType)
            : base("Asset of type {0} could not be found.".FormatWith(assetType.FullName))
        {
            Contract.Requires<ArgumentNullException>(assetType != null);

            this.AssetType = assetType;
            this.ResourceString = resourceString;
        }
    }
}
