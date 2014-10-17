﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents an OpenGL sampler, an object that stores texture sampling parameters.
    /// </summary>
    /// <seealso href="http://www.opengl.org/wiki/Sampler_Object"/>
    [DebuggerDisplay("Handle = {Handle}, Texture Unit = {TextureUnit}, Parameter Count = {Parameters.Length}")]
    public class Sampler : GLObject, IBindable
    {
        /// <summary>
        /// Used for access restriction to the initialization method.
        /// </summary>
        private readonly object initializationLock = new object();

        /// <summary>
        /// Backing field.
        /// </summary>
        private ImmutableArray<SamplerParameterDescription> _Parameters;

        /// <summary>
        /// Stores all set sampler parameters.
        /// </summary>
        public ImmutableArray<SamplerParameterDescription> Parameters
        {
            get
            {
                return _Parameters;
            }
            private set
            {
                this.SetProperty(ref _Parameters, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private TextureUnit _TextureUnit;

        /// <summary>
        /// The <see cref="TextureUnit"/> the <see cref="Sampler"/> will be bound to.
        /// </summary>
        public TextureUnit TextureUnit
        {
            get
            {
                Contract.Ensures(Contract.Result<TextureUnit>() >= 0);

                return _TextureUnit;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _TextureUnit, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Sampler"/>.
        /// </summary>
        /// <param name="textureUnit">The <see cref="TextureUnit"/> to bind the <see cref="Sampler"/> to.</param>
        /// <param name="parameters">Sampler parameters.</param>
        public Sampler(TextureUnit textureUnit, IEnumerable<SamplerParameterDescription> parameters)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);
            Contract.Requires<ArgumentNullException>(parameters != null);

            this.TextureUnit = textureUnit;
            this.Parameters = parameters.ToImmutableArray();
        }

        /// <summary>
        /// Binds the <see cref="Sampler"/> to the <see cref="P:TextureUnit"/>.
        /// </summary>
        public void Bind()
        {
            this.Initialize();
            GL.BindSampler(this.TextureUnit, this);
        }

        /// <summary>
        /// Unbinds the <see cref="Sampler"/> from the <see cref="P:TextureUnit"/>.
        /// </summary>
        public void Unbind()
        {
            GL.BindSampler(this.TextureUnit, 0);
        }

        /// <summary>
        /// Disposes the <see cref="Sampler"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            lock (this.initializationLock)
            {
                if (this.IsInitialized)
                {
                    GL.DeleteSampler(this);
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialization callback.
        /// </summary>
        protected override void OnInitialize()
        {
            this.Handle = GL.GenSampler();
            foreach (SamplerParameterDescription description in this.Parameters.EnsureNonNull())
            {
                GL.SamplerParameter(this, description.ParameterName, description.Value);
            }
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._TextureUnit >= 0);
        }
    }
}
