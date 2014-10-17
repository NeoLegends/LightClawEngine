using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.IO
{
    public class EffectPassReader : IContentReader
    {
        private static readonly JsonSerializer serializer = JsonSerializer.CreateDefault();

        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(EffectPass));
        }

        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            IContentManager contentManager = parameters.ContentManager;

            EffectData data;
            using (StreamReader sr = new StreamReader(parameters.AssetStream))
            using (JsonTextReader jtr = new JsonTextReader(sr))
            {
                data = await Task.Run(() => serializer.Deserialize<EffectData>(jtr));
            }

            string vertexShaderSource = await contentManager.LoadAsync<string>(data.Sources.Vertex.Source, parameters.CancellationToken);
            string fragmentSource = await contentManager.LoadAsync<string>(data.Sources.Fragment.Source, parameters.CancellationToken);
            string tessControlSource = null;
            if (data.Sources.TessControl != null)
            {
                tessControlSource = await contentManager.LoadAsync<string>(data.Sources.TessControl.Source, parameters.CancellationToken);
            }
            string tessEvalSource = null;
            if (data.Sources.TessControl != null)
            {
                tessEvalSource = await contentManager.LoadAsync<string>(data.Sources.TessEval.Source, parameters.CancellationToken);
            }
            string geometrySource = null;
            if (data.Sources.Geometry != null)
            {
                geometrySource = await contentManager.LoadAsync<string>(data.Sources.Geometry.Source, parameters.CancellationToken);
            }

            List<VertexAttributeDescription> descriptions = new List<VertexAttributeDescription>(8);
            if (data.Sources.Vertex.PositionAttribute != null)
            {
                descriptions.Add(new VertexAttributeDescription(data.Sources.Vertex.PositionAttribute, VertexAttributeLocation.Position));
            }
            if (data.Sources.Vertex.TexCoordAttribute != null)
            {
                descriptions.Add(new VertexAttributeDescription(data.Sources.Vertex.TexCoordAttribute, VertexAttributeLocation.TexCoords));
            }
            if (data.Sources.Vertex.NormalAttribute != null)
            {
                descriptions.Add(new VertexAttributeDescription(data.Sources.Vertex.NormalAttribute, VertexAttributeLocation.Normals));
            }
            if (data.Sources.Vertex.BinormalAttribute != null)
            {
                descriptions.Add(new VertexAttributeDescription(data.Sources.Vertex.BinormalAttribute, VertexAttributeLocation.Binormals));
            }
            if (data.Sources.Vertex.TangentAttribute != null)
            {
                descriptions.Add(new VertexAttributeDescription(data.Sources.Vertex.TangentAttribute, VertexAttributeLocation.Tangent));
            }
            if (data.Sources.Vertex.ColorAttribute != null)
            {
                descriptions.Add(new VertexAttributeDescription(data.Sources.Vertex.ColorAttribute, VertexAttributeLocation.Color));
            }

            parameters.CancellationToken.ThrowIfCancellationRequested();
            List<Shader> shaders = new List<Shader>(5)
            {
                new Shader(vertexShaderSource, ShaderType.VertexShader, descriptions.ToArray()),
                new Shader(fragmentSource, ShaderType.FragmentShader)
            };
            if (tessControlSource != null)
            {
                shaders.Add(new Shader(tessControlSource, ShaderType.TessControlShader));
            }
            if (tessEvalSource != null)
            {
                shaders.Add(new Shader(tessEvalSource, ShaderType.TessEvaluationShader));
            }
            if (geometrySource != null)
            {
                shaders.Add(new Shader(geometrySource, ShaderType.GeometryShader));
            }

            return new EffectPass(new ShaderProgram(shaders.ToArray()), true);
        }
    }
}
