using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    public class StringContentReader : IContentReader
    {
        public bool CanRead(Type assetType, object parameter)
        {
            return assetType == typeof(string);
        }

        public async Task<object> ReadAsync(string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            if (assetType == typeof(string))
            {
                using (StreamReader sr = new StreamReader(assetStream))
                {
                    return await sr.ReadToEndAsync();
                }
            }
            else
            {
                return await Task.FromResult<string>(null);
            }
        }
    }
}
