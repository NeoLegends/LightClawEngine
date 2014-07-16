using System;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreTests
{
    [TestClass]
    public class SerializerTest
    {
        [TestMethod]
        public async Task TestChildManagerSerialization()
        {
            Scene scene = new Scene();
            GameObject gameObject = new GameObject(new Transform(Vector3.ForwardLH, Quaternion.Zero, Vector3.One).Yield());
            scene.Add(gameObject);

            LightClawSerializer ser = new LightClawSerializer();
            byte[] result = await ser.SerializeAsync(scene);

            Assert.IsTrue(result.Length > 0);
        }
    }
}
