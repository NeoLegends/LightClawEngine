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
        private TextureUnit textureUnit;

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
        /// Initializes a new <see cref="Sampler"/>.
        /// </summary>
        /// <param name="textureUnit">The <see cref="TextureUnit"/> to bind the <see cref="Sampler"/> to.</param>
        /// <param name="parameters">Sampler parameters.</param>
        public Sampler(IEnumerable<SamplerParameterDescription> parameters)
        {
            Contract.Requires<ArgumentNullException>(parameters != null);

            this.VerifyAccess();

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
        public Binding Bind(TextureUnit textureUnit)
        {
            this.VerifyAccess();

            this.textureUnit = textureUnit;
            GL.BindSampler(textureUnit, this);
            return new Binding(this);
        }

        /// <summary>
        /// Unbinds the <see cref="Sampler"/> from the <see cref="P:TextureUnit"/>.
        /// </summary>
        public void Unbind()
        {
            this.VerifyAccess();
            GL.BindSampler(this.textureUnit, 0);
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
                Log.Warn(
                    ex, 
                    "A {0} was thrown while disposing of a {1}. In most cases, this should be nothing to worry about. Check the error message to make sure there really is nothing to worry about, though.", 
                    ex.GetType().Name, typeof(Sampler).Name
                );
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
            Contract.Invariant(this.textureUnit >= 0);
        }
    }
}
