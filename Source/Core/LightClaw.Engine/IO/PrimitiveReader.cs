using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents an <see cref="IContentReader"/> that can read all primitive types ( <see cref="Type.IsPrimitive"/> ),
    /// <see cref="String"/> s and <see cref="Decimal"/>s.
    /// </summary>
    public class PrimitiveReader : IContentReader
    {
        /// <summary>
        /// Checks whether the <see cref="IContentReader"/> can read assets of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="assetType">The type of the asset that is about to be read.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="IContentReader"/> can read assets of the specified <see cref="Type"/> ,
        /// otherwise <c>false</c> .
        /// </returns>
        public bool CanRead(Type assetType)
        {
            return assetType.IsPrimitive || (assetType == typeof(string)) || (assetType == typeof(decimal));
        }

        /// <summary>
        /// Asynchronously converts from the specified <paramref name="ContentReadParameters.AssetStream"/> into a
        /// usable asset of type <paramref name="ContentReadParameters.AssetType"/>.
        /// </summary>
        /// <remarks>
        /// This method accepts an <see cref="Encoding"/> via <see cref="ContentReadParameters.Parameter"/>. If
        /// <see cref="ContentReadParameters.Parameter"/> is <c>null</c> , <see cref="PrimitiveReader"/> will fall back
        /// to <see cref="Encoding.UTF8"/>.
        /// </remarks>
        /// <param name="parameters">
        /// <see cref="ContentReadParameters"/> containing information about the asset to be loaded.
        /// </param>
        /// <returns>
        /// The deserialized asset or <c>null</c> if an error occured or the specified type of asset cannot be read.
        /// </returns>
        public Task<object> ReadAsync(ContentReadParameters parameters)
        {
            Encoding encoding = parameters.Parameter as Encoding;
            Type assetType = parameters.AssetType;
            if (assetType.IsPrimitive || assetType == typeof(decimal))
            {
                using (BinaryReader br = new BinaryReader(parameters.AssetStream, encoding ?? Encoding.UTF8, true))
                {
                    // Integers
                    if (assetType == typeof(byte))
                    {
                        return Task.FromResult<object>(br.ReadByte());
                    }
                    else if (assetType == typeof(sbyte))
                    {
                        return Task.FromResult<object>(br.ReadSByte());
                    }
                    else if (assetType == typeof(short))
                    {
                        return Task.FromResult<object>(br.ReadInt16());
                    }
                    else if (assetType == typeof(ushort))
                    {
                        return Task.FromResult<object>(br.ReadUInt16());
                    }
                    else if (assetType == typeof(int))
                    {
                        return Task.FromResult<object>(br.ReadInt32());
                    }
                    else if (assetType == typeof(uint))
                    {
                        return Task.FromResult<object>(br.ReadUInt32());
                    }
                    else if (assetType == typeof(long))
                    {
                        return Task.FromResult<object>(br.ReadInt64());
                    }
                    else if (assetType == typeof(ulong))
                    {
                        return Task.FromResult<object>(br.ReadUInt64());
                    }

                    // Floating point numbers
                    else if (assetType == typeof(float))
                    {
                        return Task.FromResult<object>(br.ReadSingle());
                    }
                    else if (assetType == typeof(double))
                    {
                        return Task.FromResult<object>(br.ReadDouble());
                    }
                    if (assetType == typeof(decimal))
                    {
                        return Task.FromResult<object>(br.ReadDecimal());
                    }

                    // Rest
                    else if (assetType == typeof(bool))
                    {
                        return Task.FromResult<object>(br.ReadBoolean());
                    }
                    else if (assetType == typeof(char))
                    {
                        return Task.FromResult<object>(br.ReadChar());
                    }
                    else if (assetType == typeof(IntPtr))
                    {
                        return Task.FromResult<object>(new IntPtr(br.ReadInt32()));
                    }
                    else if (assetType == typeof(UIntPtr))
                    {
                        return Task.FromResult<object>(new UIntPtr(br.ReadUInt32()));
                    }
                    else
                    {
                        return Task.FromResult<object>(null);
                    }
                }
            }
            else if (assetType == typeof(string))
            {
                using (StreamReader sr = new StreamReader(parameters.AssetStream, encoding ?? Encoding.UTF8))
                {
                    return sr.ReadToEndAsync().Upcast<string, object>();
                }
            }
            else
            {
                return Task.FromResult<object>(null);
            }
        }
    }
}
