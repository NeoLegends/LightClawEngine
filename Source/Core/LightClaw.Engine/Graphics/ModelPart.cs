using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents a subdivision of the <see cref="Model"/>-class allowing for different shaders on the same <see cref="Model"/>.
    /// </summary>
    public class ModelPart : Entity, IDrawable, IUpdateable, ILateUpdateable
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
        public event EventHandler<ValueChangedEventArgs<ModelEffect>> EffectChanged;

        /// <summary>
        /// Notifies about changes in the parent <see cref="Model"/>.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Model>> ModelChanged;

        /// <summary>
        /// Notifies about the start of the updating process.
        /// </summary>
        /// <remarks>Raised before any updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> Updating;

        /// <summary>
        /// Notifies about the finsih of the updating process.
        /// </summary>
        /// <remarks>Raised after any updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> Updated;

        /// <summary>
        /// Notifies about the start of the late updating process.
        /// </summary>
        /// <remarks>Raised before any late updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> LateUpdating;

        /// <summary>
        /// Notifies about the finsih of the late updating process.
        /// </summary>
        /// <remarks>Raised after any late updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> LateUpdated;

        /// <summary>
        /// Notifies about changes in the <see cref="VertexArrayObject"/>.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<VertexArrayObject>> VaoChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private ModelEffect _Effect;

        /// <summary>
        /// Gets the <see cref="ModelEffect"/> used to render the <see cref="ModelPart"/>.
        /// </summary>
        public ModelEffect Effect
        {
            get
            {
                return _Effect;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                ModelEffect oldValue = _Effect;
                if (oldValue != null)
                {
                    oldValue.ModelPart = null;
                }
                this.SetProperty(ref _Effect, value);
                value.ModelPart = this;
                this.Raise(this.EffectChanged, value, oldValue);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private Model _Model;

        /// <summary>
        /// The <see cref="Model"/> the <see cref="ModelPart"/> is a part of.
        /// </summary>
        public Model Model
        {
            get
            {
                return _Model;
            }
            internal set
            {
                Model previous = this.Model;
                this.SetProperty(ref _Model, value);
                this.Raise(this.ModelChanged, value, previous);
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
                VertexArrayObject previous = this.Vao;
                this.SetProperty(ref _Vao, value);
                this.Raise(this.VaoChanged, value, previous);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="ModelPart"/>.
        /// </summary>
        public ModelPart() { }

        /// <summary>
        /// Initializes a new <see cref="ModelPart"/> and sets <see cref="P:Material"/> and <see cref="P:Vao"/>.
        /// </summary>
        /// <param name="effect">The <see cref="ModelEffect"/> used to shade the <see cref="ModelPart"/>.</param>
        /// <param name="vao">The <see cref="VertexArrayObject"/> storing the geometry data.</param>
        public ModelPart(ModelEffect effect, VertexArrayObject vao)
        {
            Contract.Requires<ArgumentNullException>(effect != null);
            Contract.Requires<ArgumentNullException>(vao != null);

            this.Effect = effect;
            this.Vao = vao;
        }

        /// <summary>
        /// Draws the <see cref="ModelPart"/> to the screen.
        /// </summary>
        public void Draw()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
            {
                Effect effect = this.Effect;
                VertexArrayObject vao = this.Vao;
                if ((effect != null) && (vao != null))
                {
                    using (GLBinding vaoBinding = new GLBinding(vao))
                    {
                        for (int i = 0; i < effect.Passes.Count; i++)
                        {
                            effect.Apply(i);
                            GL.DrawElements(BeginMode.Triangles, vao.IndexCount, DrawElementsType.UnsignedShort, 0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="ModelPart"/> updating the <see cref="Material"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        public void Update(GameTime gameTime)
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
            {
                Effect effect = this.Effect;
                if (effect != null)
                {
                    effect.Update(gameTime);
                }
            }
        }

        /// <summary>
        /// Late-updates the <see cref="ModelPart"/>.
        /// </summary>
        public void LateUpdate()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
            {
                Effect effect = this.Effect;
                if (effect != null)
                {
                    effect.LateUpdate();
                }
            }
        }
    }
}
