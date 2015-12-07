using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using LightClaw.Engine.Threading.Coroutines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using OpenTK;
using Tests.Utilities;

namespace Tests.Core
{
    [TestClass]
    public class SceneLifecycleTests
    {
        [TestMethod]
        public async Task SceneSave()
        {
            Scene s = this.GetScene();

            using (MemoryStream ms = new MemoryStream())
            using (FileStream fs = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Scene.lcs")))
            {
                Console.WriteLine("Time to save the scene with compression: " + await TestUtilities.DoMeasuredActionAsync(() => s.SaveAsync(ms)));
                Assert.IsTrue(ms.Length > 0);

                ms.Position = 0;
                ms.CopyTo(fs);
            }

            GC.Collect();

            using (MemoryStream ms = new MemoryStream())
            using (FileStream fs = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SceneRaw.lcs")))
            {
                Console.WriteLine("Time to save the scene without compression: " + await TestUtilities.DoMeasuredActionAsync(() => s.SaveRawAsync(ms)));

                ms.Position = 0;
                ms.CopyTo(fs);
            }

            GC.Collect();

            using (MemoryStream ms = new MemoryStream())
            using (BsonWriter bw = new BsonWriter(ms))
            using (FileStream fs = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SceneBson.lcs")))
            {
                JsonSerializer ser = JsonSerializer.CreateDefault(
                    new JsonSerializerSettings()
                    {
                        DefaultValueHandling = DefaultValueHandling.Populate,
                        NullValueHandling = NullValueHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.All,
                        TypeNameHandling = TypeNameHandling.Auto
                    }
                );
                Console.WriteLine("Time to save the scene as BSON: " + TestUtilities.DoMeasuredAction(() => ser.Serialize(bw, s)));

                ms.Position = 0;
                ms.CopyTo(fs);
            }
        }

        [TestMethod]
        public async Task SceneLoad()
        {
            Scene s = this.GetScene();
            using (MemoryStream ms = new MemoryStream())
            {
                await s.SaveAsync(ms);
                ms.Position = 0;
                Scene deser = await Scene.Load(ms);

                Assert.AreEqual(s.Count, deser.Count);
            }
        }

        private Scene GetScene()
        {
            return new Scene(
                new[] {
                    new GameObject(
                        new CoroutineController()
                    ),
                    new GameObject(
                        new Component[] {
                            new Transform(new Vector3(10, 15, 20), Quaternion.Identity, Vector3.One),
                            new Camera() { FoV = 16 / 9, Height = 1080, Width = 1920, Zoom = 18 }
                        }
                    )
                }
            ) { Name = "TestScene" };
        }
    }
}
