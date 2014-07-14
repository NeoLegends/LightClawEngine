using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using Munq;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    [ProtoContract]
    public abstract class GLObject : IDisposable
    {
        [IgnoreDataMember, ProtoIgnore]
        public int Id { get; protected set; }

        [CLSCompliant(false)]
        [IgnoreDataMember, ProtoIgnore]
        public IocContainer Ioc { get; protected set; }

        protected GLObject() 
        {
            this.Ioc = LightClawEngine.DefaultIocContainer;
        }

        protected GLObject(int id)
            : this()
        {
            this.Id = id;
        }

        ~GLObject()
	    {
            this.Dispose(false);
	    }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Id = 0;
            GC.SuppressFinalize(this);
        }

        public static implicit operator int(GLObject glObject)
        {
            return (glObject != null) ? glObject.Id : 0;
        }
    }
}
