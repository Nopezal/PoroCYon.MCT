using System;
using System.Collections.Generic;
using System.Linq;
using TAPI;

namespace PoroCYon.MCT.Internal.ModClasses
{
    [GlobalMod]
    sealed class MProj : ModProjectile
    {
        public MProj(ModBase @base, Projectile p)
            : base(@base, p)
        {

        }
    }
}
