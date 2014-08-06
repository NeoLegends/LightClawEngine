using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a collection of <see cref="ModelPart"/>s.
    /// </summary>
    public class ModelPartCollection : ObservableCollection<ModelPart>
    {
        /// <summary>
        /// Initializes a new <see cref="ModelPartCollection"/>.
        /// </summary>
        public ModelPartCollection() { }

        /// <summary>
        /// Initializes a new <see cref="ModelPartCollection"/> from a set of <see cref="ModelPart"/>s.
        /// </summary>
        /// <param name="modelMeshes">The <see cref="ModelPart"/>s to start with.</param>
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
