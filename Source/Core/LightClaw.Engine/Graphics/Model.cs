using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public class Model : Entity, IDrawable, IUpdateable, ILateUpdateable
    {
        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        private ModelMeshCollection _ModelMeshes = new ModelMeshCollection();

        public ModelMeshCollection ModelMeshes
        {
            get
            {
                return _ModelMeshes;
            }
            private set
            {
                this.SetProperty(ref _ModelMeshes, value);
            }
        }

        public Model()
        {
            this.ModelMeshes.CollectionChanged += (s, e) =>
            {
                foreach (ModelMesh modelMesh in e.OldItems)
                {
                    if (modelMesh != null)
                    {
                        modelMesh.Model = null;
                    }
                }
                foreach (ModelMesh modelMesh in e.NewItems)
                {
                    if (modelMesh != null)
                    {
                        modelMesh.Model = this;
                    }
                }
            };
        }

        public void Draw()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
            {
                foreach (ModelMesh modelMesh in this.ModelMeshes)
                {
                    if (modelMesh != null)
                    {
                        modelMesh.Draw();
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
            {
                foreach (ModelMesh modelMesh in this.ModelMeshes)
                {
                    if (modelMesh != null)
                    {
                        modelMesh.Update(gameTime);
                    }
                }
            }
        }

        public void LateUpdate()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
            {
                foreach (ModelMesh modelMesh in this.ModelMeshes)
                {
                    if (modelMesh != null)
                    {
                        modelMesh.LateUpdate();
                    }
                }
            }
        }
    }
}
