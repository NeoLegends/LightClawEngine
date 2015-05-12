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
            set
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
            set
            {
                this.SetProperty(ref _Vao, value, this.VaoChanged);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="ModelPart"/>.
        /// </summary>
        public ModelPart() { }

        /// <summary>
        /// Initializes a new <see cref="ModelPart"/> and sets <see cref="P:Material"/> and <see cref="P:Vao"/>.
        /// </summary>
        /// <param name="effect">The <see cref="Effect"/> used to shade the <see cref="ModelPart"/>.</param>
        /// <param name="vao">The <see cref="VertexArrayObject"/> storing the geometry data.</param>
        public ModelPart(Effect effect, VertexArrayObject vao)
        {
            Contract.Requires<ArgumentNullException>(effect != null);
            Contract.Requires<ArgumentNullException>(vao != null);

            this.Effect = effect;
            this.Vao = vao;
        }

        /// <summary>
        /// Draws the <see cref="ModelPart"/> to the screen.
        /// </summary>
        public void Draw(ref Matrix4 transform)
        {
            this.VerifyAccess();

            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
            {
                //Effect effect = this.Effect;
                //VertexArrayObject vao = this.Vao;
                //if ((effect != null) && (vao != null))
                //{
                //    using (Binding vaoBinding = new Binding(vao))
                //    {
                //        for (int i = 0; i < effect.Count; i++)
                //        {
                //            using (Binding effectPassPinding = effect.ApplyPass(i))
                //            {
                //                vao.DrawIndexed();
                //            }
                //        }
                //    }
                //}
                this.OnDraw(ref transform);
            }
        }

        /// <summary>
        /// Drawing callback.
        /// </summary>
        /// <param name="transform">The current transformation matrix.</param>
        protected abstract void OnDraw(ref Matrix4 transform);
    }
}
