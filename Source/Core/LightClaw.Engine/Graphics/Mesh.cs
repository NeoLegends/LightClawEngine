using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public class Mesh : Component, IEnumerable<MeshPart>
    {
        private bool isLoaded = false;

        [DataMember]
        public string MeshFormat { get; private set; }

        [IgnoreDataMember]
        public MeshPartCollection Parts { get; private set; }

        [DataMember]
        public string ResourceString { get; private set; }

        private Mesh() { }

        public Mesh(string resourceString, string meshFormat)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(meshFormat));

            this.MeshFormat = meshFormat;
            this.ResourceString = resourceString;
        }

        public IEnumerator<MeshPart> GetEnumerator()
        {
            return this.Parts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected override void OnDraw()
        {
            if (this.isLoaded)
            {
                this.Parts.Draw();
            }
        }

        protected override void OnLoad()
        {
            this.IocC.Resolve<IContentManager>()
                     .LoadAsync<MeshPartCollection>(this.ResourceString, this.MeshFormat)
                     .ContinueWith(t => 
                     {
                         this.Parts = t.Result;
                         this.isLoaded = true;
                     }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
