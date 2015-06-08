using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a subdivision of the <see cref="Model"/>-class allowing for different shaders on the same
    /// <see cref="Model"/>.
    /// </summary>
    public abstract class ModelPart : DispatcherEntity
    {
        protected readonly bool OwnsEffect;

        protected readonly bool OwnsVao;

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
        /// Notifies about a change in the <see cref="P:Effect"/>.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Effect>> EffectChanged;

        /// <summary>
        /// Notifies about changes in the <see cref="VertexArrayObject"/>.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<VertexArrayObject>> VaoChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private Effect _Effect;

        /// <summary>
        /// Gets the <see cref="Effect"/> used to render the <see cref="ModelPart"/>.
        /// </summary>
        public Effect Effect
        {
            get
            {
                return _Effect;
            }
            protected set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Effect, value, this.EffectChanged);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private VertexArrayObject _Vao;

        /// <summary>
        /// The <see cref="VertexArrayObject"/> storing the geometry data.
        /// </summary>
        public VertexArrayObject Vao
        {
            get
            {
                return _Vao;
            }
            protected set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Vao, value, this.VaoChanged);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="ModelPart"/>.
        /// </summary>
        protected ModelPart() { }

        /// <summary>
        /// Initializes a new <see cref="ModelPart"/> and sets <see cref="P:Material"/> and <see cref="P:Vao"/>.
        /// </summary>
        /// <param name="effect">The <see cref="Effect"/> used to shade the <see cref="ModelPart"/>.</param>
        /// <param name="vao">The <see cref="VertexArrayObject"/> storing the geometry data. Will be taken ownage of.</param>
        protected ModelPart(Effect effect, VertexArrayObject vao)
            : this(effect, vao, false, true)
        {
            Contract.Requires<ArgumentNullException>(effect != null);
            Contract.Requires<ArgumentNullException>(vao != null);
        }

        /// <summary>
        /// Initializes a new <see cref="ModelPart"/> and sets <see cref="P:Material"/> and <see cref="P:Vao"/>.
        /// </summary>
        /// <param name="effect">The <see cref="Effect"/> used to shade the <see cref="ModelPart"/>.</param>
        /// <param name="vao">The <see cref="VertexArrayObject"/> storing the geometry data.</param>
        /// <param name="ownsEffect">Indicates whether the <paramref cref="effect"/> will be taken ownage of.</param>
        /// <param name="ownsEffect">Indicates whether the <paramref cref="vao"/> will be taken ownage of.</param>
        protected ModelPart(Effect effect, VertexArrayObject vao, bool ownsEffect, bool ownsVao)
        {
            Contract.Requires<ArgumentNullException>(effect != null);
            Contract.Requires<ArgumentNullException>(vao != null);

            this.Effect = effect;
            this.OwnsEffect = ownsEffect;
            this.OwnsVao = ownsVao;
            this.Vao = vao;
        }

        /// <summary>
        /// Draws the <see cref="ModelPart"/> to the screen.
        /// </summary>
        /// <param name="mvp">The model view projection matrix used to draw the <see cref="ModelPart"/>.</param>
        public void Draw(ref Matrix4 mvp)
        {
            this.VerifyAccess();

            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
            {
                this.OnDraw(ref mvp);
            }
        }

        /// <summary>
        /// Disposes the <see cref="ModelPart"/> and releases all associated resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources are to be disposed as well, otherwise <c>false</c>.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.OwnsEffect)
                {
                    this.Effect.Dispose();
                }
                if (this.OwnsVao)
                {
                    this.Vao.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Drawing callback.
        /// </summary>
        /// <param name="transform">The current transformation matrix.</param>
        protected abstract void OnDraw(ref Matrix4 transform);
    }
}
