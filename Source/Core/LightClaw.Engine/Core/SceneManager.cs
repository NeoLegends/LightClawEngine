using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents the central interface for managing scene lifecycles and final image composition.
    /// </summary>
    /// <remarks>
    /// <see cref="SceneManager"/> <u>will</u> assume ownage of the <see cref="Scene"/> it controls
    /// and thus also disposes them if <see cref="SceneManager"/> itself is disposed.
    /// 
    /// Image composition works by rendering all managed scenes on top of each other. Scenes with higher slots are
    /// drawn later and thus are more visible.
    /// </remarks>
    internal class SceneManager : Manager, ISceneManager
    {
        /// <summary>
        /// The underlying list of managed <see cref="Scene"/>s.
        /// </summary>
        private readonly SortedDictionary<int, Scene> scenes = new SortedDictionary<int, Scene>();

        /// <summary>
        /// A working copy of the scenes to work with.
        /// </summary>
        private readonly List<Scene> workingCopy = new List<Scene>();

        /// <summary>
        /// Gets the <see cref="Scene"/> at the specified <paramref name="slot"/>
        /// </summary>
        /// <param name="slot">The slot of the <see cref="Scene"/> to retreive.</param>
        /// <returns>The <see cref="Scene"/> at the specified slot.</returns>
        public Scene this[int slot]
        {
            get
            {
                lock (this.scenes)
                {
                    return this.scenes[slot];
                }
            }
        }

        /// <summary>
        /// Initializes a new <see cref="SceneManager"/> from a resource string.
        /// </summary>
        /// <param name="startScene">The <see cref="Scene"/> to load on startup.</param>
        /// <remarks>
        /// As the startup scene will be loaded synchronously, it is crucial to engine performance that the <see cref="Scene"/> is
        /// small and can be loaded quickly.
        /// </remarks>
        public SceneManager(string startScene)
        {
            Contract.Requires<ArgumentNullException>(startScene != null);

            logger.Info(() => "Initializing scene manager from resource string '{0}'.".FormatWith(startScene));
            this.Load(0, startScene).Wait();
        }

        /// <summary>
        /// Initializes a new <see cref="SceneManager"/> from an already loaded <see cref="Scene"/>.
        /// </summary>
        /// <param name="startScene">The <see cref="Scene"/> to use as startup <see cref="Scene"/>.</param>
        public SceneManager(Scene startScene)
        {
            Contract.Requires<ArgumentNullException>(startScene != null);

            logger.Info(() => "Initializing scene manager from scene '{0}'".FormatWith(startScene.Name ?? "N/A"));
            this.Load(0, startScene);
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator{T}"/>.
        /// </summary>
        /// <returns>The <see cref="IEnumerator{T}"/>.</returns>
        public IEnumerator<Scene> GetEnumerator()
        {
            Scene[] scenes;
            lock (this.scenes)
            {
                scenes = this.scenes.Values.ToArray();
            }
            return (IEnumerator<Scene>)scenes.GetEnumerator();
        }

        /// <summary>
        /// Gets the <see cref="System.Collections.IEnumerator"/>.
        /// </summary>
        /// <returns>The <see cref="System.Collections.IEnumerator"/>.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Asynchronously loads a <see cref="Scene"/> from the specified <paramref name="resourceString"/>.
        /// </summary>
        /// <param name="slot">The slot to load the <see cref="Scene"/> into.</param>
        /// <param name="resourceString">The resource string of the <see cref="Scene"/> to load.</param>
        /// <returns>The slot the <see cref="Scene"/> was inserted into in the end.</returns>
        /// <remarks>
        /// If the desired slot is already taken, the method tries to load the scene in the slot below. This is done until a free
        /// slot is found. However, moving the scene into a lower layer during rendering (final image is a composition of all scenes drawing 
        /// on top of each other) poses a higher risk of being overdrawn by scenes that are not supposed to overdraw it.
        /// </remarks>
        /// <exception cref="InvalidOperationException">All slots below taken, scene could not be laoded.</exception>
        public async Task<int> Load(int slot, string resourceString)
        {
            return this.Load(slot, await IocC.Resolve<IContentManager>().LoadAsync<Scene>(resourceString));
        }

        /// <summary>
        /// Asynchronously loads a <see cref="Scene"/> into the <see cref="SceneManager"/>.
        /// </summary>
        /// <param name="slot">The slot to load the <see cref="Scene"/> into.</param>
        /// <param name="s">The existing <see cref="Scene"/> to load.</param>
        /// <returns>The slot the <see cref="Scene"/> was inserted into in the end.</returns>
        /// <remarks>
        /// If the desired slot is already taken, the method tries to load the scene in the slot below. This is done until a free
        /// slot is found. However, moving the scene into a lower layer during rendering (final image is a composition of all scenes drawing 
        /// on top of each other) poses a higher risk of being overdrawn by scenes that are not supposed to overdraw it.
        /// </remarks>
        /// <exception cref="InvalidOperationException">All slots below taken, scene could not be loaded.</exception>
        public int Load(int slot, Scene s)
        {
            logger.Info(() => "Loading a new scene into position {0}.".FormatWith(slot));

            if (this.IsLoaded && !s.IsLoaded)
            {
                s.Load();
            }
            lock (this.scenes)
            {
                for (int i = slot; i < int.MinValue; i--)
                {
                    try
                    {
                        logger.Debug(() => "Trying to insert scene '{0}' into position {1}.".FormatWith(s.Name ?? "N/A", i));
                        this.scenes.Add(i, s);
                        logger.Debug(() => "Scene inserted successfully.");
                        return i;
                    }
                    catch (ArgumentException)
                    {
                        logger.Debug(() => "Position {0} taken, incrementing...".FormatWith(i));
                    }
                }

                // Very unlikely to happen, except for sb wanting to have their scene at int.MinValue + 1 and its already taken ;)
                logger.Warn(() => "Scene insertion failed, all slots below were taken.");
                throw new InvalidOperationException("Scene insertion failed, all slots below were taken.");
            }
        }

        /// <summary>
        /// Tries to move a <see cref="Scene"/> from one slot to another.
        /// </summary>
        /// <param name="slot">The slot of the old scene.</param>
        /// <param name="newSlot">The new slot.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Scene"/> was moved, otherwise <c>false</c>. <c>false</c> will also be returned if there
        /// was no <see cref="Scene"/> at <paramref name="slot"/>.
        /// </returns>
        /// <remarks>
        /// If the desired slot is already taken, the method tries to load the scene in the slot below. This is done until a free
        /// slot is found. However, moving the scene into a lower layer during rendering (final image is a composition of all scenes drawing 
        /// on top of each other) poses a higher risk of being overdrawn by scenes that are not supposed to overdraw it.
        /// </remarks>
        /// <exception cref="InvalidOperationException">All slots below taken, scene could not be moved.</exception>
        public void Move(int slot, int newSlot)
        {
            logger.Debug(() => "Moving a scene from {0} to position {1}.".FormatWith(slot, newSlot));

            lock (this.scenes)
            {
                Scene scene;
                if (this.scenes.TryGetValue(slot, out scene) && this.scenes.Remove(slot))
                {
                    for (int i = newSlot; i < int.MinValue; i--)
                    {
                        try
                        {
                            logger.Debug(() => "Trying to move scene to {0}.".FormatWith(i));
                            this.scenes.Add(i, scene);
                            logger.Debug(() => "Scene moved.");
                            return;
                        }
                        catch (ArgumentException)
                        {
                            logger.Debug(() => "Position {0} taken, incrementing...".FormatWith(i));
                        }
                    }

                    // Very unlikely to happen, except for sb wanting to have their scene at int.MinValue + 1 and its already taken ;)
                    logger.Warn(() => "All slots below taken, scene could not be moved. Reinserting into old position.");
                    this.scenes[slot] = scene;
                    throw new InvalidOperationException("All slots below taken, scene could not be moved. Reinserting into old position.");
                }
            }
        }

        /// <summary>
        /// Unloads the <see cref="Scene"/> from the specified slot.
        /// </summary>
        /// <param name="slot">The slot of the <see cref="Scene"/> to unload.</param>
        /// <returns><c>true</c> if a <see cref="Scene"/> was unloaded, otherwise <c>false</c>.</returns>
        public bool Unload(int slot)
        {
            logger.Debug(() => "Unloading scene from position {0}.".FormatWith(slot));

            Scene s = null;
            bool result;
            lock (this.scenes)
            {
                result = this.scenes.TryGetValue(slot, out s) ? this.scenes.Remove(slot) : false;
            }
            if (s != null)
            {
                logger.Debug(() => "Disposing scene...");
                s.Dispose();
            }

            logger.Debug(() => result ? "Scene unloaded from position {0}.".FormatWith(slot) : "Scene could not be removed.");
            return result;
        }

        /// <summary>
        /// Implementation of <see cref="M:Enable"/>.
        /// </summary>
        protected override void OnEnable() 
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Enable();
            }
            this.workingCopy.Clear();
        }

        /// <summary>
        /// Implementation of <see cref="M:Disable"/>.
        /// </summary>
        protected override void OnDisable()
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Disable();
            }
            this.workingCopy.Clear();
        }

        /// <summary>
        /// Implementation of <see cref="M:Draw"/>.
        /// </summary>
        protected override void OnDraw()
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Draw();
            }
            this.workingCopy.Clear();
        }

        /// <summary>
        /// Implementation of <see cref="M:Load"/>.
        /// </summary>
        protected override void OnLoad()
        {
            logger.Info(() => "Loading scene manager.");

            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Load();
            }
            this.workingCopy.Clear();

            logger.Info(() => "Scene manager loaded.");
        }

        /// <summary>
        /// Implementation of <see cref="M:Reset"/>.
        /// </summary>
        protected override void OnReset()
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Reset();
            }
            this.workingCopy.Clear();
        }

        /// <summary>
        /// Implementation of <see cref="M:Update"/>.
        /// </summary>
        protected override void OnUpdate(GameTime gameTime)
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.Update(gameTime);
            }
            this.workingCopy.Clear();
        }

        /// <summary>
        /// Implementation of <see cref="M:LateUpdate"/>.
        /// </summary>
        protected override void OnLateUpdate()
        {
            lock (this.scenes)
            {
                this.workingCopy.AddRange(this.scenes.Values);
            }
            foreach (Scene s in this.workingCopy)
            {
                s.LateUpdate();
            }
            this.workingCopy.Clear();
        }
    }
}
