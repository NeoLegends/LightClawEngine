using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using ObjLoader.Loader;
using ObjLoader.Loader.Data;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.IO
{
    public class ModelReader : DispatcherEntity, IContentReader
    {
        private static readonly ConcurrentDictionary<IContentManager, IObjLoader> cachedLoaders = new ConcurrentDictionary<IContentManager, IObjLoader>();

        private static readonly ObjLoaderFactory loaderFactory = new ObjLoaderFactory();

        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(Model));
        }

        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            IContentManager mgr = parameters.ContentManager;
            LoadResult loadResult = cachedLoaders.GetOrAdd(parameters.ContentManager, cmgr => loaderFactory.Create(new LightClawMaterialResolver(cmgr)))
                                                 .Load(parameters.AssetStream);
            ModelPart[] modelParts = await Task.WhenAll(loadResult.Groups.Select(async group =>
            {
                Task<Effect> effectLoadTask = mgr.LoadAsync<Effect>("Shaders/Basic.shr", parameters.CancellationToken);
                //Task<Texture2D> textureLoadTask = mgr.LoadAsync<Texture2D>(group.Material.DiffuseTextureMap, parameters.CancellationToken);

                ushort[] indices;
                Vertex[] vertices;
                ProcessGroup(group, loadResult, out indices, out vertices);

                Effect effect = await effectLoadTask.ConfigureAwait(false);
                //Texture2D texture = await textureLoadTask.ConfigureAwait(false);

                return await this.Dispatcher.Invoke(() =>
                {
                    IBuffer indexBuffer = BufferObject.Create(indices, BufferTarget.ElementArrayBuffer);
                    IBuffer vertexBuffer = BufferObject.Create(vertices, BufferTarget.ArrayBuffer);

                    EffectPass pass = effect.Passes.First();
                    VertexArrayObject vao = new VertexArrayObject(
                        indexBuffer,
                        new BufferDescription(
                            vertexBuffer,
                            new VertexAttributePointer(pass.Attributes["inVertexPosition"], 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0),
                            new VertexAttributePointer(pass.Attributes["inVertexColor"], 4, VertexAttribPointerType.UnsignedByte, false, Vertex.SizeInBytes, 32)
                            //Vertex.GetInterleavedVaps(pass.Attributes["inVertexPosition"], pass.Attributes["inVertexNormal"], pass.Attributes["inVertexTexCoords"], pass.Attributes["inVertexColor"])
                        )
                    );

                    return new LightClawModelPart(effect, vao, null);
                }).ConfigureAwait(false);
            })).ConfigureAwait(false);

            return new Model(modelParts, true);
        }

        private static void ProcessGroup(Group group, LoadResult loadResult, out ushort[] indices, out Vertex[] vertices)
        {
            Contract.Requires<ArgumentNullException>(group != null);
            Contract.Requires<ArgumentNullException>(loadResult != null);

            checked
            {
                ushort highestVertexIndex = 0;
                List<ushort> indexList = new List<ushort>();
                Dictionary<VertexIndexGroup, ushort> verticesWithIndices = new Dictionary<VertexIndexGroup, ushort>();

                for (ushort i = 0; i < group.Faces.Count; i++)
                {
                    Face face = group.Faces[i];
                    for (ushort j = 0; j < face.Count; j++)
                    {
                        FaceVertex vertex = face[j];

                        VertexIndexGroup vi = new VertexIndexGroup(vertex.VertexIndex, vertex.NormalIndex, vertex.TextureIndex);
                        ushort index;
                        if (verticesWithIndices.TryGetValue(vi, out index))
                        {
                            indexList.Add(index);
                        }
                        else
                        {
                            verticesWithIndices.Add(vi, highestVertexIndex);
                            indexList.Add(highestVertexIndex);

                            highestVertexIndex++;
                        }
                    }
                }

                indices = indexList.ToArray();
                vertices = verticesWithIndices.Keys.Select(v => v.ToVertex(loadResult)).ToArray();
            }
        }

        private class LightClawMaterialResolver : IMaterialStreamProvider
        {
            private readonly IContentManager contentManager;

            public LightClawMaterialResolver(IContentManager manager)
            {
                Contract.Requires<ArgumentNullException>(manager != null);

                this.contentManager = manager;
            }

            public Stream Open(string materialFilePath)
            {
                return this.contentManager.GetStreamAsync(materialFilePath).Result;
            }
        }

        private struct VertexIndexGroup : IEquatable<VertexIndexGroup>
        {
            public int VertexIndex;

            public int NormalsIndex;

            public int UVIndex;

            public VertexIndexGroup(int vertexIndex, int normalIndex, int uvIndex)
            {
                this.VertexIndex = vertexIndex;
                this.NormalsIndex = normalIndex;
                this.UVIndex = uvIndex;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                    return false;

                return (obj is VertexIndexGroup) ? this.Equals((VertexIndexGroup)obj) : false;
            }

            public bool Equals(VertexIndexGroup other)
            {
                return (this.VertexIndex == other.VertexIndex) && (this.NormalsIndex == other.NormalsIndex) &&
                       (this.UVIndex == other.UVIndex);
            }

            public override int GetHashCode()
            {
                return HashF.GetHashCode(this.VertexIndex, this.NormalsIndex, this.UVIndex);
            }

            public Vertex ToVertex(LoadResult loadResult)
            {
                Contract.Requires<ArgumentNullException>(loadResult != null);

                ObjLoader.Loader.Data.VertexData.Vertex position = loadResult.Vertices.ElementAtOrDefault(this.VertexIndex);
                ObjLoader.Loader.Data.VertexData.Normal normal = loadResult.Normals.ElementAtOrDefault(this.NormalsIndex);
                ObjLoader.Loader.Data.VertexData.Texture uv = loadResult.Textures.ElementAtOrDefault(this.UVIndex);

                return new Vertex(
                    new Vector3(position.X, position.Y, position.Z),
                    new Vector3(normal.X, normal.Y, normal.Z),
                    new Vector2(uv.X, uv.Y),
                    new Color(position.X, position.Y, position.Z)
                );
            }
        }
    }
}
