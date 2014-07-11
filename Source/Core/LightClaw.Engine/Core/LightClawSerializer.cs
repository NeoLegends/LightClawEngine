using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ProtoBuf.Meta;

namespace LightClaw.Engine.Core
{
    public class LightClawSerializer
    {
        private readonly RuntimeTypeModel model = RuntimeTypeModel.Create();

        public LightClawSerializer()
        {
            this.model.AutoAddMissingTypes = true;
            this.model.AutoCompile = true;
        }

        public Task<T> DeserializeAsync<T>(Stream source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<NotSupportedException>(source.CanRead);

            return Task.Run(() => (T)this.model.DeserializeWithLengthPrefix(source, null, typeof(T), PrefixStyle.Fixed32, 1));
        }

        public Task<T> DeserializeAsync<T>(byte[] source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            using (MemoryStream ms = new MemoryStream(source))
            {
                return this.DeserializeAsync<T>(ms);
            }
        }

        public Task SerializeAsync(Stream destination, object instance)
        {
            Contract.Requires<ArgumentNullException>(destination != null);
            Contract.Requires<ArgumentNullException>(instance != null);
            Contract.Requires<NotSupportedException>(destination.CanWrite);

            Type instanceType = instance.GetType();
            return Task.Run(() => this.model.SerializeWithLengthPrefix(destination, instance, instanceType, PrefixStyle.Fixed32, 1));
        }

        public Task<byte[]> SerializeAsync(object instance)
        {
            Contract.Requires<ArgumentNullException>(instance != null);

            using (MemoryStream ms = new MemoryStream(32768))
            {
                return this.SerializeAsync(ms, instance).ContinueWith(t => ms.ToArray());
            }
        }
    }
}
