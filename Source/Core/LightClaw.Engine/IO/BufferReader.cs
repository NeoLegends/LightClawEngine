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
    /// <summary>
    /// Represents an <see cref="IContentReader"/> reading <see cref="LightClaw.Engine.Graphics.Buffer"/>s.
    /// </summary>
    public class BufferReader : IContentReader
    {
        /// <summary>
        /// Asynchronously converts from the specified <paramref name="assetStream"/> into a <see cref="LightClaw.Engine.Graphics.Buffer"/>:
        /// </summary>
        /// <param name="contentManager">The <see cref="IContentManager"/> that triggered the loading process.</param>
        /// <param name="resourceString">The resource string of the asset to be loaded.</param>
        /// <param name="assetStream">A <see cref="Stream"/> of the asset's data.</param>
        /// <param name="assetType">The <see cref="Type"/> of asset to read.</param>
        /// <param name="parameter">A parameter the client specifies when requesting an asset.</param>
        /// <returns>
        /// The deserialized <see cref="LightClaw.Engine.Graphics.Buffer"/> or <c>null</c> if an error occured
        /// or the specified <paramref name="assetType"/> cannot be read.
        /// </returns>
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

        /// <summary>
        /// Parameters required for interpreting the data inside the asset stream.
        /// </summary>
        [DataContract, ProtoContract]
        public struct BufferLoadParameters
        {
            /// <summary>
            /// An initial offset inside the asset stream buffer.
            /// </summary>
            [DataMember, ProtoMember(1)]
            public long Start { get; private set; }

            /// <summary>
            /// The amount of data to read.
            /// </summary>
            [DataMember, ProtoMember(2)]
            public long Count { get; private set; }

            /// <summary>
            /// A <see cref="BufferUsageHint"/> hinting the buffer's usage.
            /// </summary>
            [DataMember, ProtoMember(3)]
            public BufferUsageHint Hint { get; private set; }
            
            /// <summary>
            /// The <see cref="LightClaw.Engine.Graphics.Buffer"/>s <see cref="BufferTarget"/>.
            /// </summary>
            [DataMember, ProtoMember(4)]
            public BufferTarget Target { get; private set; }

            /// <summary>
            /// Initializes new <see cref="BufferLoadParameters"/> setting the amount of data inside the buffer and the
            /// <see cref="BufferTarget"/>.
            /// </summary>
            /// <remarks>Offset / Start will be 0 and <see cref="BufferUsageHint.StaticDraw"/> will be used.</remarks>
            /// <param name="count">The amount of data to read.</param>
            /// <param name="target">The <see cref="LightClaw.Engine.Graphics.Buffer"/>s <see cref="BufferTarget"/>.</param>
            public BufferLoadParameters(long count, BufferTarget target)
                : this(0L, count, target)
            {
                Contract.Requires<ArgumentOutOfRangeException>(count >= 0L);
            }

            /// <summary>
            /// Initializes new <see cref="BufferLoadParameters"/> setting the offset inside the stream, the amount of data inside
            /// the buffer and the <see cref="BufferTarget"/>.
            /// </summary>
            /// <remarks><see cref="BufferUsageHint.StaticDraw"/> will be used.</remarks>
            /// <param name="count">The amount of data to read.</param>
            /// <param name="start">An initial offset inside the asset stream buffer.</param>
            /// <param name="target">The <see cref="LightClaw.Engine.Graphics.Buffer"/>s <see cref="BufferTarget"/>.</param>
            public BufferLoadParameters(long start, long count, BufferTarget target)
                : this(start, count, target, BufferUsageHint.StaticDraw)
            {
                Contract.Requires<ArgumentOutOfRangeException>(count >= 0L);
                Contract.Requires<ArgumentOutOfRangeException>(start >= 0L);
            }

            /// <summary>
            /// Initializes new <see cref="BufferLoadParameters"/> setting the offset inside the stream, the amount of data inside
            /// the buffer, the <see cref="BufferUsageHint"/> and the <see cref="BufferTarget"/>.
            /// </summary>
            /// <param name="count">The amount of data to read.</param>
            /// <param name="hint">A <see cref="BufferUsageHint"/> hinting the buffer's usage.</param>
            /// <param name="start">An initial offset inside the asset stream buffer.</param>
            /// <param name="target">The <see cref="LightClaw.Engine.Graphics.Buffer"/>s <see cref="BufferTarget"/>.</param>
            public BufferLoadParameters(long start, long count, BufferTarget target, BufferUsageHint hint)
                : this()
            {
                Contract.Requires<ArgumentOutOfRangeException>(count >= 0L);
                Contract.Requires<ArgumentOutOfRangeException>(start >= 0L);

                this.Count = count;
                this.Hint = hint;
                this.Target = target;
                this.Count = count;
            }
        }
    }
}
