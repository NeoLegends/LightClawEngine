using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using OpenTK;

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
        /// Notifies about changes in the source.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<ResourceString>> SourceChanged;

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
        private ResourceString _Source;

        /// <summary>
        /// The resource string of the model to be drawn.
        /// </summary>
        [DataMember]
        public ResourceString Source
        {
            get
            {
                return _Source;
            }
            private set
            {
                ResourceString previous = this.Source;
                this.SetProperty(ref _Source, value);
                this.Raise(this.SourceChanged, value, previous);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Mesh"/>. Constructor required for serialization.
        /// </summary>
        private Mesh() { }

        /// <summary>
        /// Initializes a new <see cref="Mesh"/> and sets the resource string of the model to load.
        /// </summary>
        /// <param name="resourceString">The resource string of the model to be drawn.</param>
        public Mesh(ResourceString resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            Log.Info(() => "Initializing a new mesh from model '{0}'.".FormatWith(resourceString));

            this.Name = resourceString;
            this.Source = resourceString;
        }

        /// <summary>
        /// Initializes a new <see cref="Mesh"/>.
        /// </summary>
        /// <param name="model">The model to be drawn.</param>
        public Mesh(Model model)
        {
            Contract.Requires<ArgumentNullException>(model != null);

            Log.Info(() => "Initializing a new mesh from model '{0}'.".FormatWith(model.Name));

            this.Name = model.Name;
            this.Model = model;
        }

        /// <summary>
        /// Draws the model to the screen.
        /// </summary>
        protected override void OnDraw()
        {
            Model model = this.Model;
            if (model != null)
            {
                Matrix4 modelMatrix = this.GameObject.Transform.ModelMatrix;
                model.Draw(ref modelMatrix);
            }
        }

        /// <summary>
        /// Asynchronously loads the <see cref="Model"/> into the <see cref="Mesh"/>.
        /// </summary>
        /// <remarks>Drawing and updating will be performed when the mesh is loaded successfully.</remarks>
        protected override async void OnLoad()
        {
            Log.Debug(() => "Loading mesh '{0}'.".FormatWith(this.Name ?? this.Source));

            ResourceString rs = this.Source;
            if ((this.Model == null) && (rs != null))
            {
                try
                {
                    this.Model = await this.IocC.Resolve<IContentManager>()
                                                .LoadAsync<Model>(rs)
                                                .ConfigureAwait(false);
                    this.Name = this.Model.Name;
                    Log.Debug(() => "Mesh '{0}' loaded successfully.");
                }
                catch (Exception ex)
                {
                    Log.Error(
                        "Mesh '{0}' could not be loaded, an exception of type {1} occured. it will not be rendered.".FormatWith(
                            ex.GetType().FullName,
                            this.Name ?? this.Source
                        ), 
                        ex
                    );
                }
            }
        }
    }
}
