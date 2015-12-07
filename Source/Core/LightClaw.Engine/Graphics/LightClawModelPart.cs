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
        /// <summary>
        /// Indicates whether the <see cref="LightClawModelPart"/> owns its diffuse texture.
        /// </summary>
        protected readonly bool OwnsDiffuse;

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
        public LightClawModelPart(Effect effect, VertexArrayObject vao, Texture2D diffuse)
            : this(effect, vao, diffuse, false, true, false)
        {
            Contract.Requires<ArgumentNullException>(effect != null);
            Contract.Requires<ArgumentNullException>(vao != null);
        }

        /// <summary>
        /// Initializes a new <see cref="LightClawModelPart"/>.
        /// </summary>
        /// <param name="effect">The <see cref="Effect"/>.</param>
        /// <param name="vao">The <see cref="VertexArrayObject"/>.</param>
        /// <param name="diffuse">The diffuse texture.</param>
        /// <param name="ownsEffect">Indicates whether the <paramref cref="effect"/> will be taken ownage of.</param>
        /// <param name="ownsVao">Indicates whether the <paramref cref="vao"/> will be taken ownage of.</param>
        /// <param name="ownsTexture">Indicates whether the <paramref cref="diffuse"/> texture will be taken ownage of.</param>
        public LightClawModelPart(Effect effect, VertexArrayObject vao, Texture2D diffuse, bool ownsEffect, bool ownsVao, bool ownsTexture)
            : base(effect, vao, ownsEffect, ownsVao)
        {
            Contract.Requires<ArgumentNullException>(effect != null);
            Contract.Requires<ArgumentNullException>(vao != null);

            this.Diffuse = diffuse;
            this.OwnsDiffuse = ownsTexture;
        }

        /// <summary>
        /// Disposes the <see cref="LightClawModelPart"/> and releases all associated resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources are to be disposed as well, otherwise <c>false</c>.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.OwnsDiffuse)
                {
                    this.Diffuse.Dispose();
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
        /// <param name="mvp">The model view projection matrix used to draw the <see cref="ModelPart"/>.</param>
        protected override void OnDraw(ref Matrix4 mvp)
        {
            Effect effect = this.Effect;
            VertexArrayObject vao = this.Vao;
            if ((effect != null) && (vao != null))
            {
                EffectPass pass = effect.First();

                pass.DataUniforms["MVP"].Set(ref mvp);
                //pass.SamplerUniforms["diffuse"].Set(this.Diffuse, TextureUnit.Texture0);

                using (Binding vaoBinding = vao.Bind())
                using (Binding effectPassPinding = pass.Bind())
                {
                    vao.DrawIndexed();
                }
            }
        }
    }
}
