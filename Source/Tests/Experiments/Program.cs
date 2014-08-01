using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Core;
using LightClaw.Engine.Coroutines;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;
using Munq;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            //try
            //{
            //    IocContainer container = LightClawEngine.DefaultIocContainer;

            //    Shader vertexShader = new Shader("Shaders/Basic.vert", ShaderType.VertexShader);
            //    Shader fragmentShader = new Shader("Shaders/Basic.frag", ShaderType.FragmentShader);
            //    //ShaderProgram program = new ShaderProgram(new[] { vertexShader, fragmentShader });

            //    MeshPartCollection partCollection = new MeshPartCollection();
            //    Mesh cube = new Mesh(partCollection);

            //    GameObject gameObject = new GameObject(
            //        new Transform(new Vector3(0.0f, 0.0f, -2.0f), Quaternion.Identity, Vector3.One),
            //        cube
            //    );
            //    Scene startScene = new Scene();
            //    startScene.Add(gameObject);

            //    using (IGame game = new Game(Assembly.LoadFrom(GeneralSettings.Default.GameCodeAssembly), startScene))
            //    {
            //        game.Run();
            //    }
            //}
            //finally
            //{
            //    LogManager.Shutdown();
            //}

            Vector3[] normals = new Vector3[1000];
            Vector2[] texCoords = new Vector2[1000];
            Vector3[] vertices = new Vector3[1000];

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = Vector3.Random;
            }
            for (int i = 0; i < texCoords.Length; i++)
            {
                texCoords[i] = Vector2.Random;
            }
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Random;
            }

            MeshData meshData = new MeshData(vertices, normals, texCoords);

            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, meshData);
                Console.WriteLine("Pbuf: " + ms.Length);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                meshData.Save(ms).Wait();
                Console.WriteLine("Custom: " + ms.Length);
            }

            Stopwatch st = Stopwatch.StartNew();
            for (int i = 0; i < 250; i++)
            {
                meshData.Save(Stream.Null).Wait();
            }
            Console.WriteLine("Saving 250 times using custom serialization took {0}ms.".FormatWith(st.Elapsed));

            st.Restart();
            for (int i = 0; i < 250; i++)
            {
                Serializer.Serialize(Stream.Null, meshData);
            }
            Console.WriteLine("Saving 250 times using protbuf-net took {0}ms.".FormatWith(st.Elapsed));

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }
    }
}
