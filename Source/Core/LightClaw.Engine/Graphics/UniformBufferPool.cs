using System;
using System.Collections.Concurrent;
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
    public class UniformBufferPool : Entity, IDisposable
    {
        private static readonly object glPropertyLock = new object();

        private static readonly UniformBufferPool _Default = new UniformBufferPool();

        public static UniformBufferPool Default
        {
            get
            {
                Contract.Ensures(Contract.Result<UniformBufferPool>() != null);

                return _Default;
            }
        }

        private static int _MaxGeometryUboCount = -1;

        public static int MaxGeometryUboCount
        {
            get
            {
                lock (glPropertyLock)
                {
                    int maxUboCount = _MaxGeometryUboCount;
                    return (maxUboCount > 0) ? maxUboCount : (_MaxGeometryUboCount = GL.GetInteger(GetPName.MaxGeometryUniformBlocks));
                }
            }
        }

        private static int _MaxFragmentUboCount = -1;

        public static int MaxFragmentUboCount
        {
            get
            {
                lock (glPropertyLock)
                {
                    int maxUboCount = _MaxFragmentUboCount;
                    return (maxUboCount > 0) ? maxUboCount : (_MaxFragmentUboCount = GL.GetInteger(GetPName.MaxFragmentUniformBlocks));
                }
            }
        }

        private static int _MaxVertexUboCount = -1;

        public static int MaxVertexUboCount
        {
            get
            {
                lock (glPropertyLock)
                {
                    int maxUboCount = _MaxVertexUboCount;
                    return (maxUboCount > 0) ? maxUboCount : (_MaxVertexUboCount = GL.GetInteger(GetPName.MaxVertexUniformBlocks));
                }
            }
        }

        private static int _MaxUboLength = -1;

        public static int MaxUboLength
        {
            get
            {
                lock (glPropertyLock)
                {
                    int maxUboLength = _MaxUboLength;
                    return (maxUboLength > 0) ? maxUboLength : (_MaxUboLength = GL.GetInteger(GetPName.MaxUniformBlockSize));
                }
            }
        }

        private static int _UboAlignment = -1;

        public static int UboAlignment
        {
            get
            {
                lock (glPropertyLock)
                {
                    int uboAlignment = _UboAlignment;
                    return (uboAlignment > 0) ? uboAlignment : (_UboAlignment = GL.GetInteger(GetPName.UniformBufferOffsetAlignment));
                }
            }
        }

        private readonly object bufferObtainLock = new object();

        private readonly Dictionary<object, List<UboBinding>> pipelineBindings = new Dictionary<object, List<UboBinding>>();

        private readonly List<Buffer> uniformBuffers = new List<Buffer>();

        private bool _IsDisposed = false;

        public bool IsDisposed
        {
            get
            {
                return _IsDisposed;
            }
            private set
            {
                this.SetProperty(ref _IsDisposed, value);
            }
        }

        public UniformBufferPool()
        {
            this.uniformBuffers.Add(new Buffer(BufferTarget.UniformBuffer, BufferUsageHint.DynamicDraw));
        }

        ~UniformBufferPool()
        {
            this.Dispose(false);
        }

        public RangedBuffer GetBuffer(int length, Stage stage, object pipeline)
        {
            lock (bufferObtainLock)
            {
                List<UboBinding> bindings;
                if (!this.pipelineBindings.TryGetValue(pipeline, out bindings))
                {
                    this.pipelineBindings.Add(pipeline, new List<UboBinding>());
                }

                int maxUbos = GetMaxUniformBlocksForStage(stage);
                if (bindings.Count(binding => binding.Stage == stage) + 1 > maxUbos)
                {
                    throw new OutOfUniformBufferSpaceException("The maximum amount of bound uniform buffers for the specified stage ({0}) is reached. {1} buffers are already bound.".FormatWith(stage, maxUbos));
                }

                int bindingIndex = GetMinimumBindingIndex(bindings);
                if (bindingIndex > maxUbos)
                {
                    throw new OutOfUniformBufferSpaceException("All bindable uniform blocks ({0}) taken.".FormatWith(bindings.Count));
                }

                Buffer targetBuffer = this.uniformBuffers.FirstOrDefault(buffer => buffer.Count + length < MaxUboLength);
                if (targetBuffer == null)
                {
                    targetBuffer = new Buffer(BufferTarget.UniformBuffer, BufferUsageHint.DynamicDraw);
                    this.uniformBuffers.Add(targetBuffer);
                }

                int alignedOffset = MathF.RoundToMultiple(targetBuffer.Count, UboAlignment);
                RangedBuffer rangedBuffer = new RangedBuffer(
                    targetBuffer,
                    new BufferRange(alignedOffset, length),
                    bindingIndex,
                    BufferRangeTarget.UniformBuffer
                );
                bindings.Add(new UboBinding(rangedBuffer, bindingIndex, stage));

                byte[] zeros = new byte[length];
                zeros.Initialize();
                rangedBuffer.Set(zeros);

                return rangedBuffer;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (Buffer ubo in this.uniformBuffers.FilterNull())
            {
                try
                {
                    ubo.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Warn(() => "An error occured while disposing one of the {0}'s underlying {1}s.".FormatWith(typeof(UniformBufferPool).Name, typeof(Buffer).Name), ex);
                }
            }
            if (disposing)
            {
                this.pipelineBindings.Clear();
                this.uniformBuffers.Clear();
            }
            GC.SuppressFinalize(this);
            this.IsDisposed = true;
        }

        private static int GetMinimumBindingIndex(IEnumerable<UboBinding> bindings)
        {
            Contract.Requires<ArgumentNullException>(bindings != null);

            // See http://stackoverflow.com/a/1100221
            IEnumerable<int> indices = bindings.Select(binding => binding.Index);
            return Enumerable.Range(0, indices.Max() + 1).Except(indices).Min();
        }

        private static int GetMaxUniformBlocksForStage(Stage stage)
        {
            switch (stage)
            {
                case Stage.Fragment:
                    return MaxFragmentUboCount;
                case Stage.Geometry:
                    return MaxGeometryUboCount;
                case Stage.Vertex:
                    return MaxVertexUboCount;
                default:
                    throw new NotSupportedException("Returning the maximum uniform block count is only possible for the default values of {0}.".FormatWith(typeof(Stage).Name));
            }
        }

        public enum Stage
        {
            Fragment,

            Geometry,

            Vertex
        }

        private struct UboBinding
        {
            public int Index { get; private set; }

            public RangedBuffer Buffer { get; private set; }

            public Stage Stage { get; private set; }

            public UboBinding(RangedBuffer buffer, int index, Stage stage)
                : this()
            {
                Contract.Requires<ArgumentNullException>(buffer != null);
                Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
                
                this.Index = index;
                this.Buffer = buffer;
                this.Stage = stage;
            }
        }
    }
}
