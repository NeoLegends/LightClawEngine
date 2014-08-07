using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
            List<GameObject> gameObjects = new List<GameObject>(2048);
            for (int i = 0; i < 2048; i++)
            {
                gameObjects.Add(
                    new GameObject(
                        new Transform(Vector3.Random, Quaternion.Random, Vector3.Random),
                        RandomF.GetBoolean() ? (Component)GetCamera() : (Component)GetMesh(),
                        RandomF.GetBoolean() ? (Component)GetCamera() : null,
                        RandomF.GetBoolean() ? (Component)GetMesh() : null
                    )
                );
            }

            Scene s = new Scene(gameObjects);

            Console.WriteLine("Saving scenes...");

            using (FileStream fs = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Scene.lcs")))
            {
                s.Save(fs).Wait();
            }

            Console.WriteLine("Compressed save finished, saving uncompressed version now...");

            using (MemoryStream ms = new MemoryStream(1024 * 1024 * 6))
            using (StreamReader sr = new StreamReader(ms))
            using (FileStream fs = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Scene.xml")))
            using (XmlWriter xmlWriter = XmlWriter.Create(fs, new XmlWriterSettings() { Indent = true, IndentChars = "    " }))
            {
                s.SaveXml(ms).Wait();
                ms.Position = 0;
                System.Xml.Linq.XDocument.Parse(sr.ReadToEnd()).Save(xmlWriter);
            }

            GC.Collect();

            Stopwatch st = Stopwatch.StartNew();
            s.Save(Stream.Null).Wait();
            Console.WriteLine("Saving compressed to Stream.Null took {0}ms.".FormatWith(st.ElapsedMilliseconds));

            GC.Collect();

            st.Restart();
            s.SaveXml(Stream.Null).Wait();
            Console.WriteLine("Saving uncompressed to Stream.Null took {0}ms.".FormatWith(st.ElapsedMilliseconds));

            using (MemoryStream ms = new MemoryStream())
            {
                s.Save(ms).Wait();
                ms.Position = 0;

                st.Restart();
                Scene result = Scene.Load(ms).Result;
                Console.WriteLine("Loading a compressed scene took {0}ms.".FormatWith(st.ElapsedMilliseconds));
            }

            GC.Collect();

            using (MemoryStream ms = new MemoryStream())
            {
                s.SaveXml(ms).Wait();
                ms.Position = 0;

                st.Restart();
                Scene result = Scene.LoadXml(ms).Result;
                Console.WriteLine("Loading an uncompressed scene took {0}ms.".FormatWith(st.ElapsedMilliseconds));
            }

            GC.Collect();

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }

        static Camera GetCamera()
        {
            return new Camera()
            {
                FoV = RandomF.GetSingle() * 120.0f,
                Height = RandomF.GetInt32(480, 2161),
                Width = RandomF.GetInt32(640, 3841),
                Zoom = RandomF.GetSingle() * 300.0f
            };
        }

        static Mesh GetMesh()
        {
            return new Mesh(RandomF.GetString(100));
        }
    }
}
