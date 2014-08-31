using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class TextureUnitManager : Entity
    {
        private readonly bool[] allocatedTextureUnits = new bool[GL.GetInteger(GetPName.MaxTextureUnits)];

        public TextureUnitManager() { }

        public TextureUnit GetTextureUnit()
        {
            for (int i = 0; i < allocatedTextureUnits.Length; i++)
            {
                if (!allocatedTextureUnits[i])
                {
                    allocatedTextureUnits[i] = true;
                    TextureUnit result = new TextureUnit(i);
                    result.Disposed += TextureUnitDisposed;
                    return result;
                }
            }

            throw new InvalidOperationException("No free texture units available.");
        }

        private void TextureUnitDisposed(object sender, ParameterEventArgs e)
        {
            Contract.Assume(sender is TextureUnit);

            if (e.Parameter != null)
            {
                int allocatedTextureUnit = (int)e.Parameter;
                if (allocatedTextureUnit >= 0 && allocatedTextureUnit < allocatedTextureUnits.Length)
                {
                    this.allocatedTextureUnits[allocatedTextureUnit] = false;
                    ((TextureUnit)sender).Disposed -= TextureUnitDisposed;
                }
                else
                {
                    Logger.Warn(() => "The texture unit to release ({0}) was outside of the boundaries (0 to {1}, inclusive).".FormatWith(allocatedTextureUnit, allocatedTextureUnits.Length - 1));
                }
            }
        }
    }
}
