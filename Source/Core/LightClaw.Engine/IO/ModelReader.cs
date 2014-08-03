using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

using GLBuffer = LightClaw.Engine.Graphics.Buffer;

namespace LightClaw.Engine.IO
{
    public class ModelReader : IContentReader
    {
        public async Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            ModelData modelData = await Task.Run(() => Serializer.Deserialize<ModelData>(assetStream));

            Model model = new Model();
            foreach (ModelMeshData modelMeshData in modelData.ModelMeshes)
            {
                ModelMesh modelMesh = new ModelMesh();
                foreach (ModelMeshPartData modelMeshPartData in modelMeshData.ModelMeshParts)
                {
                    modelMesh.ModelMeshParts.Add(
                        new ModelMeshPart(
                            await contentManager.LoadAsync<Material>(modelMeshPartData.MaterialResourceString),
                            new VertexArrayObject(
                                modelMeshPartData.VertexData.Select(vData => 
                                    new BufferDescription(
                                        GLBuffer.Create(
                                            vData.VertexData, 
                                            BufferTarget.ArrayBuffer
                                        ), 
                                        vData.VertexAttributePointers
                                    )
                                ),
                                GLBuffer.Create(modelMeshPartData.IndexData, BufferTarget.ElementArrayBuffer)
                            )
                        )
                    );
                }
                model.ModelMeshes.Add(modelMesh);
            }
            return model;
        }

        [ProtoContract]
        private struct ModelData
        {
            [ProtoMember(1)]
            public ModelMeshData[] ModelMeshes { get; private set; }

            public ModelData(IEnumerable<ModelMeshData> modelMeshes)
                : this()
            {
                Contract.Requires<ArgumentNullException>(modelMeshes != null);

                this.ModelMeshes = modelMeshes.ToArray();
            }
        }

        [ProtoContract]
        private struct ModelMeshData
        {
            [ProtoMember(1)]
            public ModelMeshPartData[] ModelMeshParts { get; private set; }

            public ModelMeshData(ModelMeshPartData[] modelMeshParts)
                : this()
            {
                Contract.Requires<ArgumentNullException>(modelMeshParts != null);

                this.ModelMeshParts = modelMeshParts;
            }
        }

        [ProtoContract]
        private struct ModelMeshPartData
        {
            [ProtoMember(1)]
            public uint[] IndexData { get; private set; }

            [ProtoMember(2)]
            public string MaterialResourceString { get; private set; }

            [ProtoMember(3)]
            public VertexDataDescription[] VertexData { get; private set; }

            public ModelMeshPartData(string materialResourceString, uint[] indexData, VertexDataDescription[] vertexData)
                : this()
            {
                Contract.Requires<ArgumentNullException>(materialResourceString != null);
                Contract.Requires<ArgumentNullException>(indexData != null);
                Contract.Requires<ArgumentNullException>(vertexData != null);

                this.IndexData = indexData;
                this.MaterialResourceString = materialResourceString;
                this.VertexData = vertexData;
            }
        }

        [ProtoContract]
        private struct VertexDataDescription
        {
            [ProtoMember(1)]
            public VertexAttributePointer[] VertexAttributePointers { get; private set; }

            [ProtoMember(2)]
            public float[] VertexData { get; private set; }

            public VertexDataDescription(VertexAttributePointer[] vertexAttributePointers, byte[] vertexData)
                : this()
            {
                Contract.Requires<ArgumentNullException>(vertexAttributePointers != null);
                Contract.Requires<ArgumentNullException>(vertexData != null);

                this.VertexAttributePointers = vertexAttributePointers;
                this.VertexData = VertexData;
            }
        }
    }
}
