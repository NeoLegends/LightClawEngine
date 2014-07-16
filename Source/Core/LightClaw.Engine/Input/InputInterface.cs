using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Input;
using ProtoBuf;

namespace LightClaw.Engine.Input
{
    [ProtoContract]
    [Solitary(typeof(InputInterface), "More than one InputInterface induces unecessary overhead.")]
    public class InputInterface : Component
    {
        public InputInterface()
        {

        }


    }
}
