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
    public class BasicMesh : Component
    {
        private static readonly Vector3[] cubeData = new[]
        {
            //new Vector3(-0.5f,  0.5f, 0.0f),
            //new Vector3(-0.5f, -0.5f, 0.0f),
            //new Vector3( 0.5f, -0.5f, 0.0f),
            //new Vector3( 0.5f,  0.5f, 0.0f)
            new Vector3(-0.25f,  0.25f,  0.25f),
            new Vector3( 0.25f,  0.25f,  0.25f),
            new Vector3( 0.25f,  0.25f, -0.25f),
            new Vector3(-0.25f,  0.25f, -0.25f),
            
            new Vector3(-0.25f, -0.25f,  0.25f),
            new Vector3( 0.25f, -0.25f,  0.25f),
            new Vector3( 0.25f, -0.25f, -0.25f),
            new Vector3(-0.25f, -0.25f, -0.25f)
        };

        private static readonly Vector3[] colorData = new[]
        {
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.5f, 0.0f, 0.5f),
            
            new Vector3(0.5f, 0.0f, 0.5f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f)
        };

        public static readonly Vector3[] vertexData = new[] 
        {
            new Vector3(-0.25f,  0.25f,  0.25f),
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3( 0.25f,  0.25f,  0.25f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3( 0.25f,  0.25f, -0.25f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(-0.25f,  0.25f, -0.25f),
            new Vector3(0.5f, 0.0f, 0.5f),
            new Vector3(-0.25f, -0.25f,  0.25f),
            new Vector3(0.5f, 0.0f, 0.5f),
            new Vector3( 0.25f, -0.25f,  0.25f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3( 0.25f, -0.25f, -0.25f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(-0.25f, -0.25f, -0.25f),
            new Vector3(1.0f, 0.0f, 0.0f)
        };

        private static readonly ushort[] indices = new ushort[]
        {
            //0, 1, 2,
            //0, 2, 3
            // Top
            0, 1, 2,
            0, 2, 3,

            // Front
            0, 5, 1,
            0, 4, 5,

            // Right
            1, 6, 2,
            1, 6, 5,

            // Left
            0, 3, 7,
            0, 5, 4,

            // Back
            2, 7, 3,
            2, 6, 7
        };

        private static readonly Matrix4 viewProjectionMatrix = 
            Matrix4.CreatePerspectiveFieldOfView(MathF.DegreesToRadians(50), 16 / 9, 0.01f, 100f) *
            Matrix4.LookAt(new Vector3(6, 3, 6), Vector3.Zero, new Vector3(0, 1, 0));

        private static readonly Matrix4 rot = Matrix4.CreateFromQuaternion(Quaternion.FromAxisAngle(Vector3.UnitX, MathF.DegreesToRadians(-90.0f)));

        private int getErrorCount = 0;

        private Model model;

        private Matrix4 modelMatrix;

        public BasicMesh() { }

        protected override void Dispose(bool disposing)
        {
            try
            {
                Model m = this.model;
                if (m != null)
                {
                    m.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected override async void OnLoad()
        {
            IContentManager mgr = this.IocC.Resolve<IContentManager>();
            this.model = await mgr.LoadAsync<Model>("Game/Dragonborn.FBX");
        }

        protected override void OnDraw()
        {
            Model m = this.model;
            if (m != null)
            {
                m.Draw(ref this.modelMatrix);
            }
        }

        protected override bool OnUpdate(GameTime gameTime, int pass)
        {
            if (pass == 0)
            {
                float degreeValue = (float)((gameTime.TotalGameTime.TotalSeconds * 90.0) % 360.0);
                float radiansValue = MathF.DegreesToRadians(degreeValue);
                this.modelMatrix = rot * Matrix4.CreateRotationY(radiansValue);
            }
            return true;
        }
    }
}
