using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [ProtoContract]
    public class MeshData : Entity
    {
        private const string NormalsEntryFileName = "Normals";

        private const string TextureCoordinatesFileName = "TextureCoordinates";

        private const string VerticesFileName = "Vertices";

        [ProtoMember(1)]
        public Vector3[] Normals { get; private set; }

        [ProtoMember(2)]
        public Vector2[] TextureCoordinates { get; private set; }

        [ProtoMember(3)]
        public Vector3[] Vertices { get; private set; }

        public MeshData(Vector3[] vertices, Vector3[] normals, Vector2[] texCoords)
        {
            Contract.Requires<ArgumentNullException>(vertices != null);
            Contract.Requires<ArgumentNullException>(normals != null);
            Contract.Requires<ArgumentNullException>(texCoords != null);

            this.Vertices = vertices;
            this.Normals = normals;
            this.TextureCoordinates = texCoords;
        }

        public async Task Save(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            await this.Save(await this.IocC.Resolve<IContentManager>().GetStreamAsync(resourceString));
        }

        public Task Save(Stream destination)
        {
            Contract.Requires<ArgumentNullException>(destination != null);
            Contract.Requires<ArgumentException>(destination.CanWrite);

            return Task.Run(() => Serializer.Serialize(destination, this));
        }

        public static async Task<MeshData> Load(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(resourceString != null);

            return await Load(await LightClawEngine.DefaultIocContainer.Resolve<IContentManager>().GetStreamAsync(resourceString));
        }

        public static Task<MeshData> Load(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentException>(s.CanRead);

            return Task.Run(() => Serializer.Deserialize<MeshData>(s));
        }
    }
}
