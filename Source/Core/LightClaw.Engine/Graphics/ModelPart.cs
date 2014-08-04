using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class ModelPart : Entity, IDrawable, IUpdateable, ILateUpdateable
    {
        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public event EventHandler<ValueChangedEventArgs<Material>> MaterialChanged;

        public event EventHandler<ValueChangedEventArgs<Model>> ModelChanged;

        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        public event EventHandler<ValueChangedEventArgs<VertexArrayObject>> VaoChanged;

        private Material _Material;

        public Material Material
        {
            get
            {
                return _Material;
            }
            set
            {
                if (value != null)
                {
                    value.ModelPart = this;
                }
                Material previous = this.Material;
                this.SetProperty(ref _Material, value);
                this.Raise(this.MaterialChanged, value, previous);
            }
        }

        private Model _Model;

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

        private VertexArrayObject _Vao;

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

        public ModelPart() { }

        public ModelPart(Material material, VertexArrayObject vao)
        {
            Contract.Requires<ArgumentNullException>(material != null);
            Contract.Requires<ArgumentNullException>(vao != null);

            this.Material = material;
            this.Vao = vao;
        }

        public void Draw()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Drawing, this.Drawn))
            {
                Material mat = this.Material;
                VertexArrayObject vao = this.Vao;
                if ((vao != null) && (mat != null))
                {
                    using (GLBinding materialBinding = new GLBinding(mat))
                    using (GLBinding vaoBinding = new GLBinding(vao))
                    {
                        GL.DrawElements(BeginMode.TriangleStrip, vao.IndexCount, DrawElementsType.UnsignedShort, 0);
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
            {
                Material mat = this.Material;
                if (mat != null)
                {
                    mat.Update(gameTime);
                }
            }
        }

        public void LateUpdate()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
            {
                Material mat = this.Material;
                if (mat != null)
                {
                    mat.LateUpdate();
                }
            }
        }
    }
}
