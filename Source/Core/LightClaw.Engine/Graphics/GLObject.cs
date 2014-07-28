using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using Munq;

namespace LightClaw.Engine.Graphics
{
    [DataContract]
    public abstract class GLObject : Entity, IDisposable
    {
        [IgnoreDataMember]
        public int Handle { get; protected set; }

        protected GLObject() { }

        protected GLObject(int id)
            : this()
        {
            this.Handle = id;
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
            this.Handle = 0;
            GC.SuppressFinalize(this);
        }

        public static implicit operator int(GLObject glObject)
        {
            return (glObject != null) ? glObject.Handle : 0;
        }
    }
}
