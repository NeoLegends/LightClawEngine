using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public static class EnumExtensions
    {
        public static bool IsSamplerUniform(this ActiveUniformType aut) // Damn, I hate this function. It's quick though.
        {
            switch (aut)
            {
                case ActiveUniformType.Sampler1D:
                case ActiveUniformType.Sampler1DShadow:
                case ActiveUniformType.Sampler1DArray:
                case ActiveUniformType.Sampler1DArrayShadow:
                case ActiveUniformType.Sampler2D:
                case ActiveUniformType.Sampler2DShadow:
                case ActiveUniformType.Sampler2DArray:
                case ActiveUniformType.Sampler2DArrayShadow:
                case ActiveUniformType.Sampler2DMultisample:
                case ActiveUniformType.Sampler2DMultisampleArray:
                case ActiveUniformType.Sampler2DRect:
                case ActiveUniformType.Sampler2DRectShadow:
                case ActiveUniformType.Sampler3D:
                case ActiveUniformType.SamplerCube:
                case ActiveUniformType.SamplerCubeShadow:
                case ActiveUniformType.SamplerCubeMapArray:
                case ActiveUniformType.SamplerCubeMapArrayShadow:
                case ActiveUniformType.IntSampler1D:
                case ActiveUniformType.IntSampler1DArray:
                case ActiveUniformType.IntSampler2D:
                case ActiveUniformType.IntSampler2DArray:
                case ActiveUniformType.IntSampler2DMultisample:
                case ActiveUniformType.IntSampler2DMultisampleArray:
                case ActiveUniformType.IntSampler2DRect:
                case ActiveUniformType.IntSampler3D:
                case ActiveUniformType.IntSamplerCube:
                case ActiveUniformType.IntSamplerCubeMapArray:
                case ActiveUniformType.UnsignedIntSampler1D:
                case ActiveUniformType.UnsignedIntSampler1DArray:
                case ActiveUniformType.UnsignedIntSampler2D:
                case ActiveUniformType.UnsignedIntSampler2DArray:
                case ActiveUniformType.UnsignedIntSampler2DMultisample:
                case ActiveUniformType.UnsignedIntSampler2DMultisampleArray:
                case ActiveUniformType.UnsignedIntSampler2DRect:
                case ActiveUniformType.UnsignedIntSampler3D:
                case ActiveUniformType.UnsignedIntSamplerCube:
                case ActiveUniformType.UnsignedIntSamplerCubeMapArray:
                    return true;
            }

            return false;
        }
    }
}
