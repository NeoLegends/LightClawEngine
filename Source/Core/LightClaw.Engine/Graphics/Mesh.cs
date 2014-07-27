using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public class Mesh : Component, IEnumerable<MeshPart>
    {
        private static ILog logger = LogManager.GetLogger(typeof(Mesh));

        [DataMember]
        public string MeshFormat { get; private set; }

        [IgnoreDataMember]
        public MeshPartCollection Parts { get; private set; }

        [DataMember]
        public string ResourceString { get; private set; }

        private Mesh() { }

        public Mesh(string resourceString, string meshFormat)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(meshFormat));

            logger.Debug("Initializing a new mesh from resource string '{0}' and format '{1}'.".FormatWith(resourceString, meshFormat));

            this.MeshFormat = meshFormat;
            this.ResourceString = resourceString;
        }

        public Mesh(MeshPartCollection parts)
        {
            Contract.Requires<ArgumentNullException>(parts != null);

            logger.Debug("Initilizing a new mesh from a part collection consisting of {0} MeshParts.".FormatWith(parts.Count));

            this.Parts = parts;
        }

        public IEnumerator<MeshPart> GetEnumerator()
        {
            return this.Parts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected override void Dispose(bool disposing)
        {
            MeshPartCollection meshParts = this.Parts;
            if (meshParts != null)
            {
                meshParts.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnDraw()
        {
            MeshPartCollection parts = this.Parts;
            if (parts != null)
            {
                parts.Draw();
            }
        }

        protected override void OnLoad()
        {
            if (this.ResourceString != null && this.MeshFormat != null && this.Parts == null)
            {
                logger.Info("Loading a mesh from '{0}' as '{1}'.".FormatWith(this.ResourceString, this.MeshFormat));
                Task<MeshPartCollection> loaderTask = this.IocC.Resolve<IContentManager>()
                                                               .LoadAsync<MeshPartCollection>(this.ResourceString, this.MeshFormat);

                loaderTask.ContinueWith(
                    t =>
                    {
                        this.Parts = t.Result;
                        logger.Info("Mesh '{0}' loaded successfully.");
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion
                );
                loaderTask.ContinueWith(
                    t => 
                    {
                        logger.Warn("Loading mesh '{0}' as '{1}' failed. At least one exception was thrown. Writing them sequentially into the log.".FormatWith(this.ResourceString, this.MeshFormat));
                        foreach (Exception ex in t.Exception.InnerExceptions)
                        {
                            logger.Warn("An exception of type '{0}' was thrown while loading mesh '{1}' as '{2}'.".FormatWith(ex.GetType().AssemblyQualifiedName, this.ResourceString, this.MeshFormat), ex);
                        }
                    }, 
                    TaskContinuationOptions.OnlyOnFaulted
                );
            }
        }
    }
}
