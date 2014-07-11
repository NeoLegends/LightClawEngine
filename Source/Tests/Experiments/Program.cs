using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Coroutines;
using ProtoBuf;

namespace Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, new OuterList());
            }
        }

        [ProtoContract]
        public class OuterList
        {
            [ProtoMember(1)]
            public List<InnerList> InnerList = new List<InnerList>();
        }

        [ProtoContract(IgnoreListHandling = true)]
        public class InnerList : IEnumerable<int>
        {
            [ProtoMember(1)]
            public List<int> innerList = new List<int>();

            public IEnumerator<int> GetEnumerator()
            {
                return this.innerList.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
