using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public class ModelMeshCollection : ObservableCollection<ModelMesh>
    {
        public ModelMeshCollection() { }

        public ModelMeshCollection(IEnumerable<ModelMesh> modelMeshes)
            : base(modelMeshes) 
        {
            Contract.Requires<ArgumentNullException>(modelMeshes != null);

            foreach (ModelMesh modelMesh in modelMeshes)
            {
                this.Add(modelMesh);
            }
        }
    }
}
