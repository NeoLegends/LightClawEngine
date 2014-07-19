using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using LightClaw.Engine.Core;
using ProtoBuf;

namespace LightClaw.Engine.IO
{
    public class SceneReader : IContentReader
    {
        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(Scene));
        }

        public Task<object> ReadAsync(string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            return Task.Run(() =>
            {
                if (!ZipFile.IsZipFile(assetStream, true))
                    return null;
                assetStream.Seek(0, SeekOrigin.Begin);

                using (ZipFile sceneZip = ZipFile.Read(assetStream))
                {
                    List<GameObject> gameObjects = new List<GameObject>(8192);

                    foreach (ZipEntry gameObject in sceneZip.Where(entry => entry.FileName.Contains("GameObjects")))
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream((int)gameObject.UncompressedSize))
                            {
                                gameObject.Extract(ms);
                                ms.Position = 0;
                                gameObjects.Add(Serializer.Deserialize<GameObject>(ms));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine(ex.ToString());
                        }
                    }

                    using (MemoryStream ms = new MemoryStream(128))
                    {
                        sceneZip["Name"].Extract(ms);
                        ms.Position = 0;
                        return new Scene(gameObjects) { Name = Encoding.UTF8.GetString(ms.ToArray()) };
                    }
                }
            }).ContinueWith(t => (object)t.Result, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
