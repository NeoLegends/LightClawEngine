using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;

namespace LightClaw.Engine.Core
{
    public class Scene : ListChildManager<GameObject>, IDrawable
    {
        public void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
