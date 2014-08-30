using System;
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

        private IBuffer _Buffer = new Buffer(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);

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
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _Location;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

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

        private int _Size;

        public int Size
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() > 0);

                return _Size;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value > 0);

                this.SetProperty(ref _Size, value);
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

        public EffectAttribute(EffectStage stage, string name, int size)
        {
            Contract.Requires<ArgumentNullException>(stage != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
            Contract.Requires<ArgumentOutOfRangeException>(size > 0);

            this.Name = name;
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

        public void Set<T>(T[] data)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Length > 0);

            this.Buffer.Set(data);
        }

        public void SetRange<T>(T[] data, int offset)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentException>(data.Length > 0);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);

            this.Buffer.SetRange(data, offset);
        }

        public void Set(IntPtr data, int sizeInBytes)
        {
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(sizeInBytes > 0);

            this.Buffer.Set(data, sizeInBytes);
        }

        public void SetRange(IntPtr data, int offset, int sizeInBytes)
        {
            Contract.Requires<ArgumentNullException>(data != IntPtr.Zero);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(sizeInBytes > 0);

            this.Buffer.SetRange(data, offset, sizeInBytes);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(this.AttributeName));
            Contract.Invariant(this._Location >= 0);
            Contract.Invariant(this._Size > 0);
            Contract.Invariant(this.Stage != null);
        }
    }
}
