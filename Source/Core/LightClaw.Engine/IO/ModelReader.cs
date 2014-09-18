using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL4;

using LCBuffer = LightClaw.Engine.Graphics.OpenGL.Buffer;

namespace LightClaw.Engine.IO
{
    public class ModelReader : IContentReader
    {
        public Task<object> ReadAsync(IContentManager contentManager, ResourceString resourceString, Stream assetStream, Type assetType, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
