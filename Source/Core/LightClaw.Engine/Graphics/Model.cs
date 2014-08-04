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
    public class Model : Entity, IDrawable, IUpdateable, ILateUpdateable
    {
        private IEnumerable<IGrouping<Shader, ModelPart>> groupedModelParts;

        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        private ModelPartCollection _ModelParts = new ModelPartCollection();

        public ModelPartCollection ModelParts
        {
            get
            {
                return _ModelParts;
            }
            private set
            {
                this.SetProperty(ref _ModelParts, value);
            }
        }

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

        public Model(IEnumerable<ModelPart> modelMeshes)
            : this()
        {
            Contract.Requires<ArgumentNullException>(modelMeshes != null);

            this.ModelParts.AddRange(modelMeshes);
        }

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
