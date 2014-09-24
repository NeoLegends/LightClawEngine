using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public static class EnumExtensions
    {
        public static bool IsMatrix(this ActiveUniformType type)
        {
            switch (type)
            {
                case ActiveUniformType.FloatMat2:
                case ActiveUniformType.FloatMat2x3:
                case ActiveUniformType.FloatMat2x4:
                case ActiveUniformType.FloatMat3:
                case ActiveUniformType.FloatMat3x2:
                case ActiveUniformType.FloatMat3x4:
                case ActiveUniformType.FloatMat4:
                case ActiveUniformType.FloatMat4x2:
                case ActiveUniformType.FloatMat4x3:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSampler(this ActiveUniformType aut)
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
                default:
                    return false;
            }
        }

        public static bool IsVector(this ActiveUniformType type)
        {
            switch (type)
            {
                case ActiveUniformType.BoolVec2:
                case ActiveUniformType.BoolVec3:
                case ActiveUniformType.BoolVec4:
                case ActiveUniformType.DoubleVec2:
                case ActiveUniformType.DoubleVec3:
                case ActiveUniformType.DoubleVec4:
                case ActiveUniformType.FloatVec2:
                case ActiveUniformType.FloatVec3:
                case ActiveUniformType.FloatVec4:
                case ActiveUniformType.IntVec2:
                case ActiveUniformType.IntVec3:
                case ActiveUniformType.IntVec4:
                case ActiveUniformType.UnsignedIntVec2:
                case ActiveUniformType.UnsignedIntVec3:
                case ActiveUniformType.UnsignedIntVec4:
                    return true;
                default:
                    return false;
            }
        }

        public static BufferAccessMask ToAccessMask(this BufferAccess access)
        {
            switch (access)
            {
                case BufferAccess.ReadOnly:
                    return BufferAccessMask.MapReadBit;
                case BufferAccess.ReadWrite:
                    return BufferAccessMask.MapWriteBit | BufferAccessMask.MapReadBit;
                case BufferAccess.WriteOnly:
                    return BufferAccessMask.MapWriteBit;
                default:
                    throw new InvalidOperationException("The access-parameter had a wrong value.");
            }
        }
    }
}
