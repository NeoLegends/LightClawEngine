using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class StructureInformationAttribute : Attribute
    {
        public int ComponentCount { get; private set; }

        public int ComponentSizeInBit { get; private set; }

        public int ComponentSizeInBytes
        {
            get
            {
                return this.ComponentSizeInBit / 8;
            }
        }

        public bool IsFloatingPoint { get; private set; }

        public int TotalSize
        {
            get
            {
                return this.ComponentCount * this.ComponentSizeInBit;
            }
        }

        public int TotalSizeInBytes
        {
            get
            {
                return this.TotalSize / 8;
            }
        }

        public StructureInformationAttribute(int componentCount, int componentSizeInBit, bool isFloatingPoint)
        {
            Contract.Requires<ArgumentOutOfRangeException>(componentCount > 0);
            Contract.Requires<ArgumentOutOfRangeException>(componentSizeInBit > 0);

            this.ComponentCount = componentCount;
            this.ComponentSizeInBit = componentSizeInBit;
            this.IsFloatingPoint = isFloatingPoint;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.ComponentCount > 0);
            Contract.Invariant(this.ComponentSizeInBit > 0);
        }
    }
}
