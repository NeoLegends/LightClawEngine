using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    public interface ISceneManager : IControllable, IEnumerable<Scene>
    {
        Task<bool> Load(int index, string resourceString);

        bool Load(int index, Scene s);

        bool Unload(int index);
    }
}
