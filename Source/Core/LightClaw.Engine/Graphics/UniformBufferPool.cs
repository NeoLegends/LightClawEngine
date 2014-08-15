using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class UniformBufferPool : Entity
    {
        private static readonly UniformBufferPool _Default = new UniformBufferPool();

        public static UniformBufferPool Default
        {
            get
            {
                Contract.Ensures(Contract.Result<UniformBufferPool>() != null);

                return _Default;
            }
        }

        private readonly ConcurrentDictionary<object, ConcurrentDictionary<int, RangedBuffer>> pipelinePools = new ConcurrentDictionary<object, ConcurrentDictionary<int, RangedBuffer>>();

        private readonly ConcurrentBag<BufferUllage> uniformBuffers = new ConcurrentBag<BufferUllage>();

        public UniformBufferPool()
        {
            this.uniformBuffers.Add(new BufferUllage(new Buffer(BufferTarget.UniformBuffer, BufferUsageHint.DynamicDraw), 0));
        }

        public UniformBufferDescription GetBuffer(int length, object pipeline)
        {
            throw new NotImplementedException();
        }

        private struct BufferUllage
        {
            public Buffer Buffer { get; private set; }

            public int Ullage { get; private set; }

            public BufferUllage(Buffer buffer, int ullage)
                : this()
            {
                Contract.Requires<ArgumentNullException>(buffer != null);
                Contract.Requires<ArgumentOutOfRangeException>(ullage >= 0);

                this.Buffer = buffer;
                this.Ullage = ullage;
            }
        }
    }
}
