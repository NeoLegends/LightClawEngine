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
        public BasicEffect() 
        {
            
        }

        protected override void OnUpdate(Engine.Core.GameTime gameTime) { }

        protected override void OnLateUpdate()
        {
            
        }
    }
}
