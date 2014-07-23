using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public class MeshPartCollection : List<MeshPart>, IDrawable
    {
        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public MeshPartCollection() { }

        public MeshPartCollection(int capacity)
            : base(capacity) 
        {
            Contract.Requires<ArgumentOutOfRangeException>(capacity >= 0);
        }

        public MeshPartCollection(IEnumerable<MeshPart> parts)
            : base(parts)
        {
            Contract.Requires<ArgumentNullException>(parts != null);
        }

        public void Draw()
        {
            this.Raise(this.Drawing);
            foreach (MeshPart meshPart in this)
            {
                meshPart.Draw();
            }
            this.Raise(this.Drawn);
        }

        private void Raise(EventHandler<ParameterEventArgs> handler, ParameterEventArgs args = null)
        {
            if (handler != null)
            {
                handler(this, args ?? new ParameterEventArgs());
            }
        }
    }
}
