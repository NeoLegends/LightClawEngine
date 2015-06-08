using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using DryIoc;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using NLog;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using AssimpMesh = Assimp.Mesh;
using AssimpScene = Assimp.Scene;

namespace LightClaw.Engine.IO
{
    public class ModelReader : DispatcherEntity, IContentReader
    {
        private static readonly Logger assimpLog = LogManager.GetLogger("assimp");

        private static readonly PostProcessSteps postProcessingSteps =
            PostProcessSteps.CalculateTangentSpace | PostProcessSteps.FindDegenerates |
            PostProcessSteps.FindInvalidData | PostProcessSteps.GenerateSmoothNormals | 
            PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.OptimizeGraph | 
            PostProcessSteps.OptimizeMeshes | PostProcessSteps.RemoveRedundantMaterials | 
            PostProcessSteps.SortByPrimitiveType |  PostProcessSteps.SplitLargeMeshes | 
            PostProcessSteps.Triangulate | PostProcessSteps.ValidateDataStructure;

        private static readonly LogStream logStream = new LogStream(OnLogMessage);

        private readonly AssimpContext context = new AssimpContext();

        private readonly LightClawIOSystem ioSystem;

        public ModelReader()
        {
            this.context.SetConfig(new NormalSmoothingAngleConfig(80.0f));
            this.context.SetConfig(new MeasureTimeConfig(true));
            this.context.SetConfig(new MeshVertexLimitConfig(ushort.MaxValue));
            this.context.SetConfig(new RemoveDegeneratePrimitivesConfig(true));

            this.context.SetIOSystem(this.ioSystem = new LightClawIOSystem(this.IocC.Resolve<IContentManager>()));
        }

        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(Model));
        }

        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            Func<AssimpScene> f = () =>
            {
                parameters.CancellationToken.ThrowIfCancellationRequested();
                return this.context.ImportFileFromStream(
                   parameters.AssetStream,
                   postProcessingSteps,
                   Path.GetExtension(parameters.ResourceString)
                );
            };
            Task<Effect> shaderLoadTask = parameters.ContentManager.LoadAsync<Effect>("Shaders/Basic.shr", parameters.CancellationToken);
            AssimpScene s = this.Dispatcher.IsOnThread ? await Task.Run(f).ConfigureAwait(false) : f(); // Make sure we don't pollute the main thread with IO
            Effect effect = await shaderLoadTask.ConfigureAwait(false);

            Dictionary<AssimpMesh, VertexArrayObject> vaos = new Dictionary<AssimpMesh, VertexArrayObject>();
            await this.Dispatcher.ImmediateOr(() =>
            {
                foreach (AssimpMesh mesh in s.Meshes)
                {
                    if (!mesh.HasVertices)
                    {
                        this.Log.Warn(() => string.Format("Mesh {0} does not have vertex data! It will not be loaded.", mesh.Name));
                        continue;
                    }

                    BufferObject indexBuffer = BufferObject.Create(mesh.GetIndicesUInt16(), BufferTarget.ElementArrayBuffer);

                    BufferObject normalBuffer = BufferObject.Create(mesh.Normals.ToArray(), BufferTarget.ArrayBuffer);
                    BufferObject texCoordBuffer = mesh.HasTextureCoords(0) ?
                        BufferObject.Create(mesh.TextureCoordinateChannels[0].ToArray(), BufferTarget.ArrayBuffer) :
                        null;
                    BufferObject vertexBuffer = BufferObject.Create(mesh.Vertices.ToArray(), BufferTarget.ArrayBuffer);

                    vaos.Add(
                        mesh,
                        new VertexArrayObject(
                            indexBuffer,
                            new BufferDescription(
                                normalBuffer,
                                new VertexAttributePointer(VertexAttributeLocation.Normals, 3, VertexAttribPointerType.Float, false, 0, 0)
                            ),
                            new BufferDescription(
                                texCoordBuffer,
                                new VertexAttributePointer(VertexAttributeLocation.TexCoords, 2, VertexAttribPointerType.Float, false, 0, 0)
                            ),
                            new BufferDescription(
                                vertexBuffer,
                                new VertexAttributePointer(VertexAttributeLocation.Position, 3, VertexAttribPointerType.Float, false, 0, 0)
                            )
                        )
                    );
                }
            }, DispatcherPriority.Normal).ConfigureAwait(false);

            IEnumerable<ModelPart> modelParts = await Task.WhenAll(vaos.Select(async kvp =>
            {
                Material mat = s.Materials[kvp.Key.MaterialIndex];
                
                Texture2D diffuse= await parameters.ContentManager.LoadAsync<Texture2D>(
                    mat.HasTextureDiffuse ? 
                        Path.Combine(Path.GetDirectoryName(parameters.ResourceString), s.Materials[kvp.Key.MaterialIndex].TextureDiffuse.FilePath) :
                        "Internals/Default.png",
                    parameters.CancellationToken
                ).ConfigureAwait(false);
                
                return new LightClawModelPart(effect, kvp.Value, diffuse, false, true, false);
            }));

            return new Model(modelParts, true);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                this.context.Dispose();
                this.ioSystem.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private static void OnLogMessage(string msg, string userData)
        {
            assimpLog.Info(msg);
        }
    }

    internal static class MeshExtensions
    {
        public static ushort[] GetIndicesUInt16(this Assimp.Mesh m)
        {
            Contract.Requires<ArgumentNullException>(m != null);

            if (m.HasFaces)
            {
                List<ushort> indices = new List<ushort>();
                for (int i = 0; i < m.Faces.Count; i++)
                {
                    Face f = m.Faces[i];
                    if (f.HasIndices)
                    {
                        for (int j = 0; j < f.IndexCount; j++)
                        {
                            indices.Add((ushort)f.Indices[j]);
                        }
                    }
                }

                return indices.ToArray();
            }
            else
            {
                return null;
            }
        }
    }
}
