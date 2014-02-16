using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.Internal.ModClasses
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
