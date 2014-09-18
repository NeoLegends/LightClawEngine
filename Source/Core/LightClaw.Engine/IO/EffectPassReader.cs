using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Extensions;
using Newtonsoft.Json;

namespace LightClaw.Engine.IO
{
    public class EffectPassReader : IContentReader
    {
        public Task<object> ReadAsync(IContentManager contentManager, ResourceString resourceString, Stream assetStream, Type assetType, object parameter)
        {
            if (typeof(EffectPass).IsAssignableFrom(assetType))
            {
                return Task.FromResult((object)null);
            }
            else
            {
                return Task.FromResult((object)null);
            }
        }
    }
}
