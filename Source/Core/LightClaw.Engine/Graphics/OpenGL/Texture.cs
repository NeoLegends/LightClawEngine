using System;
using System.Collections.Generic;
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
    public abstract class Texture : GLObject, IBindable
    {
        private static readonly Logger staticLogger = LogManager.GetLogger(typeof(Texture).Name);

        private TextureDescription _Description;

        /// <summary>
        /// The <see cref="TextureDescription"/> describing the pixel format, width, height, etc.
        /// </summary>
        public TextureDescription Description
        {
            get
            {
                return _Description;
            }
            protected set
            {
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

        private TextureUnit _TextureUnit = TextureUnit.Texture0;

        /// <summary>
        /// The <see cref="TextureUnit"/> the <see cref="Texture"/> will be bound to.
        /// </summary>
        public TextureUnit TextureUnit
        {
            get
            {
                Contract.Ensures(Contract.Result<TextureUnit>() >= 0);

                return _TextureUnit;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _TextureUnit, value);
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
            staticLogger.Info(() => "Initializing OpenGL Texturing system. Enabling textures...");

            GL.Enable(EnableCap.Texture1D);
            // Not sure whether bug still exists, but on
            // http://www.opengl.org/wiki/Common_Mistakes#Automatic_mipmap_generation it states that ATI cards had
            // problems with automatic mipmap generation if Texture2D wasn't enabled. But since I own an NVIDIA card, I
            // can't test that.
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.TextureCubeMap);
            GL.Enable(EnableCap.TextureCubeMapSeamless);

            staticLogger.Info(() => "Textures enabled, configuring mipmap generation...");

            int linearMipmapLinear = (int)All.LinearMipmapLinear;
            int linear = (int)All.Linear;

            GL.TexParameterI(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, ref linearMipmapLinear);
            GL.TexParameterI(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, ref linear);
            GL.TexParameterI(TextureTarget.Texture1DArray, TextureParameterName.TextureMinFilter, ref linearMipmapLinear);
            GL.TexParameterI(TextureTarget.Texture1DArray, TextureParameterName.TextureMagFilter, ref linear);

            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref linearMipmapLinear);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref linear);
            GL.TexParameterI(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, ref linearMipmapLinear);
            GL.TexParameterI(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, ref linear);

            GL.TexParameterI(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, ref linearMipmapLinear);
            GL.TexParameterI(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, ref linear);

            GL.TexParameterI(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, ref linearMipmapLinear);
            GL.TexParameterI(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, ref linear);

            GL.TexParameterI(TextureTarget.TextureRectangle, TextureParameterName.TextureMinFilter, ref linearMipmapLinear);
            GL.TexParameterI(TextureTarget.TextureRectangle, TextureParameterName.TextureMagFilter, ref linear);

            staticLogger.Info(() => "Mipmap generation configured, enabling generation itself...");

            GL.GenerateMipmap(GenerateMipmapTarget.Texture1D);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture1DArray);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DMultisample);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DMultisampleArray);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture3D);
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMapArray);

            staticLogger.Info(() => "Mipmap generation settings set.");

            if (VideoSettings.Default.AnisotropicFiltering && SupportsExtension("GL_EXT_texture_filter_anisotropic"))
            {
                staticLogger.Info(() => "Enabling anisotropic filtering...");

                TextureParameterName anisoParameterName = (TextureParameterName)OpenTK.Graphics.OpenGL.ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt;
                float requestedAnisoLevel = VideoSettings.Default.AnisotropicLevel;
                float maxSupportedAnisoLevel = GL.GetFloat((GetPName)OpenTK.Graphics.OpenGL.ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt);
                float anisoLevel = Math.Min(requestedAnisoLevel, maxSupportedAnisoLevel);

                staticLogger.Info(
                    () => "Anisotropic level will be {0} (maximum supported by hardware: {1}, requested through settings: {2}).".FormatWith(anisoLevel, maxSupportedAnisoLevel, requestedAnisoLevel)
                );

                GL.TexParameter(TextureTarget.Texture1D, anisoParameterName, anisoLevel);
                GL.TexParameter(TextureTarget.Texture1DArray, anisoParameterName, anisoLevel);

                GL.TexParameter(TextureTarget.Texture2D, anisoParameterName, anisoLevel);
                GL.TexParameter(TextureTarget.Texture2DArray, anisoParameterName, anisoLevel);

                GL.TexParameter(TextureTarget.Texture3D, anisoParameterName, anisoLevel);

                GL.TexParameter(TextureTarget.TextureCubeMap, anisoParameterName, anisoLevel);
                GL.TexParameter(TextureTarget.TextureCubeMapArray, anisoParameterName, anisoLevel);

                staticLogger.Info(() => "Anisotropic filtering up to level {0} enabled.".FormatWith(anisoLevel));
            }
            else
            {
                staticLogger.Info(() => "Anisotropic filtering will not be enabled. It's either turned off in settings or the extension is not supported.");
            }

            staticLogger.Info(() => "Texturing set up.");
        }

        /// <summary>
        /// Initializes a new <see cref="Texture"/> from the specified <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The <see cref="TextureDescription"/> to initialize from.</param>
        protected Texture(TextureDescription description)
        {
            Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(TextureTarget), description.Target));

            this.Description = description;
        }

        /// <summary>
        /// Binds the <see cref="Texture"/> to the <see cref="P:TextureUnit"/>.
        /// </summary>
        public virtual void Bind()
        {
            this.Bind(this.TextureUnit);
        }

        /// <summary>
        /// Binds the <see cref="Texture"/> to the specified <paramref name="textureUnit"/>.
        /// </summary>
        /// <param name="textureUnit">The <see cref="TextureUnit"/> to bind to.</param>
        public virtual void Bind(TextureUnit textureUnit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            this.VerifyAccess();

            GL.ActiveTexture(textureUnit);
            GL.BindTexture(this.Target, this);
        }

        /// <summary>
        /// Unbinds the <see cref="Texture"/> from the <see cref="P:TextureUnit"/>.
        /// </summary>
        public void Unbind()
        {
            this.Unbind(this.TextureUnit);
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
            if (this.CheckAccess())
            {
                this.DeleteTexture(disposing);
            }
            else
            {
                this.Dispatcher.Invoke(this.DeleteTexture, disposing, DispatcherPriority.Background).Wait();
            }
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
            Contract.Invariant(this._TextureUnit >= 0);
        }
    }
}
