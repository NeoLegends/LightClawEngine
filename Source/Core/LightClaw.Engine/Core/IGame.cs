using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace LightClaw.Engine.Core
{
    [ContractClass(typeof(IGameContracts))]
    public interface IGame : IDisposable, INameable
    {
        IList<JoystickDevice> Joysticks { get; }

        KeyboardDevice Keyboard { get; }

        MouseDevice Mouse { get; }

        double TimeSinceLastUpdate { get;  }

        double TotalGameTime { get; }

        IEnumerable<Scene> Scenes { get; }

        Task LoadScene(int index, string resourceString);

        void Run();
    }

    [ContractClassFor(typeof(IGame))]
    abstract class IGameContracts : IGame
    {
        IList<JoystickDevice> IGame.Joysticks
        {
            get
            {
                return null;
            }
        }

        KeyboardDevice IGame.Keyboard
        {
            get
            {
                return null;
            }
        }

        MouseDevice IGame.Mouse
        {
            get
            {
                return null;
            }
        }

        string INameable.Name { get; set; }

        double IGame.TimeSinceLastUpdate
        {
            get
            {
                return 0;
            }
        }

        double IGame.TotalGameTime
        {
            get
            {
                return 0;
            }
        }

        IEnumerable<Scene> IGame.Scenes
        {
            get
            {
                return null;
            }
        }

        Task IGame.LoadScene(int index, string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Ensures(Contract.Result<Task>() != null);

            return null;
        }

        void IGame.Run()
        {
        }

        void IDisposable.Dispose()
        {
        }
    }
}
