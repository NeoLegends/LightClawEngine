using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public class ModelMesh : Entity, IDrawable, IUpdateable, ILateUpdateable
    {
        private IEnumerable<IGrouping<Material, ModelMeshPart>> groupedMeshParts;

        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        private Model _Model;

        public Model Model
        {
            get
            {
                return _Model;
            }
            internal set
            {
                this.SetProperty(ref _Model, value);
            }
        }

        private ModelMeshPartCollection _ModelMeshParts = new ModelMeshPartCollection();

        public ModelMeshPartCollection ModelMeshParts
        {
            get
            {
                return _ModelMeshParts;
            }
            private set
            {
                this.SetProperty(ref _ModelMeshParts, value);
                this.groupedMeshParts = null;
            }
        }

        public ModelMesh()
        {
            this.ModelMeshParts.CollectionChanged += (s, e) =>
            {
                this.groupedMeshParts = null; // Delete cache if collection changed, rebuild on .Update()
                foreach (ModelMeshPart modelMeshPart in e.OldItems)
                {
                    if (modelMeshPart != null)
                    {
                        modelMeshPart.ModelMesh = null;
                    }
                }
                foreach (ModelMeshPart modelMeshPart in e.NewItems)
                {
                    if (modelMeshPart != null)
                    {
                        modelMeshPart.ModelMesh = this;
                    }
                }
            };
        }

        public void Draw()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
            {
                IEnumerable<IGrouping<Material, ModelMeshPart>> groupedMeshParts = this.groupedMeshParts;
                if (groupedMeshParts != null)
                {
                    foreach (IGrouping<Material, ModelMeshPart> grouping in groupedMeshParts)
                    {
                        if (grouping.Key != null)
                        {
                            using (GLBinding materialBinding = new GLBinding(grouping.Key))
                            {
                                foreach (ModelMeshPart modelMeshPart in grouping)
                                {
                                    if (modelMeshPart != null)
                                    {
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
                foreach (ModelMeshPart part in this.ModelMeshParts)
                {
                    if (part != null)
                    {
                        part.Update(gameTime);
                    }
                }
                if (this.groupedMeshParts == null) // Rebuild grouping cache if it's null
                {
                    this.groupedMeshParts = this.ModelMeshParts.GroupBy(modelMeshPart => modelMeshPart.Material);
                }
            }
        }

        public void LateUpdate()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
            {
                foreach (ModelMeshPart part in this.ModelMeshParts)
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
