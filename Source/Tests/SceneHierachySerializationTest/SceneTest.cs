using System;
using System.IO;
using LightClaw.Engine.Core;
using LightClaw.Engine.Coroutines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;

namespace SceneHierachySerializationTest
{
    [TestClass]
    public class SceneTest
    {
        [TestMethod]
        public void TestSceneSerialization()
        {
            using (FileStream fs = File.OpenWrite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Scene.pbuf")))
            {
                Scene scene = new Scene()
                {
                    new GameObject()
                    {
                        new CoroutineController()
                    }
                };
                Serializer.Serialize(fs, scene);

                Assert.IsFalse(fs.Length == 0);
            }
        }
    }
}
