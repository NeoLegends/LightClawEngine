using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents an OpenGL sampler, an object that stores texture sampling parameters.
    /// </summary>
    /// <seealso href="http://www.opengl.org/wiki/Sampler_Object"/>
    [DebuggerDisplay("Texture Unit: {TextureUnit}, Parameter Count: {Parameters.Length}")]
    public class Sampler : GLObject, IBindable
    {
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

            this.VerifyAccess();

            this.TextureUnit = textureUnit;
            this.Parameters = parameters.ToImmutableArray();

            this.Handle = GL.GenSampler();
            foreach (SamplerParameterDescription description in this.Parameters.EnsureNonNull())
            {
                GL.SamplerParameter(this, description.ParameterName, description.Value);
            }
        }

        /// <summary>
        /// Binds the <see cref="Sampler"/> to the <see cref="P:TextureUnit"/>.
        /// </summary>
        public Binding Bind()
        {
            this.VerifyAccess();
            GL.BindSampler(this.TextureUnit, this);
            return new Binding(this);
        }

        /// <summary>
        /// Unbinds the <see cref="Sampler"/> from the <see cref="P:TextureUnit"/>.
        /// </summary>
        public void Unbind()
        {
            this.VerifyAccess();
            GL.BindSampler(this.TextureUnit, 0);
        }

        /// <summary>
        /// Disposes the <see cref="Sampler"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether to release managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            this.Dispatcher.ImmediateOr(this.DeleteSampler, disposing, DispatcherPriority.Background);
        }

        [System.Security.SecurityCritical]
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private void DeleteSampler(bool disposing)
        {
            try
            {
                GL.DeleteSampler(this);
            }
            catch (Exception ex)
            {
                Log.Warn("An {0} was thrown while disposing of a {1}. This might or might not be an unwanted condition.".FormatWith(ex.GetType().Name, typeof(Sampler).Name), ex);
            }
            finally
            {
                base.Dispose(disposing);
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
