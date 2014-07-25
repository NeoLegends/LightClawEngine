using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.IO
{
    public class SceneReader : IContentReader
    {
        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(Scene));
        }

        public Task<object> ReadAsync(string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            return (assetType == typeof(Scene)) ?
                Task.Run(() => new NetDataContractSerializer().ReadObject(assetStream)) :
                Task.FromResult((object)null);
        }
    }
}
