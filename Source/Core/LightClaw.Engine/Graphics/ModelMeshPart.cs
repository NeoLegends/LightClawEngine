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
    public class ModelMeshPart : Entity, IDrawable, IUpdateable, ILateUpdateable
    {
        public event EventHandler<ParameterEventArgs> Drawing;

        public event EventHandler<ParameterEventArgs> Drawn;

        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        private Material _Material;

        public Material Material
        {
            get
            {
                return _Material;
            }
            set
            {
                this.SetProperty(ref _Material, value);
            }
        }

        private ModelMesh _ModelMesh;

        public ModelMesh ModelMesh
        {
            get
            {
                return _ModelMesh;
            }
            internal set
            {
                this.SetProperty(ref _ModelMesh, value);
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
                this.SetProperty(ref _Vao, value);
            }
        }

        public ModelMeshPart() { }

        public ModelMeshPart(Material material, VertexArrayObject vao)
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
                VertexArrayObject vao = this.Vao;
                if (vao != null)
                {
                    // Material will be bound by ModelMesh to reduce shader switches, do not bind here
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
