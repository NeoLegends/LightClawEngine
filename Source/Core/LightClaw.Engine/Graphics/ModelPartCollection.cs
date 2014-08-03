using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public class ModelPartCollection : ObservableCollection<ModelPart>
    {
        public ModelPartCollection() { }

        public ModelPartCollection(IEnumerable<ModelPart> modelMeshes)
        {
            Contract.Requires<ArgumentNullException>(modelMeshes != null);

            foreach (ModelPart modelMeshPart in modelMeshes)
            {
                this.Add(modelMeshPart);
            }
        }
    }
}
