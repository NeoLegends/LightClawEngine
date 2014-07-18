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
        public bool CanRead(Type assetType, object parameter)
        {
            return assetType == typeof(Icon);
        }

        public Task<object> ReadAsync(string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            if (assetType == typeof(Icon))
            {
                return Task.FromResult<object>(new Icon(assetStream));
            }
            else
            {
                return Task.FromResult<object>(null);
            }
        }
    }
}
