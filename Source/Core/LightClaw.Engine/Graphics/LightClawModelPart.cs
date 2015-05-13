using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics.OpenGL;
using OpenTK;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// The default implementation of <see cref="ModelPart"/>.
    /// </summary>
    public class LightClawModelPart : ModelPart
    {
        private Texture2D _Diffuse;

        /// <summary>
        /// The diffuse texture.
        /// </summary>
        public Texture2D Diffuse
        {
            get
            {
                return _Diffuse;
            }
            private set
            {
                this.SetProperty(ref _Diffuse, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="LightClawModelPart"/>.
        /// </summary>
        /// <param name="effect">The <see cref="Effect"/>.</param>
        /// <param name="vao">The <see cref="VertexArrayObject"/>.</param>
        public LightClawModelPart(Effect effect, VertexArrayObject vao, Texture2D texture)
            : base(effect, vao)
        {
            Contract.Requires<ArgumentNullException>(effect != null);
            Contract.Requires<ArgumentNullException>(vao != null);
        }

        /// <summary>
        /// Drawing callback.
        /// </summary>
        /// <param name="mvp">The model view projection matrix used to draw the <see cref="ModelPart"/>.</param>
        protected override void OnDraw(ref Matrix4 mvp)
        {
            Effect effect = this.Effect;
            VertexArrayObject vao = this.Vao;
            if ((effect != null) && (vao != null))
            {
                using (Binding vaoBinding = vao.Bind())
                using (Binding effectPassPinding = effect.ApplyPass(0))
                {
                    //effect.First().DataUniforms["MVP"].Set(ref mvp);
                    vao.DrawIndexed();
                }
            }
        }
    }
}
