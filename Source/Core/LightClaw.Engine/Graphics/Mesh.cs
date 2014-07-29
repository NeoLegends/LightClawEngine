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
    public class Mesh : Component
    {
        private static ILog logger = LogManager.GetLogger(typeof(Mesh));

        [DataMember]
        public string MeshFormat { get; private set; }

        [IgnoreDataMember]
        public MeshPart Part { get; private set; }

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

        public Mesh(MeshPart part)
        {
            Contract.Requires<ArgumentNullException>(part != null);

            logger.Debug("Initilizing a new mesh from a mesh part.");

            this.Part = part;
        }

        protected override void Dispose(bool disposing)
        {
            this.Part = null; // Prevent drawing after disposal
            base.Dispose(disposing);
        }

        protected override void OnDraw()
        {
            MeshPart part = this.Part;
            if (part != null)
            {
                part.Draw();
            }
            base.OnDraw();
        }

        protected override void OnLoad()
        {
            if (this.ResourceString != null && this.MeshFormat != null && this.Part == null)
            {
                logger.Info("Loading a mesh from '{0}' as '{1}'.".FormatWith(this.ResourceString, this.MeshFormat));
                Task<MeshPart> loaderTask = this.IocC.Resolve<IContentManager>()
                                                     .LoadAsync<MeshPart>(this.ResourceString, this.MeshFormat);

                loaderTask.ContinueWith(
                    t =>
                    {
                        this.Part = t.Result;
                        this.Part.Component = this;
                        logger.Info("Mesh '{0}' loaded successfully.");
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion
                );
                loaderTask.ContinueWith(
                    t => logger.Warn(
                        "Loading mesh '{0}' as '{1}' failed. At least one exception was thrown.".FormatWith(this.ResourceString, this.MeshFormat), 
                        t.Exception
                    ),
                    TaskContinuationOptions.OnlyOnFaulted
                );
            }
            base.OnLoad();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            MeshPart part = this.Part;
            if (part != null)
            {
                part.Update(gameTime);
            }
            base.OnUpdate(gameTime);
        }

        protected override void OnLateUpdate()
        {
            MeshPart part = this.Part;
            if (part != null)
            {
                part.LateUpdate();
            }
            base.OnLateUpdate();
        }
    }
}
