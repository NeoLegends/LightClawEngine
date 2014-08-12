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
        public Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            throw new NotImplementedException();
            //ModelData modelData = await Task.Run(() => Serializer.Deserialize<ModelData>(assetStream));

            //Model model = new Model();
            //foreach (ModelPartData modelPartData in modelData.ModelParts)
            //{
            //    model.ModelParts.Add(
            //        new ModelPart(
            //            await contentManager.LoadAsync<Material>(modelPartData.MaterialResourceString),
            //            new VertexArrayObject(
            //                modelPartData.VertexData.Select(vData => 
            //                    new BufferDescription(
            //                        GLBuffer.Create(
            //                            vData.VertexData, 
            //                            BufferTarget.ArrayBuffer
            //                        ), 
            //                        vData.VertexAttributePointers
            //                    )
            //                ),
            //                GLBuffer.Create(modelPartData.IndexData, BufferTarget.ElementArrayBuffer)
            //            )
            //        )
            //    );
            //}
            //return model;
        }

        [ProtoContract]
        public struct ModelData
        {
            [ProtoMember(1)]
            public ModelPartData[] ModelParts { get; private set; }

            public ModelData(IEnumerable<ModelPartData> modelParts)
                : this()
            {
                Contract.Requires<ArgumentNullException>(modelParts != null);

                this.ModelParts = modelParts.ToArray();
            }
        }

        [ProtoContract]
        public struct ModelPartData
        {
            [ProtoMember(1)]
            public uint[] IndexData { get; private set; }

            [ProtoMember(2)]
            public string MaterialResourceString { get; private set; }

            [ProtoMember(3)]
            public VertexDataDescription[] VertexData { get; private set; }

            public ModelPartData(string materialResourceString, uint[] indexData, VertexDataDescription[] vertexData)
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
        public struct VertexDataDescription
        {
            [ProtoMember(1)]
            public VertexAttributePointer[] VertexAttributePointers { get; private set; }

            [ProtoMember(2)]
            public float[] VertexData { get; private set; }

            public VertexDataDescription(VertexAttributePointer[] vertexAttributePointers, float[] vertexData)
                : this()
            {
                Contract.Requires<ArgumentNullException>(vertexAttributePointers != null);
                Contract.Requires<ArgumentNullException>(vertexData != null);

                this.VertexAttributePointers = vertexAttributePointers;
                this.VertexData = vertexData;
            }
        }
    }
}
