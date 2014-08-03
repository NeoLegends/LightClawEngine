using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace LightClaw.Engine.IO
{
    public class BufferReader : IContentReader
    {
        public async Task<object> ReadAsync(IContentManager contentManager, string resourceString, Stream assetStream, Type assetType, object parameter)
        {
            if ((assetType == typeof(Buffer)) && (parameter != null) && (parameter is BufferLoadParameters))
            {
                BufferLoadParameters parameters = (BufferLoadParameters)parameter;
                if (assetStream.CanSeek)
                {
                    assetStream.Seek(parameters.Start, SeekOrigin.Begin);
                }

                byte[] bufferData = new byte[parameters.Count];
                await assetStream.ReadExactlyAsync(parameters.Count);
                return Graphics.Buffer.Create(bufferData, parameters.Target, parameters.Hint);
            }
            else
            {
                return null;
            }
        }

        [DataContract, ProtoContract]
        public struct BufferLoadParameters
        {
            [DataMember, ProtoMember(1)]
            public long Start { get; private set; }

            [DataMember, ProtoMember(2)]
            public long Count { get; private set; }

            [DataMember, ProtoMember(3)]
            public BufferUsageHint Hint { get; private set; }

            [DataMember, ProtoMember(4)]
            public BufferTarget Target { get; private set; }

            public BufferLoadParameters(long count, BufferTarget target)
                : this(0L, count, target)
            {
                Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            }

            public BufferLoadParameters(long start, long count, BufferTarget target)
                : this(start, count, target, BufferUsageHint.StaticDraw)
            {
                Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
                Contract.Requires<ArgumentOutOfRangeException>(start >= 0);
            }

            public BufferLoadParameters(long start, long count, BufferTarget target, BufferUsageHint hint)
                : this()
            {
                Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
                Contract.Requires<ArgumentOutOfRangeException>(start >= 0);

                this.Count = count;
                this.Hint = hint;
                this.Target = target;
                this.Count = count;
            }
        }
    }
}
