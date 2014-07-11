using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [ProtoContract(IgnoreListHandling = true)]
    public class Mesh<TVertex> : Component, IDrawable, IEnumerable<MeshPart<TVertex>>
        where TVertex : struct
    {
        [ProtoMember(1)]
        public string MeshFormat { get; private set; }

        [ProtoIgnore]
        public IEnumerable<MeshPart<TVertex>> Parts { get; private set; }

        [ProtoMember(2)]
        public string ResourceString { get; private set; }

        private Mesh() { }

        public Mesh(string resourceString, string meshFormat)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(meshFormat));

            this.MeshFormat = meshFormat;
            this.ResourceString = resourceString;
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<MeshPart<TVertex>> GetEnumerator()
        {
            return this.Parts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected override void OnEnable()
        {
            throw new NotImplementedException();
        }

        protected override void OnDisable()
        {
            throw new NotImplementedException();
        }

        protected override async void OnLoad()
        {
            this.Parts = await this.Ioc.Resolve<IContentManager>().LoadAsync<IEnumerable<MeshPart<TVertex>>>(this.ResourceString, this.MeshFormat);
        }

        protected override void OnUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
