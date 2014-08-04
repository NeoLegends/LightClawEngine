using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.IO;
using LightClaw.Extensions;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public abstract class Material : Entity, IBindable, IUpdateable, ILateUpdateable
    {
        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        private Shader _Shader;

        [IgnoreDataMember]
        public Shader Shader
        {
            get
            {
                return _Shader;
            }
            set
            {
                this.SetProperty(ref _Shader, value);
            }
        }

        private string _ShaderResourceString;

        [DataMember]
        public string ShaderResourceString
        {
            get
            {
                return _ShaderResourceString;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _ShaderResourceString, value);
            }
        }

        public abstract void Bind(); // DO NOT (!) bind shader, will be bound by model to reduce shader switches

        public abstract void Unbind();

        public void Update(GameTime gameTime)
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
            {
                this.OnUpdate(gameTime);
            }
        }

        public void LateUpdate()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
            {
                this.OnLateUpdate();
            }
        }

        protected T GetShader<T>()
            where T : Shader
        {
            return (T)this.Shader;
        }

        protected abstract void OnUpdate(GameTime gameTime);

        protected abstract void OnLateUpdate();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            string shaderResourceString = this.ShaderResourceString;
            if (shaderResourceString != null)
            {
                Task<Shader> shaderTask = this.IocC.Resolve<IContentManager>().LoadAsync<Shader>(shaderResourceString);
                shaderTask.ContinueWith(
                    t =>
                    {
                        this.Shader = t.Result;
                        logger.Debug("Shader '{0}' loaded successfully into material.".FormatWith(shaderResourceString));
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously
                );
                shaderTask.ContinueWith(
                    t => logger.Warn("Shader '{0}' could not be loaded after deserialization.".FormatWith(shaderResourceString), t.Exception),
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
                );
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.ShaderResourceString != null);
        }
    }
}
