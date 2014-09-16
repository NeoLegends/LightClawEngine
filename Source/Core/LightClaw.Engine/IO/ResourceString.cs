using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using ProtoBuf;

namespace LightClaw.Engine.IO
{
    [DataContract, ProtoContract]
    public struct ResourceString : ICloneable, IEquatable<ResourceString>
    {
        [DataMember, ProtoMember(1)]
        public string Path { get; private set; }

        public ResourceString(string path)
            : this()
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path));

            this.Path = path;
        }

        public object Clone()
        {
            return new ResourceString(this.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is ResourceString) ? this.Equals((ResourceString)obj) : false;
        }

        public override bool Equals(ResourceString other)
        {
            return (this.Path == other.Path);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Path);
        }

        public static implicit operator string(ResourceString resourceString)
        {
            return resourceString.Path;
        }

        public static implicit operator ResourceString(string resourceString)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(resourceString));

            return new ResourceString(resourceString);
        }

        public static bool operator ==(ResourceString left, ResourceString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResourceString left, ResourceString right)
        {
            return !(left == right);
        }
    }
}
