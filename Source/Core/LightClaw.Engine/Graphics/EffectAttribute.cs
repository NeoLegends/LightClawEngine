using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;
using LCBuffer = LightClaw.Engine.Graphics.OpenGL.Buffer;

namespace LightClaw.Engine.Graphics
{
    public class EffectAttribute : DisposableEntity, IInitializable
    {
        private readonly object initializationLock = new object();

        private string _AttributeName;

        public string AttributeName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return _AttributeName;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(value));

                this.SetProperty(ref _AttributeName, value);
            }
        }

        private IBuffer _Buffer = new LCBuffer(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);

        public IBuffer Buffer
        {
            get
            {
                Contract.Ensures(Contract.Result<IBuffer>() != null);

                return _Buffer;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Buffer, value);
            }
        }

        private bool _IsInitialized = false;

        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            private set
            {
                this.SetProperty(ref _IsInitialized, value);
            }
        }

        private int _Location = 0;

        public int Location
        {
            get
            {
                return _Location;
            }
            private set
            {
                this.SetProperty(ref _Location, value);
            }
        }

        public override string Name
        {
            get
            {
                return this.AttributeName;
            }
            set
            {
                throw new NotSupportedException("An {0}'s name cannot be set. It is hardcoded in the shader.".FormatWith(typeof(EffectAttribute).Name));
            }
        }

        private EffectStage _Stage;

        public EffectStage Stage
        {
            get
            {
                Contract.Ensures(Contract.Result<EffectStage>() != null);

                return _Stage;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Stage, value);
            }
        }

        private VertexAttribType _Type;

        public VertexAttribType Type
        {
            get
            {
                return _Type;
            }
            private set
            {
                this.SetProperty(ref _Type, value);
            }
        }

        private VertexAttributePointer _VertexAttibutePointer;

        public VertexAttributePointer VertexAttibutePointer
        {
            get
            {
                return _VertexAttibutePointer;
            }
            private set
            {
                this.SetProperty(ref _VertexAttibutePointer, value);
            }
        }

        public EffectAttribute(EffectStage stage, string name)
        {
            Contract.Requires<ArgumentNullException>(stage != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));

            this.AttributeName = name;
            this.Stage = stage;
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Location = GL.GetAttribLocation(this.Stage.ShaderProgram, this.AttributeName);

                        this.IsInitialized = true;
                    }
                }
            }
        }

        public void Set<T>(T[] data, VertexDataLayout layout)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Length > 0);

            this.Initialize();
            this.VerifyDataLayout(layout);
            this.Buffer.Set(data);
        }

        public void SetRange<T>(T[] data, int offset, VertexDataLayout layout)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Length > 0);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);

            this.Initialize();
            this.VerifyDataLayout(layout);
            this.Buffer.SetRange(data, offset);
        }

        public void Set(IntPtr data, int sizeInBytes, VertexDataLayout layout)
        {
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(sizeInBytes > 0);

            this.Initialize();
            this.VerifyDataLayout(layout);
            this.Buffer.Set(data, sizeInBytes);
        }

        public void SetRange(IntPtr data, int offset, int sizeInBytes, VertexDataLayout layout)
        {
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(sizeInBytes > 0);

            this.Initialize();
            this.VerifyDataLayout(layout);
            this.Buffer.SetRange(data, offset, sizeInBytes);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                this.Buffer.Dispose();

                base.Dispose(disposing);
            }
        }

        private void VerifyDataLayout(VertexDataLayout layout)
        {
            VertexAttributePointer vap = this.VertexAttibutePointer;
            if (vap != default(VertexAttributePointer))
            {
                if ((layout.Normalize != vap.Normalize) || (layout.Size != vap.Size) || (layout.Type != vap.Type))
                {
                    throw new InvalidOperationException("Once the layout has been set, it cannot be changed. Use the same layout as when it was set.");
                }
            }
            else
            {
                this.VertexAttibutePointer = new VertexAttributePointer(this.Location, layout.Size, layout.Type, layout.Normalize, 0, IntPtr.Zero);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(this._AttributeName));
            Contract.Invariant(this._Buffer != null);
            Contract.Invariant(this._Location >= 0);
            Contract.Invariant(this._Stage != null);
        }
    }
}
