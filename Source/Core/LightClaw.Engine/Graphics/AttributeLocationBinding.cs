using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [ProtoContract]
    public struct AttributeLocationBinding
    {
        [ProtoMember(1)]
        public int Index { get; private set; }

        [ProtoMember(2)]
        public string Name { get; private set; }

        public AttributeLocationBinding(int index, string name)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentNullException>(name != null);

            this.Index = index;
            this.Name = name;
        }
    }
}
