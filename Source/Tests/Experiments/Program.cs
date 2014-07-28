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

            XmlLayout.LoggingEventInfo entry = new XmlLayout.LoggingEventInfo()
            {
                Exception = new XmlLayout.ExceptionInfo(new Exception("Hello, this is a test!")),
                Level = "INFO",
                Location = new XmlLayout.Location()
                {
                    ClassName = "Program",
                    FileName = "Program.cs",
                    LineNumber = 64,
                    MethodName = "Main"
                },
                Logger = "Experiments.Program",
                Message = "Something normal happened.",
                Thread = "1",
                Timestamp = DateTime.UtcNow.ToString()
            };

            using (FileStream fs = new FileStream("E:\\Users\\Moritz\\Desktop\\Test.xml", FileMode.Create, FileAccess.ReadWrite))
            {
                new DataContractSerializer(typeof(XmlLayout.LoggingEventInfo)).WriteObject(fs, entry);
            }
        }
    }
}
