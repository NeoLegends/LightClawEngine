using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    public class ModelReader : IContentReader
    {
        public bool CanRead(Type assetType, object parameter)
        {
            throw new NotImplementedException();
        }

        public Task<object> ReadAsync(ContentReadParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
