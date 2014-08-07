using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a system to manage currently loaded <see cref="Scene"/>s and draw them in the appropriate order.
    /// </summary>
    [ContractClass(typeof(ISceneManagerContracts))]
    public interface ISceneManager : IControllable, IEnumerable<Scene>, IDrawable
    {
        /// <summary>
        /// Gets the <see cref="Scene"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the <see cref="Scene"/> to get.</param>
        /// <returns>The <see cref="Scene"/> at the specified index.</returns>
        Scene this[int index] { get; }

        /// <summary>
        /// Asynchronously loads a <see cref="Scene"/> from the specified resource string into the specified position.
        /// </summary>
        /// <param name="index">The index to load the <see cref="Scene"/> into.</param>
        /// <param name="resourceString">The resource string of the <see cref="Scene"/> to load.</param>
        /// <returns><c>true</c> if the <see cref="Scene"/> could be inserted at the specified position, otherwise <c>false</c>.</returns>
        Task<bool> Load(int index, string resourceString);

        /// <summary>
        /// Loads the specified <see cref="Scene"/> into the specified position.
        /// </summary>
        /// <param name="index">The index to load the <see cref="Scene"/> into.</param>
        /// <param name="s">The <see cref="Scene"/> to load.</param>
        /// <returns><c>true</c> if the <see cref="Scene"/> could be inserted, otherwise <c>false</c>.</returns>
        bool Load(int index, Scene s);

        /// <summary>
        /// Moves the <see cref="Scene"/> from the specified index to the new index.
        /// </summary>
        /// <param name="index">The old index of the <see cref="Scene"/> to move.</param>
        /// <param name="newIndex">The index to move the <see cref="Scene"/> to.</param>
        /// <returns><c>true</c> if the <see cref="Scene"/> could be moved, otherwise <c>false</c>.</returns>
        bool Move(int index, int newIndex);

        /// <summary>
        /// Unloads the <see cref="Scene"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="Scene"/> to unload.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Scene"/> was unloaded, otherwise <c>false</c>. If the <see cref="Scene"/>
        /// could not be found, <c>false</c> will also be returned.
        /// </returns>
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

        event EventHandler<ParameterEventArgs> ILateUpdateable.LateUpdating { add { } remove { } }

        event EventHandler<ParameterEventArgs> ILateUpdateable.LateUpdated { add { } remove { } }

        Scene ISceneManager.this[int index]
        {
            get
            {
                Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

                return null;
            }
        }

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

        bool ISceneManager.Move(int index, int newIndex)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(newIndex >= 0);

            return false;
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

        void IUpdateable.Update(GameTime gameTime)
        {
        }

        void ILateUpdateable.LateUpdate()
        {
        }

        void IDisposable.Dispose()
        {
        }

        void Graphics.IDrawable.Draw()
        {
        }
    }
}
