using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Represents an <see cref="IContentReader"/> that can read all primitive types (<see cref="Type.IsPrimitive"/>),
    /// <see cref="String"/>s and <see cref="Decimal"/>s.
    /// </summary>
    public class PrimitiveReader : IContentReader
    {
        /// <summary>
        /// Checks whether the <see cref="IContentReader"/> can read assets of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="assetType">The type of the asset that is about to be read.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="IContentReader"/> can read assets of the specified <see cref="Type"/>,
        /// otherwise <c>false</c>.
        /// </returns>
        public bool CanRead(Type assetType)
        {
            return assetType.IsPrimitive || (assetType == typeof(string)) || (assetType == typeof(decimal)) || (assetType == typeof(byte[]));
        }

        /// <summary>
        /// Asynchronously converts from the specified <see name="ContentReadParameters.AssetStream"/> into a
        /// usable asset of type <see name="ContentReadParameters.AssetType"/>.
        /// </summary>
        /// <remarks>
        /// This method accepts an <see cref="Encoding"/> via <see cref="ContentReadParameters.Parameter"/>. If
        /// <see cref="ContentReadParameters.Parameter"/> is <c>null</c>, <see cref="PrimitiveReader"/> will fall back
        /// to <see cref="Encoding.UTF8"/>.
        /// </remarks>
        /// <param name="parameters">
        /// <see cref="ContentReadParameters"/> containing information about the asset to be loaded.
        /// </param>
        /// <returns>
        /// The deserialized asset or <c>null</c> if an error occured or the specified type of asset cannot be read.
        /// </returns>
        public async Task<object> ReadAsync(ContentReadParameters parameters)
        {
            Encoding encoding = parameters.Parameter as Encoding;
            Type assetType = parameters.AssetType;
            if (assetType.IsPrimitive || assetType == typeof(decimal))
            {
                using (BinaryReader br = new BinaryReader(parameters.AssetStream, encoding ?? Encoding.UTF8, true))
                {
                    parameters.CancellationToken.ThrowIfCancellationRequested();
                    // Integers
                    if (assetType == typeof(byte))
                    {
                        return br.ReadByte();
                    }
                    else if (assetType == typeof(sbyte))
                    {
                        return br.ReadSByte();
                    }
                    else if (assetType == typeof(short))
                    {
                        return br.ReadInt16();
                    }
                    else if (assetType == typeof(ushort))
                    {
                        return br.ReadUInt16();
                    }
                    else if (assetType == typeof(int))
                    {
                        return br.ReadInt32();
                    }
                    else if (assetType == typeof(uint))
                    {
                        return br.ReadUInt32();
                    }
                    else if (assetType == typeof(long))
                    {
                        return br.ReadInt64();
                    }
                    else if (assetType == typeof(ulong))
                    {
                        return br.ReadUInt64();
                    }

                    // Floating point numbers
                    else if (assetType == typeof(float))
                    {
                        return br.ReadSingle();
                    }
                    else if (assetType == typeof(double))
                    {
                        return br.ReadDouble();
                    }
                    else if (assetType == typeof(decimal))
                    {
                        return br.ReadDecimal();
                    }

                    // Rest
                    else if (assetType == typeof(bool))
                    {
                        return br.ReadBoolean();
                    }
                    else if (assetType == typeof(char))
                    {
                        return br.ReadChar();
                    }
                    else if (assetType == typeof(IntPtr))
                    {
                        return new IntPtr(br.ReadInt32());
                    }
                    else if (assetType == typeof(UIntPtr))
                    {
                        return new UIntPtr(br.ReadUInt32());
                    }
                }
            }
            else if (assetType == typeof(string))
            {
                using (StreamReader sr = new StreamReader(parameters.AssetStream, encoding ?? Encoding.UTF8))
                {
                    parameters.CancellationToken.ThrowIfCancellationRequested();
                    return await sr.ReadToEndAsync();
                }
            }
            else if (assetType == typeof(byte[]))
            {
                using (MemoryStream ms = new MemoryStream(parameters.AssetStream.CanSeek ? MathF.ClampToInt32(parameters.AssetStream.Length) : 8192))
                {
                    await parameters.AssetStream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }

            return null;
        }
    }
}
