using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public class ModelMeshPartCollection : ObservableCollection<ModelMeshPart>
    {
        public ModelMeshPartCollection() { }

        public ModelMeshPartCollection(IEnumerable<ModelMeshPart> modelMeshes)
        {
            Contract.Requires<ArgumentNullException>(modelMeshes != null);

            foreach (ModelMeshPart modelMeshPart in modelMeshes)
            {
                this.Add(modelMeshPart);
            }
        }
    }
}
