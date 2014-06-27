using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract(IgnoreListHandling = true)]
    public class Scene : ListChildManager<GameObject>, IDrawable
    {
        public void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
