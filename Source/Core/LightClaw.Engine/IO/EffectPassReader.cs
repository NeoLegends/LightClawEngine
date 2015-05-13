using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.IO
{
    public class EffectPassReader : DispatcherEntity, IContentReader
    {
        private static readonly JsonSerializer serializer = JsonSerializer.CreateDefault();

        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(EffectPass));
        }

        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            EffectPassDescription desc;
            using (StreamReader sr = new StreamReader(parameters.AssetStream, Encoding.UTF8, true, 512, true))
            using (JsonTextReader jtr = new JsonTextReader(sr))
            {
                desc = serializer.Deserialize<EffectPassDescription>(jtr);
            }

            if (string.IsNullOrWhiteSpace(desc.VertexShader))
            {
                throw new InvalidOperationException("The vertex shader resource string was null or whitespace!");
            }
            if (string.IsNullOrWhiteSpace(desc.FragmentShader))
            {
                throw new InvalidOperationException("The fragment shader resource string was null or whitespace!");
            }

            IContentManager mgr = parameters.ContentManager;
            CancellationToken token = parameters.CancellationToken;
            IEnumerable<Shader> shaders = (await Task.WhenAll(
                TryLoadAsync(desc.VertexShader, ShaderType.VertexShader, mgr, token),
                TryLoadAsync(desc.TessControlShader, ShaderType.TessControlShader, mgr, token),
                TryLoadAsync(desc.TessEvalShader, ShaderType.TessEvaluationShader, mgr, token),
                TryLoadAsync(desc.GeometryShader, ShaderType.GeometryShader, mgr, token),
                TryLoadAsync(desc.FragmentShader, ShaderType.FragmentShader, mgr, token)
            ).ConfigureAwait(false)).FilterNull();

            ShaderProgram program = await this.Dispatcher.Invoke(() => new ShaderProgram(shaders.ToArray()), DispatcherPriority.Normal, token)
                                                         .ConfigureAwait(false);
            return new EffectPass(program, true);
        }

        private Task<Shader> TryLoadAsync(ResourceString rs, ShaderType type, IContentManager contentManager, CancellationToken token)
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);

            return rs.IsValid ? contentManager.LoadAsync<Shader>(rs, type, token) : Task.FromResult<Shader>(null);
        }
    }
}
