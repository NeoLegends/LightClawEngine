using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using log4net;

namespace LightClaw.Engine.IO
{
    public class FileSystemContentResolver : IContentResolver
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FileSystemContentResolver));

        public string RootPath { get; private set; }

        public FileSystemContentResolver() : this(AppDomain.CurrentDomain.BaseDirectory) { }

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
            catch (Exception ex)
            {
                logger.Warn("An error of type '{0}' occured while obtaining the stream to an asset.".FormatWith(ex.GetType().AssemblyQualifiedName), ex);
                result = null;
            }
            return Task.FromResult(result);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.RootPath != null);
        }
    }
}
