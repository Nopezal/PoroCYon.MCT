using System;
using System.Collections.Generic;
using System.Linq;
using TAPI;

namespace PoroCYon.MCT.Internal.ModClasses
{
    sealed class MWorld : ModWorld
    {
        [CallPriority(Single.Epsilon)]
        public override bool? CheckChristmas()
        {
            return World.ForceChristmas ?? base.CheckChristmas();
        }
        [CallPriority(Single.Epsilon)]
        public override bool? CheckHalloween()
        {
            return World.ForceHalloween ?? base.CheckChristmas();
        }
    }
}
