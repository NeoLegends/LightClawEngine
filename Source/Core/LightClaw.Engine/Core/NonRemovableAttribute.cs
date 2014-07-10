using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal sealed class NonRemovableAttribute : RemovalValidatorAttribute
    {
        public NonRemovableAttribute() : base(null) { }

        public override bool Validate(GameObject gameObjectToRemoveFrom)
        {
            return false;
        }
    }
}
