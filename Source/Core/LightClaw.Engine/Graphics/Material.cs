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
    /// <summary>
    /// Represents a material, an interface from the game and drawing code to the shader.
    /// </summary>
    [DataContract]
    public abstract class Material : Entity, IBindable, IUpdateable, ILateUpdateable
    {
        /// <summary>
        /// Notifies about changes in the component the material works for.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Component>> ComponentChanged;

        /// <summary>
        /// Notifies about the start of the updating process.
        /// </summary>
        /// <remarks>Raised before any updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> Updating;

        /// <summary>
        /// Notifies about the finsih of the updating process.
        /// </summary>
        /// <remarks>Raised after any updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> Updated;

        /// <summary>
        /// Notifies about the start of the late updating process.
        /// </summary>
        /// <remarks>Raised before any late updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> LateUpdating;

        /// <summary>
        /// Notifies about the finsih of the late updating process.
        /// </summary>
        /// <remarks>Raised after any late updating operations.</remarks>
        public event EventHandler<ParameterEventArgs> LateUpdated;

        /// <summary>
        /// Notifies about changes in the <see cref="Shader"/> the material uses.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<Shader>> ShaderChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private Component _Component;

        /// <summary>
        /// The <see cref="Component"/> the <see cref="Material"/> provides a binding to the shader for.
        /// </summary>
        /// <remarks>This could be a <see cref="Mesh"/>, a <see cref="Texture"/>-renderer-component, really anything that draws.</remarks>
        [IgnoreDataMember]
        public Component Component
        {
            get
            {
                return _Component;
            }
            internal set
            {
                Component previous = this.Component;
                this.SetProperty(ref _Component, value);
                this.Raise(this.ComponentChanged, value, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private Shader _Shader;

        /// <summary>
        /// The <see cref="Shader"/> the <see cref="Material"/> interfaces with.
        /// </summary>
        [IgnoreDataMember]
        public Shader Shader
        {
            get
            {
                return _Shader;
            }
            set
            {
                Shader previous = this.Shader;
                this.SetProperty(ref _Shader, value);
                this.Raise(this.ShaderChanged, value, previous);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private string _ShaderResourceString;

        /// <summary>
        /// The resource string of the shader.
        /// </summary>
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

        /// <summary>
        /// Binds the material to the graphics pipeline.
        /// </summary>
        /// <remarks>Do not bind the shader. It will be bound by an upper level to reduce shader switches.</remarks>
        public abstract void Bind(); 

        /// <summary>
        /// Unbinds the <see cref="Material"/> from the graphics pipeline.
        /// </summary>
        public abstract void Unbind();

        /// <summary>
        /// Updates the <see cref="Material"/> with the current <see cref="GameTime"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        public void Update(GameTime gameTime)
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
            {
                this.OnUpdate(gameTime);
            }
        }

        /// <summary>
        /// Late-updates the <see cref="Material"/>.
        /// </summary>
        public void LateUpdate()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
            {
                this.OnLateUpdate();
            }
        }

        /// <summary>
        /// Gets the <see cref="P:Shader"/> in a strongly-typed way.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to cast the property to.</typeparam>
        /// <returns>The <see cref="Shader"/> as strongly typed object or <c>null</c> if the shader is not of the requested <see cref="Type"/>.</returns>
        protected T GetShader<T>()
            where T : Shader
        {
            return this.Shader as T;
        }

        /// <summary>
        /// Update-callback.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        protected abstract void OnUpdate(GameTime gameTime);

        /// <summary>
        /// Late-update callback.
        /// </summary>
        protected abstract void OnLateUpdate();

        /// <summary>
        /// Called after deserialization of the <see cref="Material"/>, loads the <see cref="Shader"/> from the resource string.
        /// </summary>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
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
                        logger.Debug(() => "Shader '{0}' loaded successfully into material.".FormatWith(shaderResourceString));
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously
                );
                shaderTask.ContinueWith(
                    t => logger.Warn(() => "Shader '{0}' could not be loaded after deserialization.".FormatWith(shaderResourceString), t.Exception),
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
                );
            }
        }
    }
}
