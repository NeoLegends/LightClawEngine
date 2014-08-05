using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Coroutines;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreTests
{
    [TestClass]
    public class SceneLifecycleTests
    {
        [TestMethod]
        public async Task TestSceneSaving()
        {
            Scene s = this.GetScene();

            using (MemoryStream ms = new MemoryStream())
            {
                await s.Save(ms);
                Assert.IsTrue(ms.Length > 0);
            }

            try
            {
                using (FileStream fs = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Scene.lcs")))
                {
                    await s.Save(fs);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        [TestMethod]
        public async Task TestSceneLoading()
        {
            Scene s = this.GetScene();
            using (MemoryStream ms = new MemoryStream())
            {
                await s.Save(ms);
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
