using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Input
{
    [DataContract]
    [Solitary(typeof(InputInterface), "More than one InputInterface induces unnecessary overhead.")]
    public class InputInterface : Component
    {
        public InputInterface()
        {
            throw new NotImplementedException();
        }
    }
}
