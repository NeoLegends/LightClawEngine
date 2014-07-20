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
        private RuntimeTypeModel runtimeTypeModel = RuntimeTypeModel.Create();

        public LightClawSerializer()
        {
            this.runtimeTypeModel.AutoAddMissingTypes = true;
            this.runtimeTypeModel.AutoCompile = true;
        }

        public LightClawSerializer(IGameCodeInterface gameCodeInterface)
        {
            Contract.Requires<ArgumentNullException>(gameCodeInterface != null);
        }

        public Task<T> DeserializeAsync<T>(Stream source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<NotSupportedException>(source.CanRead);

            return Task.Run(() => (T)this.runtimeTypeModel.Deserialize(source, null, typeof(T)));
        }

        public Task<T> DeserializeAsync<T>(byte[] source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            using (MemoryStream ms = new MemoryStream(source))
            {
                return this.DeserializeAsync<T>(ms);
            }
        }

        public Task<T> DeserializeWithLengthPrefixAsync<T>(Stream source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<NotSupportedException>(source.CanRead);

            return Task.Run(() => (T)this.runtimeTypeModel.DeserializeWithLengthPrefix(source, null, typeof(T), PrefixStyle.Fixed32, 0));
        }

        public Task<T> DeserializeWithLengthPrefixAsync<T>(byte[] source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            using (MemoryStream ms = new MemoryStream(source))
            {
                return this.DeserializeWithLengthPrefixAsync<T>(ms);
            }
        }

        public Task SerializeAsync<T>(Stream destination, T instance)
        {
            Contract.Requires<ArgumentNullException>(destination != null);
            Contract.Requires<ArgumentNullException>(instance != null);
            Contract.Requires<NotSupportedException>(destination.CanWrite);

            return Task.Run(() => this.runtimeTypeModel.Serialize(destination, instance));
        }

        public Task<byte[]> SerializeAsync<T>(T instance)
        {
            Contract.Requires<ArgumentNullException>(instance != null);

            using (MemoryStream ms = new MemoryStream(32768))
            {
                return this.SerializeAsync(ms, instance).ContinueWith(t => ms.ToArray());
            }
        }

        public Task SerializeWithLengthPrefixAsync<T>(Stream destination, T instance)
        {
            Contract.Requires<ArgumentNullException>(destination != null);
            Contract.Requires<ArgumentNullException>(instance != null);
            Contract.Requires<NotSupportedException>(destination.CanWrite);

            return Task.Run(() => this.runtimeTypeModel.SerializeWithLengthPrefix(destination, instance, typeof(T), PrefixStyle.Fixed32, 0));
        }

        public Task<byte[]> SerializeWithLengthPrefixAsync<T>(T instance)
        {
            Contract.Requires<ArgumentNullException>(instance != null);

            using (MemoryStream ms = new MemoryStream(32768))
            {
                return this.SerializeWithLengthPrefixAsync(ms, instance).ContinueWith(t => ms.ToArray());
            }
        }
    }
}
