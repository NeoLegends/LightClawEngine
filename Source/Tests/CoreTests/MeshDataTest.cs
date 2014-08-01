using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreTests
{
    [TestClass]
    public class MeshDataTest
    {
        [TestMethod]
        public async Task TestMeshDataSave()
        {
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
            MeshData loadedMeshData;

            using (MemoryStream ms = new MemoryStream())
            {
                await meshData.Save(ms);
                Console.WriteLine(ms.Length);
                ms.Position = 0;

                loadedMeshData = await MeshData.Load(ms);
            }

            Assert.IsTrue(meshData.Vertices.SequenceEqual(loadedMeshData.Vertices));
            Assert.IsTrue(meshData.Normals.SequenceEqual(loadedMeshData.Normals));
            Assert.IsTrue(meshData.TextureCoordinates.SequenceEqual(loadedMeshData.TextureCoordinates));
        }
    }
}
