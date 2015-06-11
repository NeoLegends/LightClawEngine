using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using LightClaw.Engine.Configuration;
using LightClaw.Engine.Core;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using NLog;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents an OpenGL texture.
    /// </summary>
    [DebuggerDisplay("Size: {Width}x{Height}x{Depth}, Target: {Target}, PixelInternalFormat: {PixelInternalFormat}")]
    public abstract class Texture : GLObject, IBindable
    {
        private static readonly TextureParameterName anisoParameterName = (TextureParameterName)OpenTK.Graphics.OpenGL.ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt;

        private static readonly int linearMipmapLinear = (int)All.LinearMipmapLinear;

        private static readonly int linear = (int)All.Linear;

        private static readonly Logger staticLogger = LogManager.GetLogger(typeof(Texture).Name);

        private TextureUnit textureUnit;

        private TextureDescription _Description;

        /// <summary>
        /// The <see cref="TextureDescription"/> describing the pixel format, width, height, etc.
        /// </summary>
        public TextureDescription Description
        {
            get
            {
                Contract.Ensures(Contract.Result<TextureDescription>() != null);

                return _Description;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Description, value);
            }
        }

        /// <summary>
        /// The amount of mipmap levels.
        /// </summary>
        public int MipmapLevels
        {
            get
            {
                return this.Description.Levels;
            }
        }

        /// <summary>
        /// The amount of multisampling levels.
        /// </summary>
        public int MultisamplingLevels
        {
            get
            {
                return this.Description.MultisamplingLevels;
            }
        }

        /// <summary>
        /// The format of the pixels.
        /// </summary>
        public PixelInternalFormat PixelInternalFormat
        {
            get
            {
                return this.Description.PixelInternalFormat;
            }
        }

        /// <summary>
        /// The <see cref="TextureTarget"/> the <see cref="Texture"/> will be bound to.
        /// </summary>
        public TextureTarget Target
        {
            get
            {
                return this.Description.Target;
            }
        }

        /// <summary>
        /// The <see cref="Texture"/>s width.
        /// </summary>
        public int Width
        {
            get
            {
                return this.Description.Width;
            }
        }

        /// <summary>
        /// The <see cref="Texture"/>s height.
        /// </summary>
        public int Height
        {
            get
            {
                return this.Description.Height;
            }
        }

        /// <summary>
        /// The <see cref="Texture"/>s depth.
        /// </summary>
        public int Depth
        {
            get
            {
                return this.Description.Depth;
            }
        }

        static Texture() // Assume GLContext is present
        {
            // Not sure whether bug still exists, but on
            // http://www.opengl.org/wiki/Common_Mistakes#Automatic_mipmap_generation it states that ATI cards had
            // problems with automatic mipmap generation if Texture2D wasn't enabled. But since I own an NVIDIA card, I
            // can't test that.

            GL.Enable(EnableCap.Texture1D);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.TextureCubeMap);
            GL.Enable(EnableCap.TextureCubeMapSeamless);
        }

        /// <summary>
        /// Initializes a new <see cref="Texture"/> from the specified <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The <see cref="TextureDescription"/> to initialize from.</param>
        protected Texture(TextureDescription description)
        {
            Contract.Requires<ArgumentNullException>(description != null);
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(TextureTarget), description.Target));

            this.Description = description;

            this.VerifyAccess();
            using (this.Bind(0))
            {
                // Set anisotropic filtering level and min and mag filter
                int linearMipmapLinear = Texture.linearMipmapLinear;
                int linear = Texture.linear;
                GL.TexParameterI(this.Target, TextureParameterName.TextureMinFilter, ref linearMipmapLinear);
                GL.TexParameterI(this.Target, TextureParameterName.TextureMagFilter, ref linear);

                if (VideoSettings.Default.AnisotropicFiltering && SupportsExtension("GL_EXT_texture_filter_anisotropic"))
                {
                    // User might want higher aniso level than the h/w supports.
                    float requestedAnisoLevel = VideoSettings.Default.AnisotropicLevel;
                    float maxSupportedAnisoLevel = GL.GetFloat((GetPName)OpenTK.Graphics.OpenGL.ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt);

                    GL.TexParameter(this.Target, anisoParameterName, Math.Min(requestedAnisoLevel, maxSupportedAnisoLevel));
                }

                GL.GenerateMipmap((GenerateMipmapTarget)this.Target);
            }
        }

        /// <summary>
        /// Binds the <see cref="Texture"/> to the specified <paramref name="textureUnit"/>.
        /// </summary>
        /// <param name="textureUnit">The <see cref="TextureUnit"/> to bind to.</param>
        public virtual Binding Bind(TextureUnit textureUnit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            this.VerifyAccess();

            this.textureUnit = textureUnit;
            OpenTK.Graphics.OpenGL4.TextureUnit unit = textureUnit;
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(this.Target, this);
            return new Binding(this);
        }

        /// <summary>
        /// Unbinds the <see cref="Texture"/> from the <see cref="P:TextureUnit"/>.
        /// </summary>
        public void Unbind()
        {
            this.Unbind(this.textureUnit);
        }

        /// <summary>
        /// Unbinds the <see cref="Texture"/> from the specified <paramref name="textureUnit"/>.
        /// </summary>
        /// <param name="textureUnit">The <see cref="TextureUnit"/> to unbind from.</param>
        public virtual void Unbind(TextureUnit textureUnit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            this.VerifyAccess();

            GL.ActiveTexture(textureUnit);
            GL.BindTexture(this.Target, 0);
        }

        /// <summary>
        /// Disposes the <see cref="Texture"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            this.Dispatcher.ImmediateOr(this.DeleteTexture, disposing, DispatcherPriority.Background);
        }

        [System.Security.SecurityCritical]
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private void DeleteTexture(bool disposing)
        {
            try
            {
                GL.DeleteTexture(this);
            }
            catch (Exception ex)
            {
                Log.Warn("An {0} was thrown while disposing of a {1}. This might or might not be an unwanted condition.".FormatWith(ex.GetType().Name, typeof(ShaderProgram).Name), ex);
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Description != null);
        }
    }
}
