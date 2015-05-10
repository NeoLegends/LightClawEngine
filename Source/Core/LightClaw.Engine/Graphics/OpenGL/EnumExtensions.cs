using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Contains extension methods to various OpenGL enumerations.
    /// </summary>
    [Pure]
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the size in machine units of the unit represented by the specified <paramref name="drawElementsType"/>.
        /// </summary>
        /// <param name="drawElementsType">The <see cref="DrawElementsType"/> to get the machine unit size from.</param>
        /// <returns>The size of the specified element in machine units.</returns>
        public static int GetElementSize(this DrawElementsType drawElementsType)
        {
            switch (drawElementsType)
            {
                case DrawElementsType.UnsignedByte:
                    return sizeof(byte);
                case DrawElementsType.UnsignedInt:
                    return sizeof(uint);
                case DrawElementsType.UnsignedShort:
                    return sizeof(ushort);
                default:
                    throw new InvalidOperationException("The size of the index element cannot be determined. Enum value is invalid.");
            }
        }

        #region ActiveUniformType

        /// <summary>
        /// Checks whether the specified <see cref="ActiveUniformType"/> is a matrix.
        /// </summary>
        /// <param name="type">The <see cref="ActiveUniformType"/> to check.</param>
        /// <returns><c>true</c> if the <paramref name="type"/> of the uniform is matrix.</returns>
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

        /// <summary>
        /// Checks whether the specified <see cref="ActiveUniformType"/> is a primitive value like an int, a float, etc..
        /// </summary>
        /// <param name="type">The <see cref="ActiveUniformType"/> to check.</param>
        /// <returns><c>true</c> if the <paramref name="type"/> of the uniform is primitive value.</returns>
        public static bool IsPrimitiveValue(this ActiveUniformType type)
        {
            switch (type)
            {
                case ActiveUniformType.Bool:
                case ActiveUniformType.Double:
                case ActiveUniformType.Float:
                case ActiveUniformType.Int:
                case ActiveUniformType.UnsignedInt:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether the specified <see cref="ActiveUniformType"/> is a sampler.
        /// </summary>
        /// <param name="type">The <see cref="ActiveUniformType"/> to check.</param>
        /// <returns><c>true</c> if the <paramref name="type"/> of the uniform is sampler.</returns>
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

        /// <summary>
        /// Checks whether the specified <see cref="ActiveUniformType"/> is a vector.
        /// </summary>
        /// <param name="type">The <see cref="ActiveUniformType"/> to check.</param>
        /// <returns><c>true</c> if the <paramref name="type"/> of the uniform is vector.</returns>
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

        #endregion

        #region TextureTarget

        /// <summary>
        /// Checks whether the specified <see cref="TextureTarget"/> is a one-dimensional target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if <paramref name="target"/> is a one-dimensional target, otherwise <c>false</c>.</returns>
        public static bool IsTexture1DTarget(this TextureTarget target)
        {
            return (target == TextureTarget.Texture1D) || (target == TextureTarget.ProxyTexture1D);
        }

        /// <summary>
        /// Checks whether the specified <see cref="TextureTarget"/> is a one-dimensional array target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if <paramref name="target"/> is a one-dimensional array target, otherwise <c>false</c>.</returns>
        public static bool IsTexture1DArrayTarget(this TextureTarget target)
        {
            return (target == TextureTarget.Texture1DArray) || (target == TextureTarget.Texture1DArray);
        }

        /// <summary>
        /// Checks whether the specified <see cref="TextureTarget"/> is a two-dimensional target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if <paramref name="target"/> is a two-dimensional target, otherwise <c>false</c>.</returns>
        public static bool IsTexture2DTarget(this TextureTarget target)
        {
            switch (target)
            {
                case TextureTarget.Texture2D:
                case TextureTarget.Texture2DMultisample:
                case TextureTarget.TextureCubeMap:
                case TextureTarget.ProxyTextureCubeMap:
                case TextureTarget.ProxyTexture2D:
                case TextureTarget.ProxyTexture2DMultisample:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether the specified <see cref="TextureTarget"/> is a two-dimensional array target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if <paramref name="target"/> is a two-dimensional array target, otherwise <c>false</c>.</returns>
        public static bool IsTexture2DArrayTarget(this TextureTarget target)
        {
            switch (target)
            {
                case TextureTarget.Texture2DArray:
                case TextureTarget.Texture2DMultisampleArray:
                case TextureTarget.ProxyTexture2DArray:
                case TextureTarget.ProxyTexture2DMultisampleArray:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether the specified <see cref="TextureTarget"/> is a three-dimensional target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if <paramref name="target"/> is a three-dimensional target, otherwise <c>false</c>.</returns>
        public static bool IsTexture3DTarget(this TextureTarget target)
        {
            switch (target)
            {
                case TextureTarget.Texture3D:
                case TextureTarget.ProxyTexture3D:
                case TextureTarget.TextureCubeMap:
                case TextureTarget.ProxyTextureCubeMap:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        /// <summary>
        /// Converts a <see cref="BufferAccess"/> into a <see cref="BufferAccessMask"/>.
        /// </summary>
        /// <param name="access">The <see cref="BufferAccess"/> to convert.</param>
        /// <returns>The conversion result.</returns>
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
