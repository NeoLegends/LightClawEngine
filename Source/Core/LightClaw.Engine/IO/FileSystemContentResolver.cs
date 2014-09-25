using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using log4net;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents a <see cref="IContentResolver"/> resolving assets from the file system / disk.
    /// </summary>
    public class FileSystemContentResolver : Entity, IContentResolver
    {
        /// <summary>
        /// The root path to prepend to the relative resource strings.
        /// </summary>
        public string RootPath { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="FileSystemContentResolver"/> using the application's base directory as root path.
        /// </summary>
        /// <seealso cref="AppDomain.BaseDirectory"/>
        public FileSystemContentResolver() : this(AppDomain.CurrentDomain.BaseDirectory) { }

        /// <summary>
        /// Initializes a new <see cref="FileSystemContentResolver"/> from the specified root path.
        /// </summary>
        /// <param name="rootPath">The path to take as root.</param>
        public FileSystemContentResolver(string rootPath)
        {
            Contract.Requires<ArgumentNullException>(rootPath != null);

            this.RootPath = rootPath;
            Logger.Debug(path => "Initialized a new {0}. Root path will be {1}.".FormatWith(typeof(FileSystemContentResolver).Name, path), rootPath);
        }

        /// <summary>
        /// Checks whether the asset with the specified resource string exists.
        /// </summary>
        /// <param name="resourceString">The resource string of the asset to check for.</param>
        /// <returns><c>true</c> if the asset exists, otherwise <c>false</c>.</returns>
        public Task<bool> ExistsAsync(ResourceString resourceString)
        {
            return Task.FromResult(File.Exists(Path.Combine(this.RootPath, resourceString)));
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> around the specified asset.
        /// </summary>
        /// <param name="resourceString">The resource string of the asset to obtain a <see cref="Stream"/> around.</param>
        /// <param name="writable">Indicates whether the <see cref="Stream"/> needs to be writable.</param>
        /// <returns>The <see cref="Stream"/> around the asset or <c>null</c> if the asset could not be found.</returns>
        public Task<Stream> GetStreamAsync(ResourceString resourceString, bool writable)
        {
            Stream result = null;
            try
            {
                result = File.Open(Path.Combine(this.RootPath, resourceString), FileMode.OpenOrCreate, writable ? FileAccess.ReadWrite : FileAccess.Read);
            }
            catch (Exception exception)
            {
                Logger.Warn(ex => "An error of type '{0}' occured while obtaining the stream to an asset. Returning null.".FormatWith(ex.GetType().AssemblyQualifiedName), exception, exception);
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.RootPath != null);
        }
    }
}
