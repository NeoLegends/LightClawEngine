using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a three-dimensional polygon model.
    /// </summary>
    public class Model : Entity, IDrawable, IUpdateable, ILateUpdateable
    {
        /// <summary>
        /// A cache of the <see cref="ModelPart"/>s grouped by their shader.
        /// </summary>
        private IEnumerable<IGrouping<Shader, ModelPart>> groupedModelParts;

        /// <summary>
        /// Notifies about the start of the drawing process.
        /// </summary>
        /// <remarks>Raised before any binding / drawing occurs.</remarks>
        public event EventHandler<ParameterEventArgs> Drawing;

        /// <summary>
        /// Notifies about the finish of the drawing process.
        /// </summary>
        /// <remarks>Raised after any binding / drawing operations.</remarks>
        public event EventHandler<ParameterEventArgs> Drawn;

        /// <summary>
        /// Notifies about a change in the <see cref="P:Mesh"/>.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Mesh>> MeshChanged;

        /// <summary>
        /// Notifies about the start of the updating process.
        /// </summary>
        /// <remarks>Raised before any updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> Updating;

        /// <summary>
        /// Notifies about the finsih of the updating process.
        /// </summary>
        /// <remarks>Raised after any updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> Updated;

        /// <summary>
        /// Notifies about the start of the late updating process.
        /// </summary>
        /// <remarks>Raised before any late updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> LateUpdating;

        /// <summary>
        /// Notifies about the finsih of the late updating process.
        /// </summary>
        /// <remarks>Raised after any late updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> LateUpdated;

        /// <summary>
        /// Backing field.
        /// </summary>
        private Mesh _Mesh;

        /// <summary>
        /// The mesh the <see cref="Model"/> currently is attached to.
        /// </summary>
        public Mesh Mesh
        {
            get
            {
                return _Mesh;
            }
            internal set
            {
                Mesh previous = this.Mesh;
                this.SetProperty(ref _Mesh, value);
                this.Raise(this.MeshChanged, value, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private ModelPartCollection _ModelParts = new ModelPartCollection();

        /// <summary>
        /// A collection of <see cref="ModelPart"/>s this <see cref="Model"/> consists of.
        /// </summary>
        public ModelPartCollection ModelParts
        {
            get
            {
                return _ModelParts;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _ModelParts, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Model"/>.
        /// </summary>
        public Model()
        {
            this.ModelParts.CollectionChanged += (s, e) =>
            {
                this.groupedModelParts = null;
                foreach (ModelPart modelMesh in e.OldItems)
                {
                    if (modelMesh != null)
                    {
                        modelMesh.Model = null;
                    }
                }
                foreach (ModelPart modelMesh in e.NewItems)
                {
                    if (modelMesh != null)
                    {
                        modelMesh.Model = this;
                    }
                }
            };
        }

        /// <summary>
        /// Initializes a new <see cref="Model"/> from a range of <see cref="ModelPart"/>s.
        /// </summary>
        /// <param name="modelMeshes"></param>
        public Model(IEnumerable<ModelPart> modelMeshes)
            : this()
        {
            Contract.Requires<ArgumentNullException>(modelMeshes != null);

            this.ModelParts.AddRange(modelMeshes);
        }

        /// <summary>
        /// Draws the model to the screen.
        /// </summary>
        public void Draw()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
            {
                IEnumerable<IGrouping<Shader, ModelPart>> groupedModelParts = this.groupedModelParts;
                if (groupedModelParts != null)
                {
                    foreach (IGrouping<Shader, ModelPart> grouping in groupedModelParts)
                    {
                        if (grouping.Key != null)
                        {
                            using (GLBinding shaderBinding = new GLBinding(grouping.Key))
                            {
                                foreach (ModelPart modelMeshPart in grouping)
                                {
                                    if (modelMeshPart != null)
                                    {
                                        modelMeshPart.Material.Bind();
                                        modelMeshPart.Draw();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        public void Update(GameTime gameTime)
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
            {
                foreach (ModelPart part in this.ModelParts)
                {
                    if (part != null)
                    {
                        part.Update(gameTime);
                    }
                }
                if (this.groupedModelParts == null) // Rebuild grouping cache if it's null
                {
                    this.groupedModelParts = this.ModelParts.GroupBy(modelMeshPart => modelMeshPart.Material.Shader);
                }
            }
        }

        /// <summary>
        /// Late-updates the <see cref="Model"/>.
        /// </summary>
        public void LateUpdate()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
            {
                foreach (ModelPart part in this.ModelParts)
                {
                    if (part != null)
                    {
                        part.LateUpdate();
                    }
                }
            }
        }
    }
}
