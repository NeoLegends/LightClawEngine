using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using LightClaw.Engine.Common;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;

namespace LightClaw.AssetPipeline.AssetProcessing
{
    /// <summary>
    /// Class taking care of calling the asset processing modules
    /// </summary>
    public class AssetProcessor
    {
        /// <summary>
        /// All read <see cref="LightClaw.AssetPipeline.AssetProcessing.IAssetProcessable"/>-Instances from the DLLs.
        /// </summary>
        private List<IAssetProcessable> instances = new List<IAssetProcessable>();

        /// <summary>
        /// The where the <see cref="LightClaw.AssetPipeline.AssetProcessing.AssetProcessor"/> loads the DLLs from.
        /// </summary>
        private String dllPath = null;

        /// <summary>
        /// Creates a new instance of the class <see cref="LightClaw.AssetPipeline.AssetProcessing.AssetProcessor"/>.
        /// </summary>
        /// <param name="dllPath">The folder containing all processing libraries.</param>
        public AssetProcessor(IServiceRegistry registry, String dllPath)
        {
            Contract.Requires(registry != null);

            this.dllPath = dllPath ?? Path.Combine(registry.GetService<IFolders>().Bin, "AssetPipeline");
        }

        /// <summary>
        /// Called on start of processing a file firing all loaded class instances as well.
        /// </summary>
        /// <param name="file">The path to the file being exported.</param>
        /// <param name="refreshInstancesBeforeProcessing">
        /// Whether all available asset processing class instances shall be refreshed before firing the methods.
        /// </param>
        public void Process(String file, bool refreshInstancesBeforeProcessing = true)
        {
            Contract.Requires(file != null);

            if (refreshInstancesBeforeProcessing)
            {
                this.RefreshInstances();
            }

            foreach (IAssetProcessable instance in instances)
            {
                if (instance.SupportedFileExtensions.Any(supportedType => supportedType == Path.GetExtension(file)))
                {
                    instance.OnProcess(file);
                }
            }
        }

        /// <summary>
        /// Processes a couple of files at the same time.
        /// </summary>
        /// <param name="files">The files to process.</param>
        public void Process(IEnumerable<String> files)
        {
            Contract.Requires(files != null);

            this.RefreshInstances();
            foreach (String file in files)
            {
                this.Process(file, false);
            }
        }

        /// <summary>
        /// Clears the list of available processing class instances and reloads them from the DLLs.
        /// </summary>
        private void RefreshInstances()
        {
            this.instances.Clear();

            foreach (String asm in Directory.GetFiles(dllPath, "*.*", SearchOption.AllDirectories)
                                            .Where(file => file.ToLower().EndsWith("dll") || file.ToLower().EndsWith("exe")))
            {
                foreach (Type t in Assembly.LoadFrom(asm).GetTypesByBase<IAssetProcessable>())
                {
                    try { this.instances.Add((IAssetProcessable)Activator.CreateInstance(t)); }
                    catch { }
                }
            }
        }
    }
}
