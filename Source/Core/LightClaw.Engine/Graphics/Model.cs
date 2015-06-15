using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using OpenTK;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a three-dimensional polygon model.
    /// </summary>
    [ContentReader(typeof(ModelReader))]
    public class Model : DisposableEntity, IReadOnlyList<ModelPart>
    {
        /// <summary>
        /// Indicates whether the <see cref="Model"/> owns the <see cref="ModelPart"/>s it
        /// has been given inside the constructor.
        /// </summary>
        private readonly bool ownsParts;

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
        /// Gets the amount of <see cref="ModelParts"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return this.ModelParts.Length;
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private ImmutableArray<ModelPart> _ModelParts;

        /// <summary>
        /// A collection of <see cref="ModelPart"/>s this <see cref="Model"/> consists of.
        /// </summary>
        public ImmutableArray<ModelPart> ModelParts
        {
            get
            {
                return _ModelParts;
            }
            private set
            {
                this.SetProperty(ref _ModelParts, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="ModelPart"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the <see cref="ModelPart"/> to get.</param>
        /// <returns>The <see cref="ModelPart"/> with the specified <paramref name="index"/>.</returns>
        public ModelPart this[int index]
        {
            get
            {
                return this.ModelParts[index];
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Model"/>.
        /// </summary>
        private Model() { }

        /// <summary>
        /// Initializes a new <see cref="Model"/> from a range of <see cref="ModelPart"/>s.
        /// </summary>
        /// <param name="modelParts">The initial <see cref="ModelPart"/>s. Will be taken ownage of.</param>
        public Model(params ModelPart[] modelParts)
            : this(null, modelParts)
        {
            Contract.Requires<ArgumentNullException>(modelParts != null);
        }

        /// <summary>
        /// Initializes a new <see cref="Model"/> from a range of <see cref="ModelPart"/>s.
        /// </summary>
        /// <param name="modelParts">The initial <see cref="ModelPart"/>s.</param>
        /// <param name="ownsParts">
        /// Indicates whether the <see cref="Model"/> owns the <paramref name="modelParts"/> and thus is allowed to dispose of them.
        /// </param>
        public Model(IEnumerable<ModelPart> modelParts, bool ownsParts)
            : this(null, modelParts, ownsParts)
        {
            Contract.Requires<ArgumentNullException>(modelParts != null);
        }

        /// <summary>
        /// Initializes a new <see cref="Model"/> from a range of <see cref="ModelPart"/>s.
        /// </summary>
        /// <param name="name">The <see cref="Model"/>s name.</param>
        /// <param name="modelParts">The initial <see cref="ModelPart"/>s. Will be taken ownage of.</param>
        public Model(string name, params ModelPart[] modelParts)
            : this(name, modelParts, true)
        {
            Contract.Requires<ArgumentNullException>(modelParts != null);
        }

        /// <summary>
        /// Initializes a new <see cref="Model"/> from a range of <see cref="ModelPart"/>s.
        /// </summary>
        /// <param name="name">The <see cref="Model"/>s name.</param>
        /// <param name="modelParts">The initial <see cref="ModelPart"/>s.</param>
        /// <param name="ownsParts">
        /// Indicates whether the <see cref="Model"/> owns the <paramref name="modelParts"/> and thus is allowed to dispose of them.
        /// </param>
        public Model(string name, IEnumerable<ModelPart> modelParts, bool ownsParts)
            : base(name)
        {
            Contract.Requires<ArgumentNullException>(modelParts != null);

            this.ModelParts = modelParts.ToImmutableArray();
            this.ownsParts = ownsParts;
        }

        /// <summary>
        /// Draws the model to the screen.
        /// </summary>
        /// <param name="mvp">The model view projection matrix used to draw the <see cref="Model"/>.</param>
        public void Draw(ref Matrix4 mvp)
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
            {
                for (int i = 0; i < this.Count; i++)
                {
                    ModelPart part = this[i];
                    if (part != null)
                    {
                        part.Draw(ref mvp);
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
            return this.ModelParts.AsEnumerable()
                                  .GetEnumerator();
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
        /// Disposes the <see cref="ModelPart"/> releasing all resources.
        /// </summary>
        /// <param name="disposing">Indicates whether managed objects shall be disposed as well.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.ownsParts)
                {
                    foreach (ModelPart part in this.ModelParts)
                    {
                        try
                        {
                            part.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Log.Warn("An exception of type {0} was thrown while disposing the {1}'s {2}s.".FormatWith(ex.GetType().FullName, typeof(Model), typeof(ModelPart)), ex);
                        }
                    }
                }
                if (disposing)
                {
                    this.ModelParts = new ImmutableArray<ModelPart>();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
