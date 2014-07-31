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

            Vector3 first = Vector3.Random;
            Vector3 second = Vector3.Random;
            Vector3 third = Vector3.Random;

            Vector3[] data = new[] { first, second, third };

            byte[] bufferBlockCopyResult = new byte[data.Length * Vector3.SizeInBytes];
            System.Buffer.BlockCopy(data, 0, bufferBlockCopyResult, 0, data.Length * Vector3.SizeInBytes);

            byte[] marshalResult = new byte[data.Length * Vector3.SizeInBytes];
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(data.Length * Vector3.SizeInBytes);
            Marshal.StructureToPtr(data[0], unmanagedPointer, false);
            Marshal.StructureToPtr(data[1], (IntPtr)(unmanagedPointer + Vector3.SizeInBytes), false);
            Marshal.StructureToPtr(data[2], (IntPtr)(unmanagedPointer + 2 * Vector3.SizeInBytes), false);
            Marshal.Copy(unmanagedPointer, marshalResult, 0, marshalResult.Length);
            Marshal.FreeHGlobal(unmanagedPointer);

            Console.WriteLine(bufferBlockCopyResult.SequenceEqual(marshalResult));

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }
    }
}
