using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.GameCode
{
    public class BasicEffect : ModelEffect
    {
        private readonly Matrix projectionMatrix = Matrix.PerspectiveFovLH(MathF.DegreesToRadians(90), (float)(16 / 9), 0.1f, 100.0f);

        private readonly Matrix viewMatrix = Matrix.LookAtLH(new Vector3(10, 15, 3), Vector3.Zero, Vector3.One);

        public BasicEffect() 
        {
            IContentManager contentManager = this.IocC.Resolve<IContentManager>();
            this.Passes = new EffectPass(
                new ShaderPipeline(
                    new[] { 
                        new ShaderProgram(contentManager.LoadAsync<string>(".\\Basic.frag").Result.YieldArray(), ShaderType.FragmentShader),
                        new ShaderProgram(contentManager.LoadAsync<string>(".\\Basic.vert").Result.YieldArray(), ShaderType.VertexShader)
                    }
                )
            ).YieldArray().ToImmutableList();
        }

        protected override void OnUpdate(Engine.Core.GameTime gameTime) { }

        protected override void OnLateUpdate()
        {
            this.Passes[0].Stages[0].Values["modelViewProjectionMatrix"].Set(
                projectionMatrix * viewMatrix * this.ModelPart.Model.Component.GameObject.OfType<Transform>().ModelMatrix
            );
        }
    }
}
