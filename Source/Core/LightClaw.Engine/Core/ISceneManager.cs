using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [ContractClass(typeof(ISceneManagerContracts))]
    public interface ISceneManager : IControllable, IEnumerable<Scene>
    {
        Task<bool> Load(int index, string resourceString);

        bool Load(int index, Scene s);

        void Move(int index, int newIndex);

        bool Unload(int index);
    }

    [ContractClassFor(typeof(ISceneManager))]
    abstract class ISceneManagerContracts : ISceneManager
    {
        event EventHandler<ParameterEventArgs> IControllable.Enabling { add { } remove { } }

        event EventHandler<ParameterEventArgs> IControllable.Enabled { add { } remove { } }

        event EventHandler<ParameterEventArgs> IControllable.Disabling { add { } remove { } }

        event EventHandler<ParameterEventArgs> IControllable.Disabled { add { } remove { } }

        event EventHandler<ParameterEventArgs> IControllable.Loading { add { } remove { } }

        event EventHandler<ParameterEventArgs> IControllable.Loaded { add { } remove { } }

        event EventHandler<ParameterEventArgs> IControllable.Resetting { add { } remove { } }

        event EventHandler<ParameterEventArgs> IControllable.Resetted { add { } remove { } }

        event EventHandler<ParameterEventArgs> Graphics.IDrawable.Drawing { add { } remove { } }

        event EventHandler<ParameterEventArgs> Graphics.IDrawable.Drawn { add { } remove { } }

        event EventHandler<ParameterEventArgs> IUpdateable.Updating { add { } remove { } }

        event EventHandler<ParameterEventArgs> IUpdateable.Updated { add { } remove { } }

        bool IControllable.IsEnabled
        {
            get
            {
                return false;
            }
        }

        bool IControllable.IsLoaded
        {
            get
            {
                return false;
            }
        }

        IEnumerator<Scene> IEnumerable<Scene>.GetEnumerator()
        {
            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return null;
        }

        Task<bool> ISceneManager.Load(int index, string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Ensures(Contract.Result<Task<bool>>() != null);

            return null;
        }

        bool ISceneManager.Load(int index, Scene s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            return false;
        }

        void ISceneManager.Move(int index, int newIndex)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(newIndex >= 0);
        }

        bool ISceneManager.Unload(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

            return false;
        }

        void IControllable.Enable()
        {
        }

        void IControllable.Disable()
        {
        }

        void IControllable.Load()
        {
        }

        void IControllable.Reset()
        {
        }

        void IDisposable.Dispose()
        {
        }

        void Graphics.IDrawable.Draw()
        {
        }

        void IUpdateable.Update(GameTime gameTime)
        {

        }
    }
}
