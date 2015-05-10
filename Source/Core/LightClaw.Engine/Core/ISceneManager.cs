using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a system to manage currently loaded <see cref="Scene"/>s and draw them in the appropriate order.
    /// </summary>
    [ContractClass(typeof(ISceneManagerContracts))]
    public interface ISceneManager : IControllable, IEnumerable<Scene>, IDrawable
    {
        /// <summary>
        /// Gets the <see cref="Scene"/> at the specified <paramref name="slot"/>.
        /// </summary>
        /// <param name="slot">The index of the <see cref="Scene"/> to get.</param>
        /// <returns>The <see cref="Scene"/> at the specified index.</returns>
        Scene this[int slot] { get; }

        /// <summary>
        /// Loads the specified <see cref="Scene"/> into the specified position.
        /// </summary>
        /// <param name="slot">The index to load the <see cref="Scene"/> into.</param>
        /// <param name="s">The <see cref="Scene"/> to load.</param>
        /// <returns>The slot the scene was finally inserted into.</returns>
        int Load(int slot, Scene s);

        /// <summary>
        /// Asynchronously loads a <see cref="Scene"/> from the specified resource string into the specified position.
        /// </summary>
        /// <param name="slot">The index to load the <see cref="Scene"/> into.</param>
        /// <param name="resourceString">The resource string of the <see cref="Scene"/> to load.</param>
        /// <returns>The slot the scene was finally inserted into.</returns>
        Task<int> LoadAsync(int slot, ResourceString resourceString);

        /// <summary>
        /// Moves the <see cref="Scene"/> from the specified index to the new index.
        /// </summary>
        /// <param name="slot">The old index of the <see cref="Scene"/> to move.</param>
        /// <param name="newSlot">The index to move the <see cref="Scene"/> to.</param>
        /// <returns>
        /// The slot the <see cref="Scene"/> was moved to. If the move was impossible, the return value will be equal to
        /// <paramref name="slot"/>.
        /// </returns>
        int Move(int slot, int newSlot);

        /// <summary>
        /// Unloads the <see cref="Scene"/> at the specified index.
        /// </summary>
        /// <param name="slot">The index of the <see cref="Scene"/> to unload.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Scene"/> was unloaded, otherwise <c>false</c>. If the <see cref="Scene"/>
        /// could not be found, <c>false</c> will also be returned.
        /// </returns>
        bool Unload(int slot);
    }

    [ContractClassFor(typeof(ISceneManager))]
    internal abstract class ISceneManagerContracts : ISceneManager
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

        event EventHandler<UpdateEventArgs> IUpdateable.Updating { add { } remove { } }

        event EventHandler<UpdateEventArgs> IUpdateable.Updated { add { } remove { } }

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

        Task<int> ISceneManager.LoadAsync(int slot, ResourceString resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Requires<ArgumentOutOfRangeException>(slot >= 0);
            Contract.Ensures(Contract.Result<Task<int>>() != null);

            return null;
        }

        int ISceneManager.Load(int slot, Scene s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentOutOfRangeException>(slot >= 0);
            Contract.Ensures(Contract.Result<int>() >= 0);

            return 0;
        }

        int ISceneManager.Move(int slot, int newSlot)
        {
            Contract.Requires<ArgumentOutOfRangeException>(slot >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(newSlot >= 0);
            Contract.Ensures(Contract.Result<int>() >= 0);

            return 0;
        }

        bool ISceneManager.Unload(int slot)
        {
            Contract.Requires<ArgumentOutOfRangeException>(slot >= 0);

            return false;
        }

        void IControllable.Enable() { }

        void IControllable.Disable() { }

        void IControllable.Load() { }

        void IControllable.Reset() { }

        bool IUpdateable.Update(GameTime gameTime, int pass)
        {
            return false;
        }

        void IDisposable.Dispose() { }

        void Graphics.IDrawable.Draw() { }
    }
}
