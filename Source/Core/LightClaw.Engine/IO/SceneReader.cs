using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using log4net;

namespace LightClaw.Engine.IO
{
    public class SceneReader : IContentReader
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SceneReader));

        public Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            return (assetType == typeof(Scene)) ?
                Task.Run(() =>
                { 
                    logger.Info("Loading a scene from '{0}'.".FormatWith(resourceString));
                    return new NetDataContractSerializer().ReadObject(assetStream);
                }) :
                Task.FromResult((object)null);
        }
    }
}
