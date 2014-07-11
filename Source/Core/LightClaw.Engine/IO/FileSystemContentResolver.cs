using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.IO
{
    public class FileSystemContentResolver : IContentResolver
    {
        public string RootPath { get; private set; }

        public FileSystemContentResolver(string rootPath)
        {
            Contract.Requires<ArgumentNullException>(rootPath != null);

            this.RootPath = rootPath;
        }

        public Task<bool> ExistsAsync(string resourceString)
        {
            return Task.FromResult(File.Exists(Path.Combine(this.RootPath, resourceString)));
        }

        public Task<Stream> GetStreamAsync(string resourceString)
        {
            Stream result;
            try
            {
                result = File.OpenRead(Path.Combine(this.RootPath, resourceString));
            }
            catch
            {
                result = null;
            }
            return Task.FromResult(result);
        }
    }
}
