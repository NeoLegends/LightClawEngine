using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    public class IconReader : IContentReader
    {
        public Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            return (assetType == typeof(Icon)) ?
                Task.FromResult<object>(new Icon(assetStream)) :
                Task.FromResult<object>(null);
        }
    }
}
