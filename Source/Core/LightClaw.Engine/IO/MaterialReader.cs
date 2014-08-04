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
    public class MaterialReader : IContentReader
    {
        public Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            return Task.Run(() => new NetDataContractSerializer().ReadObject(assetStream));
        }
    }
}
