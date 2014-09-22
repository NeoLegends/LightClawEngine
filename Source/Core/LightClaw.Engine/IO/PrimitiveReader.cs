using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;

namespace LightClaw.Engine.IO
{
    public class PrimitiveReader : IContentReader
    {
        public bool CanRead(Type assetType)
        {
            return assetType.IsPrimitive || (assetType == typeof(string)) || (assetType == typeof(decimal));
        }

        public Task<object> ReadAsync(ContentReadParameters parameters)
        {
            Encoding encoding = parameters.Parameter as Encoding;
            Type assetType = parameters.AssetType;
            if (assetType.IsPrimitive)
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
            else if (assetType == typeof(decimal))
            {
                using (BinaryReader br = new BinaryReader(parameters.AssetStream, encoding ?? Encoding.UTF8))
                {
                    return Task.FromResult<object>(br.ReadDecimal());
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
