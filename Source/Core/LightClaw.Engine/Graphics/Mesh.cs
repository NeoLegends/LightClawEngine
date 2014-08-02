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
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public class Mesh<TVertex> : Component
        where TVertex : struct
    {
        private static ILog logger = LogManager.GetLogger(typeof(Mesh<TVertex>));

        private Model _Model;

        public Model Model
        {
            get
            {
                return _Model;
            }
            private set
            {
                this.SetProperty(ref _Model, value);
            }
        }

        private string _ResourceString;

        [DataMember]
        public string ResourceString
        {
            get
            {
                return _ResourceString;
            }
            private set
            {
                this.SetProperty(ref _ResourceString, value);
            }
        }

        private Mesh() { }

        public Mesh(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            logger.Info("Initializing a new mesh from model '{0}'.".FormatWith(resourceString));

            this.Name = resourceString;
            this.ResourceString = resourceString;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnDraw()
        {
            Model model = this.Model;
            if (model != null)
            {
                model.Draw();
            }

            base.OnDraw();
        }

        protected override void OnLoad()
        {
            logger.Debug("Loading mesh '{0}'.".FormatWith(this.Name ?? this.ResourceString));

            IContentManager contentManager = this.IocC.Resolve<IContentManager>();
            if (contentManager == null)
            {
                logger.Warn("IContentManager could not be obtained, mesh '{0}' will not be loaded.".FormatWith(this.Name ?? this.ResourceString));
                return;
            }

            Task<Model> meshDescriptionTask = (this.Model != null) ? Task.FromResult(this.Model) :
                                                                     contentManager.LoadAsync<Model>(this.ResourceString);
            meshDescriptionTask.ContinueWith(t =>
            {
                this.Model = t.Result;
                logger.Debug("Mesh '{0}' loaded successfully.");
            }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
            meshDescriptionTask.ContinueWith(
                t => logger.Warn("Mesh '{0}' could not be loaded, it will not be rendered.".FormatWith(this.Name ?? this.ResourceString), t.Exception),
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
            );

            base.OnLoad();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            Model model = this.Model;
            if (model != null)
            {
                model.Update(gameTime);
            }

            base.OnUpdate(gameTime);
        }

        protected override void OnLateUpdate()
        {
            Model model = this.Model;
            if (model != null)
            {
                model.LateUpdate();
            }

            base.OnLateUpdate();
        }
    }
}
