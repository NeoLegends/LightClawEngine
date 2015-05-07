using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a three-dimensional polygon model.
    /// </summary>
    public class Model : Entity, IDrawable, IEnumerable<ModelPart>, IUpdateable
    {
        /// <summary>
        /// Notifies about a change in the <see cref="P:Component"/>.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Component>> ComponentChanged;

        /// <summary>
        /// Notifies about the start of the drawing process.
        /// </summary>
        /// <remarks>Raised before any binding / drawing occurs.</remarks>
        public event EventHandler<ParameterEventArgs> Drawing;

        /// <summary>
        /// Notifies about the finish of the drawing process.
        /// </summary>
        /// <remarks>Raised after any binding / drawing operations.</remarks>
        public event EventHandler<ParameterEventArgs> Drawn;

        /// <summary>
        /// Notifies about the start of the updating process.
        /// </summary>
        /// <remarks>Raised before any updating operations.</remarks>
        public event EventHandler<UpdateEventArgs> Updating;

        /// <summary>
        /// Notifies about the finish of the updating process.
        /// </summary>
        /// <remarks>Raised after any updating operations.</remarks>
        public event EventHandler<UpdateEventArgs> Updated;

        /// <summary>
        /// Backing field.
        /// </summary>
        private Component _Component;

        /// <summary>
        /// The <see cref="Component"/> the <see cref="Model"/> currently is attached to.
        /// </summary>
        public Component Component
        {
            get
            {
                return _Component;
            }
            internal set
            {
                Component previous = this.Component;
                this.SetProperty(ref _Component, value);
                this.Raise(this.ComponentChanged, value, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private ObservableCollection<ModelPart> _ModelParts = new ObservableCollection<ModelPart>();

        /// <summary>
        /// A collection of <see cref="ModelPart"/>s this <see cref="Model"/> consists of.
        /// </summary>
        public ObservableCollection<ModelPart> ModelParts
        {
            get
            {
                return _ModelParts;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _ModelParts, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Model"/>.
        /// </summary>
        public Model()
        {
            this.ModelParts.CollectionChanged += (s, e) =>
            {
                if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Move)
                {
                    foreach (ModelPart modelMesh in e.OldItems)
                    {
                        if (modelMesh != null)
                        {
                            modelMesh.Model = null;
                        }
                    }
                    foreach (ModelPart modelMesh in e.NewItems)
                    {
                        if (modelMesh != null)
                        {
                            modelMesh.Model = this;
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Initializes a new <see cref="Model"/> from a range of <see cref="ModelPart"/>s.
        /// </summary>
        /// <param name="modelParts"></param>
        public Model(IEnumerable<ModelPart> modelParts)
            : this()
        {
            Contract.Requires<ArgumentNullException>(modelParts != null);

            this.ModelParts.AddRange(modelParts);
        }

        /// <summary>
        /// Draws the model to the screen.
        /// </summary>
        public void Draw()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
            {
                foreach (ModelPart modelPart in this)
                {
                    if (modelPart != null) // Don't use .FilterNull here as the simple if check is MUCH faster.
                    {
                        modelPart.Draw();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator{T}"/>.
        /// </summary>
        /// <returns>The <see cref="IEnumerator{T}"/>.</returns>
        public IEnumerator<ModelPart> GetEnumerator()
        {
            return this.ModelParts.GetEnumerator();
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator"/>.
        /// </summary>
        /// <returns>The <see cref="IEnumerator"/>.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        public bool Update(GameTime gameTime, int pass)
        {
            try
            {
                this.Raise(this.Updating, gameTime, pass);
                bool result = true;
                foreach (ModelPart part in this.ModelParts)
                {
                    if (part != null) // Don't use .FilterNull here as the simple if check is MUCH faster.
                    {
                        result &= part.Update(gameTime, pass);
                    }
                }
                return result;
            }
            finally
            {
                this.Raise(this.Updated, gameTime, pass);
            }
        }
    }
}
