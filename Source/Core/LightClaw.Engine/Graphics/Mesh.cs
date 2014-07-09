﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public class Mesh<TVertex> : Component, IDrawable
        where TVertex : struct
    {
        public IEnumerable<MeshPart<TVertex>> Parts { get; private set; }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        protected override void OnEnable()
        {
            throw new NotImplementedException();
        }

        protected override void OnDisable()
        {
            throw new NotImplementedException();
        }

        protected override void OnLoad()
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
