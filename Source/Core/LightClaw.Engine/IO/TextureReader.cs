using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Engine.Threading;
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
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, GDIPixelFormat.Format32bppArgb);
                try
                {
                    return await this.Dispatcher.Invoke((Func<CancellationToken, object>)(ct =>
                    {
                        ct.ThrowIfCancellationRequested();

                        TextureDescription desc = new TextureDescription(
                            data.Width, data.Height,
                            TextureDescription.GetMaxTextureLevels(data.Width, data.Height),
                            2,
                            (TextureTarget2d)(parameters.Parameter ?? TextureTarget2d.Texture2D),
                            PixelInternalFormat.Rgba
                        );
                        if (parameters.AssetType == typeof(Texture2D))
                        {
                            return Texture2D.Create(desc, data.Scan0, GLPixelFormat.Bgra, PixelType.UnsignedByte, data.Width, data.Height, 0, 0, 0);
                        }
                        else if (parameters.AssetType == typeof(Texture1DArray))
                        {
                            return Texture1DArray.Create(desc, data.Scan0, GLPixelFormat.Bgra, PixelType.UnsignedByte, data.Width, data.Height, 0, 0, 0);
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
    }
}
