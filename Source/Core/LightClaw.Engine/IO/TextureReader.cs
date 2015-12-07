using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;
using GDIPixelFormat = System.Drawing.Imaging.PixelFormat;
using GLPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace LightClaw.Engine.IO
{
    public class TextureReader : DispatcherEntity, IContentReader
    {
        public bool CanRead(Type assetType, object parameter)
        {
            return (assetType == typeof(Texture2D) || assetType == typeof(Texture1DArray));
        }

        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            using (Bitmap bmp = new Bitmap(parameters.AssetStream))
            {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                try
                {
                    return await this.Dispatcher.Invoke((Func<CancellationToken, object>)(ct =>
                    {
                        ct.ThrowIfCancellationRequested();

                        TextureDescription desc = GetDescription(parameters.AssetType, data);
                        if (parameters.AssetType == typeof(Texture1D))
                        {
                            return Texture1D.Create(
                                desc, data.Scan0,
                                bmp.PixelFormat.ToGlPixelFormat(), PixelType.UnsignedByte,
                                data.Width, 
                                0, 0
                            );
                        }
                        else if (parameters.AssetType == typeof(Texture1DArray))
                        {
                            return Texture1DArray.Create(
                                desc, data.Scan0,
                                bmp.PixelFormat.ToGlPixelFormat(), PixelType.UnsignedByte, 
                                data.Width, data.Height, 
                                0, 0, 0
                            );
                        }
                        else if (parameters.AssetType == typeof(Texture2D))
                        {
                            return Texture2D.Create(
                                desc, data.Scan0,
                                bmp.PixelFormat.ToGlPixelFormat(), PixelType.UnsignedByte, 
                                data.Width, data.Height, 
                                0, 0, 0
                            );
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    "{0} can only load {1} and {2}, not {3}!",
                                    typeof(TextureReader).Name,
                                    typeof(Texture1DArray).Name,
                                    typeof(Texture2D).Name,
                                    parameters.AssetType.Name
                                )
                            );
                        }
                    }), DispatcherPriority.Normal, parameters.CancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    bmp.UnlockBits(data);
                }
            }
        }

        private static TextureDescription GetDescription(Type textureType, BitmapData data)
        {
            Contract.Requires<ArgumentNullException>(textureType != null);
            Contract.Requires<ArgumentNullException>(data != null);

            return new TextureDescription(
                data.Width, data.Height,
                TextureDescription.GetMaxTextureLevels(data.Width, data.Height),
                2,
                (TextureTarget2d)GetTarget(textureType),
                SizedInternalFormat.Rgba8
            );
        }

        private static TextureTarget GetTarget(Type textureType)
        {
            Contract.Requires<ArgumentNullException>(textureType != null);

            if (textureType == typeof(Texture1D))
            {
                return TextureTarget.Texture1D;
            }
            if (textureType == typeof(Texture1DArray))
            {
                return TextureTarget.Texture1DArray;
            }
            else if (textureType == typeof(Texture2D))
            {
                return TextureTarget.Texture2D;
            }
            else
            {
                throw new NotSupportedException("Unsupported {0}!".FormatWith(typeof(TextureTarget).Name));
            }
        }
    }
}
