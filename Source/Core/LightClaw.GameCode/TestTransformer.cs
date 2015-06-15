using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.IO;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.GameCode
{
    [DataContract]
    public class TestTransformer : Component
    {
        private static readonly Matrix4 rot = Matrix4.CreateFromQuaternion(Quaternion.FromAxisAngle(Vector3.UnitX, MathF.DegreesToRadians(-90.0f)));

        public TestTransformer() { }

        protected override bool OnUpdate(GameTime gameTime, int pass)
        {
            if (pass == 0)
            {
                float degreeValue = (float)((gameTime.TotalGameTime.TotalSeconds * 90.0) % 360.0);
                this.Transform.LocalRotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathF.DegreesToRadians(degreeValue));
            }
            return true;
        }
    }
}
