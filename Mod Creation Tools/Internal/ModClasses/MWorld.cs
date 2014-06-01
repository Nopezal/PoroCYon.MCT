using System;
using System.Collections.Generic;
using System.Linq;
using TAPI;

namespace PoroCYon.MCT.Internal.ModClasses
{
    [GlobalMod]
    sealed class MWorld : ModWorld
    {
        public MWorld(ModBase @base)
            : base(@base)
        {

        }

        [CallPriority(Single.Epsilon)]
        public override bool? CheckChristmas()
        {
            bool forced = World.ForceChristmas;
            World.ForceChristmas = false;

            return base.CheckChristmas().HasValue ? base.CheckChristmas() : forced;
        }
        [CallPriority(Single.Epsilon)]
        public override bool? CheckHalloween()
        {
            bool forced = World.ForceHalloween;
            World.ForceHalloween = false;

            return base.CheckHalloween().HasValue ? base.CheckHalloween() : forced;
        }
    }
}
