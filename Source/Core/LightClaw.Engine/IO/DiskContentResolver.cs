using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    public class DiskContentResolver : IContentResolver
    {
        public string RootPath { get; private set; }

        public DiskContentResolver(string rootPath)
        {
            Contract.Requires<ArgumentNullException>(rootPath != null);

            this.RootPath = rootPath;
        }

        public Task<bool> ExistsAsync(string resourceString)
        {
            return Task.FromResult(File.Exists(Path.Combine(this.RootPath, resourceString)));
        }

        public Task<System.IO.Stream> GetStreamAsync(string resourceString)
        {
            try
            {
                return Task.FromResult<Stream>(File.OpenRead(Path.Combine(this.RootPath, resourceString)));
            }
            catch
            {
                return Task.FromResult<Stream>(null);
            }
        }
    }
}
