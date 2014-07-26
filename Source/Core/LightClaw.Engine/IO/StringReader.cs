using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    public class StringReader : IContentReader
    {
        public Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            if (assetType == typeof(string))
            {
                using (StreamReader sr = new StreamReader(assetStream))
                {
                    return sr.ReadToEndAsync().ContinueWith(s => (object)s, TaskContinuationOptions.ExecuteSynchronously);
                }
            }
            else
            {
                return Task.FromResult((object)null);
            }
        }
    }
}
