﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    [ProtoContract]
    public abstract class Component : Manager
    {
        [ProtoMember(1)]
        public int UpdatePriority { get; protected set; }
    }
}
