using System;
using System.IO;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Coroutines;
using LightClaw.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;

namespace CoreTests
{
    [TestClass]
    public class SerializerTest
    {
        [TestMethod]
        public void TestGameObjectSerialization()
        {
            GameObject gameObject = new GameObject();
            gameObject.Add(new CoroutineController());

            gameObject.Load();

            using (Stream ms = new MemoryStream(256))
            {
                Serializer.Serialize(ms, gameObject);

                Assert.IsTrue(ms.Length > 0);
                ms.Position = 0;
                Console.WriteLine(ms.Length);

                //GameObject deserializedObject = Serializer.Deserialize<GameObject>(ms);
                //Assert.IsTrue(deserializedObject.Count > 1);
            }
        }

        [TestMethod]
        public void TestMatrixSerialization()
        {
            using (Stream ms = new MemoryStream(128))
            {
                Serializer.Serialize(ms, Matrix.Random);

                Assert.IsTrue(ms.Length > 0);
                ms.Position = 0;

                Matrix mat = Serializer.Deserialize<Matrix>(ms);
            }
        }

        [TestMethod]
        public void TestMatrix3x2Serialization()
        {
            using (Stream ms = new MemoryStream(128))
            {
                Serializer.Serialize(ms, Matrix3x2.Random);

                Assert.IsTrue(ms.Length > 0);
                ms.Position = 0;

                Matrix3x2 mat = Serializer.Deserialize<Matrix3x2>(ms);
            }
        }

        [TestMethod]
        public void TestMatrix3x3Serialization()
        {
            using (Stream ms = new MemoryStream(128))
            {
                Serializer.Serialize(ms, Matrix3x3.Random);

                Assert.IsTrue(ms.Length > 0);
                ms.Position = 0;

                Matrix3x3 mat = Serializer.Deserialize<Matrix3x3>(ms);
            }
        }

        [TestMethod]
        public void TestMatrix5x4Serialization()
        {
            using (Stream ms = new MemoryStream(192))
            {
                Serializer.Serialize(ms, Matrix5x4.Random);

                Assert.IsTrue(ms.Length > 0);
                ms.Position = 0;

                Matrix5x4 mat = Serializer.Deserialize<Matrix5x4>(ms);
            }
        }
    }
}
