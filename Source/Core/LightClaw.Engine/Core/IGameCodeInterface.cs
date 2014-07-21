﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf.Meta;

namespace LightClaw.Engine.Core
{
    public interface IGameCodeInterface
    {
        IEnumerable<Type> GetComponents();
    }
}
