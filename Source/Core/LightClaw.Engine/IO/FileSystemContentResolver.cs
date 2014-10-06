﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
#if DESKTOP

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
        /// <seealso cref="AppDomain.BaseDirectory"></seealso>
        public FileSystemContentResolver()
            : this(AppDomain.CurrentDomain.BaseDirectory)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="FileSystemContentResolver"/> from the specified root path.
        /// </summary>
        /// <param name="rootPath">The path to take as root.</param>
        public FileSystemContentResolver(string rootPath)
        {
            Contract.Requires<ArgumentNullException>(rootPath != null);

            this.RootPath = rootPath;
            Logger.Debug(path => "Initialized a new {0}. Root path will be '{1}'.".FormatWith(typeof(FileSystemContentResolver).Name, path), rootPath);
        }

        /// <summary>
        /// Checks whether the asset with the specified resource string exists.
        /// </summary>
        /// <param name="resourceString">The resource string of the asset to check for.</param>
        /// <returns><c>true</c> if the asset exists, otherwise <c>false</c> .</returns>
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
                result = new FileStream(
                    Path.GetFullPath(Path.Combine(this.RootPath, resourceString)),
                    writable ? FileMode.OpenOrCreate : FileMode.Open,
                    writable ? FileAccess.ReadWrite : FileAccess.Read
                );
            }
            catch (DirectoryNotFoundException exception)
            {
                Logger.Warn(rs => "The directory containing asset '{0}' could not be found. Returning null.".FormatWith(rs), exception, resourceString);
            }
            catch (FileNotFoundException exception)
            {
                Logger.Warn(rs => "The file of asset '{0}' could not be found. Returning null.".FormatWith(rs), exception, resourceString);
            }
            catch (Exception exception)
            {
                Logger.Warn((ex, rs) => "An error of type '{0}' occured while obtaining the stream to asset '{1}'. Returning null.".FormatWith(ex.GetType().AssemblyQualifiedName, rs), exception, exception, resourceString);
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

#endif
}
