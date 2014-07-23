using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [DataContract]
    [StructureInformation(2, 8, true)]
    public struct GameTime
    {
        public static GameTime Null
        {
            get
            {
                return new GameTime();
            }
        }

        [DataMember]
        public double ElapsedSinceLastUpdate { get; private set; }

        [DataMember]
        public double TotalGameTime { get; private set; }

        public GameTime(double elapsedSinceUpdate, double totalTime)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(elapsedSinceUpdate >= 0.0);
            Contract.Requires<ArgumentOutOfRangeException>(totalTime >= 0.0);

            this.ElapsedSinceLastUpdate = elapsedSinceUpdate;
            this.TotalGameTime = totalTime;
        }
    }
}
