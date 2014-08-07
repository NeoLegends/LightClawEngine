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
    /// <summary>
    /// Represents a <see cref="Component"/> rendering a <see cref="Model"/> to the screen.
    /// </summary>
    [DataContract]
    public class Mesh : Component
    {
        /// <summary>
        /// Notifies about changes in the currently rendered model.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Model>> ModelChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private Model _Model;

        /// <summary>
        /// The model to be drawn.
        /// </summary>
        [IgnoreDataMember]
        public Model Model
        {
            get
            {
                return _Model;
            }
            private set
            {
                Model previous = this.Model;
                this.SetProperty(ref _Model, value);
                this.Raise(this.ModelChanged, value, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private string _ResourceString;

        /// <summary>
        /// The resource string of the model to be drawn.
        /// </summary>
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
    
        /// <summary>
        /// Initializes a new <see cref="Mesh"/>.
        /// </summary>
        private Mesh() { }

        /// <summary>
        /// Initializes a new <see cref="Mesh"/> and sets the resource string of the model to load.
        /// </summary>
        /// <param name="resourceString">The resource string of the model to be drawn.</param>
        public Mesh(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            logger.Info(() => "Initializing a new mesh from model '{0}'.".FormatWith(resourceString));

            this.Name = resourceString;
            this.ResourceString = resourceString;
        }

        /// <summary>
        /// Draws the model to the screen.
        /// </summary>
        protected override void OnDraw()
        {
            Model model = this.Model;
            if (model != null)
            {
                model.Draw();
            }

            base.OnDraw();
        }

        /// <summary>
        /// Asynchronously loads the <see cref="Model"/> into the <see cref="Mesh"/>.
        /// </summary>
        protected override void OnLoad()
        {
            logger.Debug(() => "Loading mesh '{0}'.".FormatWith(this.Name ?? this.ResourceString));

            IContentManager contentManager = this.IocC.Resolve<IContentManager>();
            if (contentManager == null)
            {
                logger.Warn(() => "IContentManager could not be obtained, mesh '{0}' will not be loaded.".FormatWith(this.Name ?? this.ResourceString));
                return;
            }

            Task<Model> meshTask = (this.Model != null) ? Task.FromResult(this.Model) : contentManager.LoadAsync<Model>(this.ResourceString, true);
            meshTask.ContinueWith(t =>
            {
                this.Model = t.Result;
                logger.Debug(() => "Mesh '{0}' loaded successfully.");
            }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
            meshTask.ContinueWith(
                t => logger.Warn(() => "Mesh '{0}' could not be loaded, it will not be rendered.".FormatWith(this.Name ?? this.ResourceString), t.Exception),
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
            );

            base.OnLoad();
        }

        /// <summary>
        /// Updates the <see cref="Model"/>.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        protected override void OnUpdate(GameTime gameTime)
        {
            Model model = this.Model;
            if (model != null)
            {
                model.Update(gameTime);
            }

            base.OnUpdate(gameTime);
        }

        /// <summary>
        /// Late-updates the <see cref="Model"/>.
        /// </summary>
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
